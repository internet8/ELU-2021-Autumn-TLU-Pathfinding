using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagePass : MonoBehaviour
{
    public Texture2D astra_silva_floor_0;
    private byte[,,] haarFeatures = new byte[8, 3, 3] {
        {{0, 0, 0}, {0, 1, 1}, {0, 1, 1}},
        {{0, 0, 0}, {1, 1, 0}, {1, 1, 0}},
        {{1, 1, 0}, {1, 1, 0}, {0, 0, 0}},
        {{0, 1, 1}, {0, 1, 1}, {0, 0, 0}},
        {{0, 1, 1}, {1, 1, 1}, {1, 1, 1}},
        {{1, 1, 0}, {1, 1, 1}, {1, 1, 1}},
        {{1, 1, 1}, {1, 1, 1}, {0, 1, 1}},
        {{1, 1, 1}, {1, 1, 1}, {1, 1, 0}}
    };

    void Start()
    {
        
    }

    public List<Vector3> GetWallBaseVertices () {
        return null;
    }
}
