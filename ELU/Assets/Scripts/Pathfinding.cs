using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Pathfinding : MonoBehaviour
{
    public Texture2D originalFloor;
    private Texture2D floor;
    private SpriteRenderer spriteRenderer;
    public GameObject floorObj;
    private TextureEditor textureEditor;
    public Vertex[] graph;
    private Queue<Vertex> queue = new Queue<Vertex>();
    public Texture2D startIcon, endIcon;

    void Start()
    {
        // tex
        textureEditor = gameObject.GetComponent<TextureEditor>();
        spriteRenderer = floorObj.GetComponent<SpriteRenderer>();
        // json
        TextAsset floorJSON = (TextAsset)Resources.Load("AstraSilva0Aligned");
        string json = floorJSON.text;
        //string json = fileToString("AstraSilva0Aligned.txt");
        //Debug.Log(json);
        graph = JsonHelper.getJsonArray<Vertex>(json);
    }

    public void FindPath (int from, int to) {
        // new texture
        floor = new Texture2D(originalFloor.width, originalFloor.height);
        floor.SetPixels(originalFloor.GetPixels());
        floor.Apply();
        //new graph
        ResetGraph();
        floor = GetFloorWithPath(graph[from], graph[to], floor, new Color(0.7f, 0.1f, 0.2f)); // new Color(0.72f, 0.07f, 0.2f)
        // applying the returned texture
        spriteRenderer.sprite = Sprite.Create(floor, new Rect(0.0f, 0.0f, floor.width, floor.height), new Vector2(0.5f, 0.5f), 1.0f);
    }

    public void ResetGraph () {
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

    public Texture2D GetFloorWithPath (Vertex startVertex, Vertex endVertex, Texture2D floor, Color color) {
        // calling dijkstra
        startVertex.shortestDist = 0;
        queue.Enqueue(startVertex);
        Dijkstra(startVertex, endVertex);
        // drawing the path recursively from the endVertex to the startVertex through parents
        Vertex currentDrawVertex = endVertex;
        RecursiveDraw(endVertex, floor, color, 0);
        textureEditor.DrawEllipse(startVertex.x, floor.height - startVertex.y, 20, 20, floor, new Color(0.6f, 0.6f, 0.6f));
        textureEditor.DrawEllipse(endVertex.x, floor.height - endVertex.y, 20, 20, floor, new Color(0.6f, 0.6f, 0.6f));
        textureEditor.DrawImage(startVertex.x, floor.height - startVertex.y, floor, startIcon, color);
        textureEditor.DrawImage(endVertex.x, floor.height - endVertex.y, floor, endIcon, color);
        // returning the final texture
        return floor;
    }

    private void RecursiveDraw (Vertex vertex, Texture2D floor, Color color, int count) {
        if (count < graph.Length && vertex.parent != null) {
            textureEditor.DrawLine(vertex.x, floor.height - vertex.y, vertex.parent.x, floor.height - vertex.parent.y, floor, Color.black, 8, 0);
            textureEditor.DrawLine(vertex.x, floor.height - vertex.y, vertex.parent.x, floor.height - vertex.parent.y, floor, color, 6, 0);
            RecursiveDraw(vertex.parent, floor, color, count++);
        }
    }

    private void Dijkstra (Vertex start, Vertex dest) {
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
