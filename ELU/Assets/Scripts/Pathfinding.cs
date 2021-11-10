using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Pathfinding : MonoBehaviour
{
    //public Texture2D originalFloor0, originalFloor1, originalFloor2, originalFloor3, originalFloor4, originalFloor5;
    //private Texture2D floor0, floor1, floor2, floor3, floor4, floor5;
    public Texture2D[] originalFloors = new Texture2D[6];
    public Sprite[] originalSprites = new Sprite[6];
    private SpriteRenderer spriteRenderer;
    public GameObject floorObj;
    private TextureEditor textureEditor;
    //public Vertex[] graph0, graph1, graph2, graph3, graph4, graph5;
    public List<Vertex[]> graphs = new List<Vertex[]>();
    private Queue<Vertex> queue = new Queue<Vertex>();
    public Texture2D startIcon, endIcon;
    private FloorConnection[] floorConnections;

    void Start()
    {
        // tex
        textureEditor = gameObject.GetComponent<TextureEditor>();
        spriteRenderer = floorObj.GetComponent<SpriteRenderer>();
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
        Debug.Log(graphs.Count);
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
        RestoreDefaultTextures();
        if (floorFrom == floorTo) {
            Texture2D floorTex = new Texture2D(originalFloors[floorFrom].width, originalFloors[floorFrom].height);
            floorTex.SetPixels(originalFloors[floorFrom].GetPixels());
            floorTex.Apply();
            ResetGraph(graphs[floorFrom]);
            floorTex = GetFloorWithPath(graphs[floorFrom], graphs[floorFrom][indexFrom], graphs[floorFrom][indexTo], floorTex, new Color(0.7f, 0.1f, 0.2f));
            ApplyNewFloorTextures(floorFrom, Sprite.Create(floorTex, new Rect(0.0f, 0.0f, floorTex.width, floorTex.height), new Vector2(0.5f, 0.5f), 1.0f));
        } else {
            // new textures
            Texture2D floorFromTex = new Texture2D(originalFloors[floorFrom].width, originalFloors[floorFrom].height);
            floorFromTex.SetPixels(originalFloors[floorFrom].GetPixels());
            floorFromTex.Apply();
            Texture2D toFloorTex = new Texture2D(originalFloors[floorTo].width, originalFloors[floorTo].height);
            toFloorTex.SetPixels(originalFloors[floorTo].GetPixels());
            toFloorTex.Apply();
            //new graph
            ResetGraph(graphs[floorFrom]);
            ResetGraph(graphs[floorTo]);
            // finding paths
            FloorConnection fc = GetFloorConnection(floorFrom, floorTo, indexFrom, indexTo);
            toFloorTex = GetFloorWithPath(graphs[floorTo], graphs[floorTo][fc.connectionArray[floorTo]], graphs[floorTo][indexTo], toFloorTex, new Color(0.7f, 0.1f, 0.2f));
            ApplyNewFloorTextures(floorTo, Sprite.Create(toFloorTex, new Rect(0.0f, 0.0f, toFloorTex.width, toFloorTex.height), new Vector2(0.5f, 0.5f), 1.0f));
            floorFromTex = GetFloorWithPath(graphs[floorFrom], graphs[floorFrom][indexFrom], graphs[floorFrom][fc.connectionArray[floorFrom]], floorFromTex, new Color(0.7f, 0.1f, 0.2f));
            ApplyNewFloorTextures(floorFrom, Sprite.Create(floorFromTex, new Rect(0.0f, 0.0f, floorFromTex.width, floorFromTex.height), new Vector2(0.5f, 0.5f), 1.0f));
            //floorFromTex = GetFloorWithPath(graph, graph[from], graph[to], floorFromTex, new Color(0.7f, 0.1f, 0.2f)); // new Color(0.72f, 0.07f, 0.2f)
            // applying the returned texture
            //spriteRenderer.sprite = Sprite.Create(floorFromTex, new Rect(0.0f, 0.0f, floorFromTex.width, floorFromTex.height), new Vector2(0.5f, 0.5f), 1.0f);
        }
    }

    private void ApplyNewFloorTextures (int floorIndex, Sprite floorTex) {
        UI ui = gameObject.GetComponent<UI>();
        switch (floorIndex) {
            case 0:
                ui.floor0 = floorTex;
                break;
            case 1:
                ui.floor1 = floorTex;
                break;
            case 2:
                ui.floor2 = floorTex;
                break;
            case 3:
                ui.floor3 = floorTex;
                break;
            case 4:
                ui.floor4 = floorTex;
                break;
            case 5:
                ui.floor5 = floorTex;
                break;
        }
        ui.SwitchMap(floorIndex);
    }

    public void RestoreDefaultTextures () {
        UI ui = gameObject.GetComponent<UI>();
        ui.floor0 = originalSprites[0];
        ui.floor1 = originalSprites[1];
        ui.floor2 = originalSprites[2];
        ui.floor3 = originalSprites[3];
        ui.floor4 = originalSprites[4];
        ui.floor5 = originalSprites[5];
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
                    shortestDist = dist;
                    result = fc;
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

    public Texture2D GetFloorWithPath (Vertex[] graph, Vertex startVertex, Vertex endVertex, Texture2D floor, Color color) {
        // calling dijkstra
        startVertex.shortestDist = 0;
        queue.Enqueue(startVertex);
        Dijkstra(startVertex, endVertex, graph);
        // drawing the path recursively from the endVertex to the startVertex through parents
        Vertex currentDrawVertex = endVertex;
        RecursiveDraw(endVertex, floor, color, 0, graph);
        textureEditor.DrawEllipse(startVertex.x, floor.height - startVertex.y, 22, 22, floor, Color.black);
        textureEditor.DrawEllipse(startVertex.x, floor.height - startVertex.y, 20, 20, floor, new Color(0.6f, 0.6f, 0.6f));
        textureEditor.DrawEllipse(endVertex.x, floor.height - endVertex.y, 22, 22, floor, Color.black);
        textureEditor.DrawEllipse(endVertex.x, floor.height - endVertex.y, 20, 20, floor, new Color(0.6f, 0.6f, 0.6f));
        textureEditor.DrawImage(startVertex.x, floor.height - startVertex.y, floor, startIcon, color);
        textureEditor.DrawImage(endVertex.x, floor.height - endVertex.y, floor, endIcon, color);
        // returning the final texture
        return floor;
    }

    private void RecursiveDraw (Vertex vertex, Texture2D floor, Color color, int count, Vertex[] graph) {
        if (count < graph.Length && vertex.parent != null) {
            textureEditor.DrawLine(vertex.x, floor.height - vertex.y, vertex.parent.x, floor.height - vertex.parent.y, floor, Color.black, 8, 0);
            textureEditor.DrawLine(vertex.x, floor.height - vertex.y, vertex.parent.x, floor.height - vertex.parent.y, floor, color, 6, 0);
            RecursiveDraw(vertex.parent, floor, color, count++, graph);
        }
    }

    private void Dijkstra (Vertex start, Vertex dest, Vertex[] graph) {
        Vertex vertex = start;
        while (queue.Count > 0) {
            vertex = queue.Dequeue();
            foreach (int edge in vertex.edges) {
                Vertex v = graph[edge];
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
}
