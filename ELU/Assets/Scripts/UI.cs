using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    public GameObject searchResultObj, fromObj, toObj;
    public GameObject searchCanvas, mapCanvas;
    private TMP_InputField from, to;
    private Pathfinding pathfinding;
    private int fromIndex = -1;
    private int toIndex = -1;

    void Start()
    {
        pathfinding = gameObject.GetComponent<Pathfinding>();
        from = fromObj.GetComponent<TMP_InputField>();
        to = toObj.GetComponent<TMP_InputField>();
    }

    public void ShowPath () {
        if (fromIndex > -1 && toIndex > -1 && fromIndex != toIndex) {
            pathfinding.FindPath(fromIndex, toIndex);
            searchCanvas.SetActive(false);
            mapCanvas.SetActive(true);
        }
    }

    public void Search (bool b) {
        DeleteSearchResults();
        string input = b ? from.text : to.text;
        int index = 0;
        foreach (Vertex v in pathfinding.originalGraph)
        {
            if (v.type.Length >= input.Length) {
                if (v.type.Substring(0, input.Length).ToLower().Equals(input.ToLower())) {
                    var button = (GameObject)Instantiate(Resources.Load("SearchResultButton", typeof(GameObject))) as GameObject;
                    if (button == null) continue;
                    button.transform.SetParent (searchResultObj.transform);
                    button.transform.localScale = Vector3.one;
                    button.transform.localRotation = Quaternion.Euler (Vector3.zero);
                    button.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(index, index, 0);
                    button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = v.type;
                    button.GetComponent<Button>().onClick.AddListener(delegate { 
                        if (b) { 
                            fromIndex = v.index;
                            from.text = "From: " + v.type;
                        } else {
                            toIndex = v.index;
                            to.text = "To: " + v.type;
                        }
                        DeleteSearchResults();
                    });
                    index++;
                }
            }
        }
    }

    void DeleteSearchResults () {
        foreach (Transform child in searchResultObj.transform) {
            Destroy(child.gameObject);
        }
    }

    public void OpenSearch () {
        searchCanvas.SetActive(true);
        mapCanvas.SetActive(false);
    }
}
