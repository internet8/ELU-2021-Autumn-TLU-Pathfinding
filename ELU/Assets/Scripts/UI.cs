using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    public Sprite floor0, floor1, floor2, floor3, floor4, floor5;
    public Button floorButton0, floorButton1, floorButton2, floorButton3, floorButton4, floorButton5, shortestButton, noObstaclesButton;
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

    void Start()
    {
        currentFloorButton = floorButton1;
        spriteRenderer = floorObj.GetComponent<SpriteRenderer>();
        pathfinding = gameObject.GetComponent<Pathfinding>();
        search = searchBoxObj.GetComponent<TMP_InputField>();
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
        switch (floor) {
            case 0:
                spriteRenderer.sprite = floor0;
                UnSelectedColor(currentFloorButton);
                currentFloorButton = floorButton0;
                SelectedColor(floorButton0);
                floorText.text = "Astra/Silva 0th floor";
                break;
            case 1:
                spriteRenderer.sprite = floor1;
                UnSelectedColor(currentFloorButton);
                currentFloorButton = floorButton1;
                SelectedColor(floorButton1);
                floorText.text = "Astra/Silva 1th floor";
                break;
            case 2:
                spriteRenderer.sprite = floor2;
                UnSelectedColor(currentFloorButton);
                currentFloorButton = floorButton2;
                SelectedColor(floorButton2);
                floorText.text = "Astra/Silva 2nd floor";
                break;
            case 3:
                spriteRenderer.sprite = floor3;
                UnSelectedColor(currentFloorButton);
                currentFloorButton = floorButton3;
                SelectedColor(floorButton3);
                floorText.text = "Astra/Silva 3rd floor";
                break;
            case 4:
                spriteRenderer.sprite = floor4;
                UnSelectedColor(currentFloorButton);
                currentFloorButton = floorButton4;
                SelectedColor(floorButton4);
                floorText.text = "Astra/Silva 4th floor";
                break;
            case 5:
                spriteRenderer.sprite = floor5;
                UnSelectedColor(currentFloorButton);
                currentFloorButton = floorButton5;
                SelectedColor(floorButton5);
                floorText.text = "Astra/Silva 5th floor";
                break;
        }
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
