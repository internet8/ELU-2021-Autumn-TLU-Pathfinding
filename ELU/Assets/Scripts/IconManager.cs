using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconManager : MonoBehaviour
{
    public GameObject cautionIcon, locationIcon;
    public List<GameObject> spawnedIcons = new List<GameObject>();

    public void GenerateCautionIcons (Vertex[] graph) {
        foreach (Vertex v in graph) {
            if (v.caution) {
                CreateIcon(new Vector2(v.x - 843, 1191 - v.y - 594), cautionIcon, v.message);
            }
        }
    }

    public void GenerateLocationIcons (Vector2 posStart, Vector2 posEnd, string nameStart, string nameEnd) {
        CreateIcon(new Vector2(posStart.x - 843, 1191 - posStart.y - 594), locationIcon, nameStart);
        CreateIcon(new Vector2(posEnd.x - 843, 1191 - posEnd.y - 594), locationIcon, nameEnd);
    }

    public void CreateIcon (Vector2 position, GameObject icon, string name) {
        GameObject i = Instantiate(icon, position, Quaternion.identity);
        i.name = name;
        spawnedIcons.Add(i);
    }

    public void DeleteCautionIcons () {
        foreach (GameObject go in spawnedIcons) {
            if (go.tag == "caution") {
                Destroy(go);
            }
        }
        spawnedIcons = new List<GameObject>();
    }

    public void DeleteLocationIcons () {
        foreach (GameObject go in spawnedIcons) {
            if (go.tag == "location") {
                Destroy(go);
            }
        }
    }
}
