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
    }

    public void FindPath (int floorFrom, int floorTo, int indexFrom, int indexTo) {
        noPathError = false;
        RestoreDefaultTextures();
        if (floorFrom == floorTo) {
            ResetGraph(graphs[floorFrom]);
            List<Vector3> pathNodesFloor = GetFloorWithPath(graphs[floorFrom], graphs[floorFrom][indexFrom], graphs[floorFrom][indexTo]);
            if (!noPathError) {
                lineRendererFrom.positionCount = pathNodesFloor.Count;
                lineRendererFrom.SetPositions(pathNodesFloor.ToArray());
            }
        } else {
            //new graph
            ResetGraph(graphs[floorFrom]);
            ResetGraph(graphs[floorTo]);
            // finding paths
            FloorConnection fc = GetFloorConnection(floorFrom, floorTo, indexFrom, indexTo);
            List<Vector3> pathNodesFloorFrom = GetFloorWithPath(graphs[floorTo], graphs[floorTo][fc.connectionArray[floorTo]], graphs[floorTo][indexTo]);
            List<Vector3> pathNodesFloorTo = GetFloorWithPath(graphs[floorFrom], graphs[floorFrom][indexFrom], graphs[floorFrom][fc.connectionArray[floorFrom]]);
            if (!noPathError) {
                lineRendererFrom.positionCount = pathNodesFloorFrom.Count;
                lineRendererFrom.SetPositions(pathNodesFloorFrom.ToArray());
                lineRendererTo.positionCount = pathNodesFloorTo.Count;
                lineRendererTo.SetPositions(pathNodesFloorTo.ToArray());
            }
        }
        if (!noPathError) {
            ui.showPath = true;
            ui.SwitchMap(floorFrom);
        }
    }

    public void RestoreDefaultTextures () {
        // remove all line renderers
        ui.showPath = false;
        ui.SwitchMap(0);
    }

    private FloorConnection GetFloorConnection (int floorFrom, int floorTo, int indexFrom, int indexTo) {
        float shortestDist = 10000;
        FloorConnection result = null;
        foreach (FloorConnection fc in floorConnections) {
            if (fc.connectionArray[floorFrom] >= 0 && fc.connectionArray[floorTo] >= 0) {
                Vector2 from = new Vector2(graphs[floorFrom][indexFrom].x, graphs[floorFrom][indexFrom].y);
                Vector2 from2 = new Vector2(graphs[floorTo][indexTo].x, graphs[floorTo][indexTo].y);
                Vector2 to = new Vector2(graphs[floorFrom][fc.connectionArray[floorFrom]].x, graphs[floorFrom][fc.connectionArray[floorFrom]].y);
                Vector2 to2 = new Vector2(graphs[floorTo][fc.connectionArray[floorTo]].x, graphs[floorTo][fc.connectionArray[floorTo]].y);
                float dist = Vector2.Distance(from, to) + Vector2.Distance(from2, to2);
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

    public List<Vector3> GetFloorWithPath (Vertex[] graph, Vertex startVertex, Vertex endVertex) {
        List<Vector3> result = new List<Vector3>();
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
                currentDrawVertex = currentDrawVertex.parent;
                count++;
            }
        } else {
            noPathError = true;
            Debug.Log("No path found!");
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
        Debug.Log(iterations);
    }
}