[System.Serializable]
public class Vertex
{
    public int index;
    public int x;
    public int y;
    public int r;
    public int[] edges;
    public string type;
    public bool caution;
    public bool room;
    public bool restroom;
    public bool stairs;
    public bool elevator;
    public bool wheelchair;
    public string message;
    // for pathfinding
    public float shortestDist = 999999;
    public Vertex parent = null;
    public int floorIndex;
}