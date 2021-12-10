using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Pathfinding : MonoBehaviour
{
    public List<Vertex[]> graphs = new List<Vertex[]>();
    private Queue<Vertex> queue = new Queue<Vertex>();
    private FloorConnection[] floorConnections;
    public bool shortestPath = true;
    private bool noPathError = false;
    public LineRenderer lineRendererFrom;
    public LineRenderer lineRendererTo;
    private UI ui;
    private IconManager iconManager;
    public string pathText = "";
    public string pathText2 = "";

    void Start()
    {
        ui = gameObject.GetComponent<UI>();
        iconManager = gameObject.GetComponent<IconManager>();
        GraphsInit();
    }

    private void GraphsInit () {
        // json
        TextAsset floorJSON = (TextAsset)Resources.Load("AstraSilva0Aligned");
        string json = floorJSON.text;
        //string json = fileToString("AstraSilva0Aligned.txt");
        //Debug.Log(json);
        graphs.Add(JsonHelper.getJsonArray<Vertex>(json));
        floorJSON = (TextAsset)Resources.Load("AstraSilva1Aligned");
        json = floorJSON.text;
        graphs.Add(JsonHelper.getJsonArray<Vertex>(json));
        floorJSON = (TextAsset)Resources.Load("AstraSilva2Aligned");
        json = floorJSON.text;
        graphs.Add(JsonHelper.getJsonArray<Vertex>(json));
        floorJSON = (TextAsset)Resources.Load("AstraSilva3Aligned");
        json = floorJSON.text;
        graphs.Add(JsonHelper.getJsonArray<Vertex>(json));
        floorJSON = (TextAsset)Resources.Load("AstraSilva4Aligned");
        json = floorJSON.text;
        graphs.Add(JsonHelper.getJsonArray<Vertex>(json));
        floorJSON = (TextAsset)Resources.Load("AstraSilva5Aligned");
        json = floorJSON.text;
        graphs.Add(JsonHelper.getJsonArray<Vertex>(json));
        // connecting graphs
        FloorConnection A002Ramp = new FloorConnection(false, false, true, new int[] { 102, 82, -1, -1, -1, -1 });
        FloorConnection A002Stairs = new FloorConnection(true, false, false, new int[] { 41, 72, -1, -1, -1, -1 });
        FloorConnection southStairs = new FloorConnection(true, false, false, new int[] { -1, -1, 57, 72, 63, 99 });
        FloorConnection northWestStairs = new FloorConnection(true, false, false, new int[] { -1, 63, 60, 69, 56, 93 });
        FloorConnection northStairs = new FloorConnection(true, false, false, new int[] { -1, 64, 55, 70, 57, 94 });
        FloorConnection northEastStairs = new FloorConnection(true, false, false, new int[] { -1, 76, 56, 71, 58, 95 });
        FloorConnection southEastStairs = new FloorConnection(true, false, false, new int[] { -1, -1, -1, 74, 62, 97 });
        FloorConnection southWestStairs = new FloorConnection(true, false, false, new int[] { -1, -1, 59, 77, 66, 102 });
        FloorConnection spiralStairs = new FloorConnection(true, false, false, new int[] { -1, 67, 58, -1, -1, -1 });
        FloorConnection northElevatorSilva = new FloorConnection(false, true, false, new int[] { -1, 61, 61, 76, 67, 103 });
        FloorConnection northElevatorAstra = new FloorConnection(false, true, false, new int[] { 36, 60, 61, 76, 67, 103 }); // first index is 36
        FloorConnection southWestElevator = new FloorConnection(false, true, false, new int[] { -1, 62, 62, 77, 68, 104 });
        floorConnections = new FloorConnection[] { A002Ramp, A002Stairs, southStairs, northWestStairs, northStairs, northEastStairs, southEastStairs, southWestStairs, spiralStairs, northElevatorAstra, northElevatorSilva, southWestElevator };
        ui.SwitchMap(1);
    }

    public void FindPath (int floorFrom, int floorTo, int indexFrom, int indexTo) {
        noPathError = false;
        pathText = "";
        pathText2 = "";
        RestoreDefaultTextures();
        if (floorFrom == floorTo) {
            ResetGraph(graphs[floorFrom]);
            List<Vector3> pathNodesFloor = GetFloorWithPath(graphs[floorFrom], graphs[floorFrom][indexFrom], graphs[floorFrom][indexTo], floorTo, true, true);
            if (!noPathError) {
                lineRendererTo.positionCount = pathNodesFloor.Count;
                lineRendererTo.SetPositions(pathNodesFloor.ToArray());
                ui.pathTwoLen = GetPathLength(pathNodesFloor);
                // icons
                GameObject startIcon = lineRendererTo.gameObject.transform.GetChild(0).gameObject;
                GameObject endIcon = lineRendererTo.gameObject.transform.GetChild(1).gameObject;
                startIcon.SetActive(true);
                endIcon.SetActive(true);
                startIcon.transform.position = new Vector2(pathNodesFloor[pathNodesFloor.Count - 1].x - 843, pathNodesFloor[pathNodesFloor.Count - 1].y - 594);
                endIcon.transform.position = new Vector2(pathNodesFloor[0].x - 843, pathNodesFloor[0].y - 594);
                startIcon.name = "Starting location: " + graphs[floorFrom][indexFrom].type + ".";
                if (graphs[floorFrom][indexFrom].caution) {
                    startIcon.SetActive(false);
                }
                endIcon.name = "Destination: " + graphs[floorFrom][indexTo].type + ".";
                if (graphs[floorFrom][indexTo].caution) {
                    endIcon.SetActive(false);
                }
                // icons old
                // Vector2 startIcon = new Vector2(graphs[floorFrom][indexFrom].x, graphs[floorFrom][indexFrom].y);
                // Vector2 endIcon = new Vector2(graphs[floorFrom][indexTo].x, graphs[floorFrom][indexTo].y);
                //iconManager.GenerateLocationIcons(startIcon, endIcon, "Starting location: " + graphs[floorFrom][indexFrom].type, "Destination: " + graphs[floorFrom][indexTo].type);
            }
        } else {
            //new graph
            ResetGraph(graphs[floorFrom]);
            ResetGraph(graphs[floorTo]);
            // finding paths
            FloorConnection fc = GetFloorConnection(floorFrom, floorTo, indexFrom, indexTo);
            if (fc == null) {
                noPathError = true;
                ui.ShowMessage("No paths found!");
                return;
            }
            List<Vector3> pathNodesFloorTo = GetFloorWithPath(graphs[floorFrom], graphs[floorFrom][indexFrom], graphs[floorFrom][fc.connectionArray[floorFrom]], floorTo, true, true);
            List<Vector3> pathNodesFloorFrom = GetFloorWithPath(graphs[floorTo], graphs[floorTo][fc.connectionArray[floorTo]], graphs[floorTo][indexTo], floorTo, true);
            if (!noPathError) {
                lineRendererFrom.positionCount = pathNodesFloorFrom.Count;
                lineRendererFrom.SetPositions(pathNodesFloorFrom.ToArray());
                ui.pathOneLen = GetPathLength(pathNodesFloorFrom);
                lineRendererTo.positionCount = pathNodesFloorTo.Count;
                lineRendererTo.SetPositions(pathNodesFloorTo.ToArray());
                ui.pathTwoLen = GetPathLength(pathNodesFloorTo);
                // icons
                GameObject startIcon = lineRendererFrom.gameObject.transform.GetChild(0).gameObject;
                GameObject endIcon = lineRendererFrom.gameObject.transform.GetChild(1).gameObject;
                startIcon.SetActive(true);
                endIcon.SetActive(true);
                startIcon.transform.position = new Vector2(pathNodesFloorFrom[pathNodesFloorFrom.Count - 1].x - 843, pathNodesFloorFrom[pathNodesFloorFrom.Count - 1].y - 594);
                endIcon.transform.position = new Vector2(pathNodesFloorFrom[0].x - 843, pathNodesFloorFrom[0].y - 594);
                startIcon.name = "Starting location: " + graphs[floorTo][fc.connectionArray[floorTo]].type + ".";
                if (graphs[floorTo][fc.connectionArray[floorTo]].caution) {
                    startIcon.SetActive(false);
                }
                endIcon.name = "Destination: " + graphs[floorTo][indexTo].type + ".";
                if (graphs[floorTo][indexTo].caution) {
                    endIcon.SetActive(false);
                }
                // icons for second path floor
                startIcon = lineRendererTo.gameObject.transform.GetChild(0).gameObject;
                endIcon = lineRendererTo.gameObject.transform.GetChild(1).gameObject;
                startIcon.SetActive(true);
                endIcon.SetActive(true);
                startIcon.transform.position = new Vector2(pathNodesFloorTo[pathNodesFloorTo.Count - 1].x - 843, pathNodesFloorTo[pathNodesFloorTo.Count - 1].y - 594);
                endIcon.transform.position = new Vector2(pathNodesFloorTo[0].x - 843, pathNodesFloorTo[0].y - 594);
                startIcon.name = "Starting location: " + graphs[floorFrom][indexFrom].type + ".";
                if (graphs[floorFrom][indexFrom].caution) {
                    startIcon.SetActive(false);
                }
                endIcon.name = "Destination: " + graphs[floorFrom][fc.connectionArray[floorFrom]].type + ".";
                if (graphs[floorFrom][fc.connectionArray[floorFrom]].caution) {
                    endIcon.SetActive(false);
                }
                // icons old
                // Vector2 startIcon = new Vector2(graphs[floorFrom][indexFrom].x, graphs[floorFrom][indexFrom].y);
                // Vector2 endIcon = new Vector2(graphs[floorFrom][fc.connectionArray[floorFrom]].x, graphs[floorFrom][fc.connectionArray[floorFrom]].y);
                //iconManager.GenerateLocationIcons(startIcon, endIcon, "Starting location: " + graphs[floorFrom][indexFrom].type, "Destination: " + graphs[floorFrom][fc.connectionArray[floorFrom]].type);
                // icons for the second floor too!
            }
        }
        if (!noPathError) {
            ui.showPath = true;
            ui.SwitchMap(floorFrom);
            ui.ShowMessage(pathText, pathText2);
        }
    }

    public void RestoreDefaultTextures () {
        // remove all line renderers
        ui.showPath = false;
        ui.ResetUI();
        ui.HideMessage();
        ui.SwitchMap(ui.currentFloor);
    }

    private FloorConnection GetFloorConnection (int floorFrom, int floorTo, int indexFrom, int indexTo) {
        float shortestDist = 100000;
        FloorConnection result = null;
        foreach (FloorConnection fc in floorConnections) {
            //Debug.Log(fc.stairs + ", " + fc.elevator + ", " + fc.ramp);
            if (fc.connectionArray[floorFrom] >= 0 && fc.connectionArray[floorTo] >= 0) {
                // Vector2 from = new Vector2(graphs[floorFrom][indexFrom].x, graphs[floorFrom][indexFrom].y);
                // Vector2 from2 = new Vector2(graphs[floorTo][indexTo].x, graphs[floorTo][indexTo].y);
                // Vector2 to = new Vector2(graphs[floorFrom][fc.connectionArray[floorFrom]].x, graphs[floorFrom][fc.connectionArray[floorFrom]].y);
                // Vector2 to2 = new Vector2(graphs[floorTo][fc.connectionArray[floorTo]].x, graphs[floorTo][fc.connectionArray[floorTo]].y);
                // with liear distance there are some issues with wheelchair paths, path based distance needed
                //float dist = Vector2.Distance(from, to) + Vector2.Distance(from2, to2);

                // finding a path and it's distance, wel'll need this to find the best floor connection
                List<Vector3> testPathFrom = GetFloorWithPath(graphs[floorFrom], graphs[floorFrom][indexFrom], graphs[floorFrom][fc.connectionArray[floorFrom]], floorTo, false);
                float dist = 100000;
                if (testPathFrom.Count > 0) {
                    dist = GetPathLength(testPathFrom);
                }
                ResetGraph(graphs[floorFrom]);
                List<Vector3> testPathTo = GetFloorWithPath(graphs[floorTo], graphs[floorTo][fc.connectionArray[floorTo]], graphs[floorTo][indexTo], floorTo, false);
                if (testPathTo.Count > 0) {
                    dist += GetPathLength(testPathTo);
                } else {
                    dist += 100000;
                }
                //Debug.Log(dist);
                ResetGraph(graphs[floorTo]);
                if (dist < shortestDist) {
                    if (shortestPath) {
                        shortestDist = dist;
                        result = fc;
                    } else if (fc.elevator || fc.ramp) {
                        shortestDist = dist;
                        result = fc;
                    }
                }
            }
        }
        // foreach (int i in result.connectionArray) {
        //     Debug.Log(i);
        // }
        noPathError = false;
        return result;
    }

    public void ResetGraph (Vertex[] graph) {
        foreach (Vertex v in graph) {
            v.shortestDist = 999999;
            v.parent = null;
        }
    }

    public string fileToString (string fileName) {
        string result = "";
        var sr = new StreamReader(Application.dataPath + "/" + fileName);
        var fileContents = sr.ReadToEnd();
        sr.Close();
    
        var lines = fileContents.Split("\n"[0]);
        foreach (string line in lines) {
            result += line;
            //print (line);
        }
        return result;
    }

    public List<Vector3> GetFloorWithPath (Vertex[] graph, Vertex startVertex, Vertex endVertex, int floorTo, bool generateText, bool text1 = false) {
        List<Vector3> result = new List<Vector3>();
        List<Vertex> vertexPath = new List<Vertex>();
        // calling dijkstra
        startVertex.shortestDist = 0;
        queue.Enqueue(startVertex);
        Dijkstra(startVertex, endVertex, graph);
        // finding vertices in path
        Vertex currentDrawVertex = endVertex;
        if (endVertex.parent != null) {
            int count = 0;
            while (count < graph.Length && currentDrawVertex != null) {
                result.Add(new Vector3(currentDrawVertex.x, 1191 - currentDrawVertex.y, 0)); // 1191 is the high of the floor image
                vertexPath.Add(currentDrawVertex);
                currentDrawVertex = currentDrawVertex.parent;
                count++;
            }
            if (generateText) {
                if (text1) {
                    pathText += PathToText(vertexPath, floorTo);
                } else {
                    pathText2 += PathToText(vertexPath, floorTo);
                }
            }
        } else {
            noPathError = true;
            //Debug.Log("No path found!");
            ui.ShowMessage("No paths found!");
        }
        return result;
    }

    public int GetClosestRestroomFloor (int fromFloor) {
        if (shortestPath) {
            return fromFloor;
        } else if (fromFloor == 0 || fromFloor == 2 || fromFloor == 5) {
            return fromFloor;
        } else if (fromFloor <= 1) {
            return 0;
        } else if (fromFloor <= 3) {
            return 2;
        }
        return 5;
    }

    public int GetClosestRestroom (int floorFrom, int floorTo, int index) {
        //Debug.Log(floor + ", " + index);
        int result = -1;
        float shortestDist = 10000;
        float dist = 10000;
        foreach (Vertex v in graphs[floorTo]) {
            if (!shortestPath) {
                if (v.restroom && v.wheelchair) {
                    Vector2 from = new Vector2(graphs[floorFrom][index].x, graphs[floorFrom][index].y);
                    Vector2 to = new Vector2(v.x, v.y);
                    dist = Vector2.Distance(from, to);
                }
            } else {
                if (v.restroom && !v.wheelchair) {
                    Vector2 from = new Vector2(graphs[floorFrom][index].x, graphs[floorFrom][index].y);
                    Vector2 to = new Vector2(v.x, v.y);
                    dist = Vector2.Distance(from, to);
                }
            }
            if (dist < shortestDist) {
                shortestDist = dist;
                result = v.index;
            }
        }
        return result;
    }

    private void Dijkstra (Vertex start, Vertex dest, Vertex[] graph) {
        int iterations = 0;
        Vertex vertex = start;
        while (queue.Count > 0) {
            iterations++;
            vertex = queue.Dequeue();
            foreach (int edge in vertex.edges) {
                Vertex v = graph[edge];
                if (!shortestPath) {
                    if (!v.stairs) {
                        // distance from the starting point to the current point
                        float dist = vertex.shortestDist + Vector2.Distance(new Vector2(vertex.x, vertex.y), new Vector2(v.x, v.y));
                        // +3000 distance if the vertex has the room flag, encourages the algorithm to find paths through the hallway
                        dist += v.room ? 3000 : 0;
                        dist += v.caution ? 3000 : 0;
                        if (v.shortestDist > dist) {
                            v.parent = vertex;
                            v.shortestDist = dist;
                            queue.Enqueue(v);
                        }
                    }
                } else {
                    // distance from the starting point to the current point
                    float dist = vertex.shortestDist + Vector2.Distance(new Vector2(vertex.x, vertex.y), new Vector2(v.x, v.y));
                    // +3000 distance if the vertex has the room flag, encourages the algorithm to find paths through the hallway
                    dist += v.room ? 3000 : 0;
                    if (v.shortestDist > dist) {
                        v.parent = vertex;
                        v.shortestDist = dist;
                        queue.Enqueue(v);
                    }
                }
            }
        }
        //Debug.Log(iterations);
    }

    private float GetPathLength (List<Vector3> path) {
        float result = 0;
        Vector3 lastV = path[0];
        foreach (Vector3 v in path) {
            result += Vector3.Distance(lastV, v);
            lastV = v;
        }
        return result;
    }

    public string PathToText (List<Vertex> path, int floorTo) {
        // 15 pixels = 92cm in real life
        string result = "";
        float distanceTraveled = 0;
        bool elevatorText = true; // for first floor, elevator needed to go from astra to silva, this boolean avoids two elevators on the path

        Vector2 lastV = new Vector2(path[path.Count-1].x, path[path.Count-1].y) - new Vector2(path[path.Count-2].x, path[path.Count-2].y);
        for (int i = path.Count-1; i > 0; i--) {
            Vector2 pos1 = new Vector2(path[i].x, path[i].y);
            Vector2 pos2 = new Vector2(path[i-1].x, path[i-1].y);
            Vector2 v = pos1 - pos2;

            float m1 = lastV.magnitude;
            float m2 = v.magnitude;
            float dot = Vector2.Dot(lastV, v);
            float angle = dot / m1 / m2;
            angle = (float)Math.Acos(angle) * 180 / (float)Math.PI;

            if (i == 1) {
                if (distanceTraveled > 1) {
                    result += "Go straight for " + Mathf.Round(distanceTraveled) + "m. ";
                }
                distanceTraveled = 0;
                if (path[i-1].stairs) {
                    result += "Use stairs to go to the " + ui.floorLabels[floorTo] + " floor. ";
                } else if (path[i-1].elevator) {
                    result += "Use elevator to go to the " + ui.floorLabels[floorTo] + " floor. ";
                } else if (path[i-1].wheelchair && !path[i-1].restroom) {
                    result += "Use ramp to go to the " + ui.floorLabels[floorTo] + " floor. ";
                } else {
                    result += "You have arrived at the selected location: " + path[i-1].type + ". ";
                }
            } else if (path[i].room) {
                if (i == path.Count-1 && path[i].room) {
                    result += "Starting position: " + path[i].type + ". ";
                } else {
                    // if (distanceTraveled > 1) {
                    //     result += "Go straight for " + Mathf.Round(distanceTraveled) + "m. ";
                    // }
                    result += "Go through room " + path[i].type + ". ";
                    distanceTraveled = -(m1 + m2) * 0.50f / 15;
                }
            } else if (path[i].stairs && i != path.Count-1) {
                if (distanceTraveled > 1) {
                    result += "Go straight for " + Mathf.Round(distanceTraveled) + "m. ";
                }
                result += "Use stairs. ";
                distanceTraveled = 0;
            } else if (path[i].elevator && elevatorText && i != path.Count-1) {
                elevatorText = false;
                if (distanceTraveled > 1) {
                    result += "Go straight for " + Mathf.Round(distanceTraveled) + "m. ";
                }
                result += "Use elevator. ";
                distanceTraveled = 0;
            } else if (path[i].wheelchair && i != path.Count-1) {
                if (distanceTraveled > 1) {
                    result += "Go straight for " + Mathf.Round(distanceTraveled) + "m. ";
                }
                result += "Use ramp. ";
                distanceTraveled = 0;
            } else if (angle > 60) {
                if (distanceTraveled > 1) {
                    result += "Go straight for " + Mathf.Round(distanceTraveled) + "m. ";
                }
                Vector2 vSum = lastV + v;
                //Debug.Log(v.x * vSum.y - vSum.x * v.y);
                if ((v.x * vSum.y - vSum.x * v.y) > 0) {
                    result += "Turn left. ";
                } else {
                    result += "Turn right. ";
                }
                distanceTraveled = 0;
            }
            distanceTraveled += (m1 + m2) * 0.50f / 15;
            lastV = v;
        }

        return result;
    }
}