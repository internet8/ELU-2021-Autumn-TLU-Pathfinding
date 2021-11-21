using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    //public Sprite floor0, floor1, floor2, floor3, floor4, floor5;
    public Sprite[] floors = new Sprite[6];
    public Button shortestButton, noObstaclesButton;
    public Button[] floorButtons = new Button[6];
    public GameObject pathFrom, pathTo;
    private Button currentFloorButton;
    private SpriteRenderer spriteRenderer;
    public GameObject floorObj;
    public GameObject searchResultObj, searchBoxObj;
    public GameObject searchCanvas, mapCanvas, searchBox;
    private TMP_InputField search;
    public TMP_Text fromText, toText, floorText;
    private Pathfinding pathfinding;
    private int fromIndex = -1;
    private int toIndex = -1;
    private int fromFloor = -1;
    private int toFloor = -1;
    private string searchLastValue = "";
    private bool fromSearch = true;
    public bool showPath = false;

    void Start()
    {
        currentFloorButton = floorButtons[1];
        spriteRenderer = floorObj.GetComponent<SpriteRenderer>();
        pathfinding = gameObject.GetComponent<Pathfinding>();
        search = searchBoxObj.GetComponent<TMP_InputField>();
        SwitchMap(0);
        SetToShortest();
    }

    public void ShowPath () {
        CloseSearchBox();
        if (fromIndex > -1 && toIndex > -1) {
            pathfinding.FindPath(fromFloor, toFloor, fromIndex, toIndex);
            searchCanvas.SetActive(false);
            mapCanvas.SetActive(true);
        } else if (fromIndex > -1) {
            Search(fromSearch);
        }
    }

    void Update() {
        if (!search.text.Equals(searchLastValue)) {
            searchLastValue = search.text;
            Search(fromSearch);
        }
    }

    public void Search (bool b) {
        DeleteSearchResults();
        string input = search.text;
        int index = 0;
        int floorIndex = 0;
        foreach (Vertex[] vertices in pathfinding.graphs) {
            foreach (Vertex v in vertices)
            {
                if (v.type.Length >= input.Length) {
                    if (v.type.Substring(0, input.Length).ToLower().Equals(input.ToLower()) && v.room) {
                        var button = (GameObject)Instantiate(Resources.Load("SearchResultButton", typeof(GameObject))) as GameObject;
                        if (button == null) continue;
                        button.transform.SetParent (searchResultObj.transform);
                        button.transform.localScale = Vector3.one;
                        button.transform.localRotation = Quaternion.Euler (Vector3.zero);
                        button.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(index, index, 0);
                        button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = v.type;
                        v.floorIndex = floorIndex;
                        button.GetComponent<Button>().onClick.AddListener(delegate { 
                            if (b) { 
                                fromFloor = v.floorIndex;
                                fromIndex = v.index;
                                fromText.text = v.type;
                                CloseSearch();
                            } else {
                                toFloor = v.floorIndex;
                                toIndex = v.index;
                                toText.text = v.type;
                                CloseSearch();
                            }
                            DeleteSearchResults();
                        });
                        index++;
                    }
                }
            }
            floorIndex++;
        }
    }

    void DeleteSearchResults () {
        foreach (Transform child in searchResultObj.transform) {
            Destroy(child.gameObject);
        }
    }

    public void CloseSearch () {
        searchCanvas.SetActive(false);
        mapCanvas.SetActive(true);
        searchBox.SetActive(true);
    }

    public void OpenSearch (bool b) {
        fromSearch = b;
        searchCanvas.SetActive(true);
        mapCanvas.SetActive(false);
        searchBox.SetActive(false);
        search.Select();
        search.ActivateInputField();
    }

    public void CloseSearchBox () {
        searchCanvas.SetActive(false);
        mapCanvas.SetActive(true);
        searchBox.SetActive(false);
    }

    public void OpenSearchBox () {
        searchCanvas.SetActive(false);
        mapCanvas.SetActive(true);
        searchBox.SetActive(true);
    }

    // map switching
    public void OnClicked(Button button)
    {
        print(button.name);
    }

    public void SetToShortest () {
        if (!pathfinding.shortestPath) {
            noObstaclesButton.GetComponent<Image>().enabled = false;
            shortestButton.GetComponent<Image>().enabled = true;
            pathfinding.shortestPath = true;
        }
    }

    public void SetToNoObstacles () {
        if (pathfinding.shortestPath) {
            noObstaclesButton.GetComponent<Image>().enabled = true;
            shortestButton.GetComponent<Image>().enabled = false;
            pathfinding.shortestPath = false;
        }
    }

    public void SwitchMap (int floor) {
        pathFrom.SetActive(false);
        pathTo.SetActive(false);
        if (fromFloor == floor && showPath) {
            pathTo.SetActive(true);
        } else if (toFloor == floor && showPath) {
            pathFrom.SetActive(true);
        }
        spriteRenderer.sprite = floors[floor];
        UnSelectedColor(currentFloorButton);
        currentFloorButton = floorButtons[floor];
        SelectedColor(floorButtons[floor]);
        floorText.text = "Astra/Silva " + floor + "th floor";
    }

    private void SelectedColor (Button button) {
        ColorBlock cb = button.colors;
        Color tlu = new Color(0.72f, 0.07f, 0.2f);
        cb.normalColor = tlu;
        cb.highlightedColor = tlu;
        cb.pressedColor = tlu;
        cb.selectedColor = tlu;
        button.colors = cb;
        button.gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color = Color.white;
    }

    private void UnSelectedColor (Button button) {
        ColorBlock cb = button.colors;
        Color tlu = new Color(0.72f, 0.07f, 0.2f);
        cb.normalColor = Color.white;
        cb.highlightedColor = Color.white;
        cb.pressedColor = Color.white;
        cb.selectedColor = Color.white;
        button.colors = cb;
        button.gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color = tlu;
    }
}
