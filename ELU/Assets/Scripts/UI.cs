using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    //public Sprite floor0, floor1, floor2, floor3, floor4, floor5;
    public Sprite[] floors = new Sprite[6];
    public Vector3[] floorPositions = new Vector3[6];
    public Button shortestButton, noObstaclesButton;
    public Button[] floorButtons = new Button[6];
    public string[] floorLabels = { "ground", "1st", "2nd", "3rd", "4th", "5th" };
    public GameObject pathFrom, pathTo;
    private Button currentFloorButton;
    private SpriteRenderer spriteRenderer;
    public GameObject floorObj;
    public GameObject searchResultObj, searchBoxObj;
    public GameObject searchCanvas, mapCanvas, searchBox, exitPrompt;
    private TMP_InputField search;
    public TMP_Text fromText, toText, floorText, messageText;
    private Pathfinding pathfinding;
    private int currentDisplayFloor = 1;
    private int fromIndex = -1;
    private int toIndex = -1;
    private int fromFloor = -1;
    private int toFloor = -1;
    public int currentFloor = 1;
    private string searchLastValue = "";
    private bool fromSearch = true;
    public bool showPath = false;
    public float pathOneLen, pathTwoLen;
    public Material lineMaterial;
    private float tilingFactor = 0.02f;
    private IconManager iconManager;
    private MapController mapController;
    private bool findRestroom = false;
    // text box bubbles
    private bool canSwitch = false;
    private bool singleFloorText = false;
    public bool displayingIconText = false;
    private string activeMessageHolder = "";
    private string hiddenMessageHolder = "";
    private string hiddenMessageSwitchHolder = "";
    private bool onSwitchBubble = false;
    public GameObject bubbleFrom, bubbleTo, bubbleSwitch;
    private int messageBoxIndex = 0;

    void Start()
    {
        currentFloorButton = floorButtons[1];
        spriteRenderer = floorObj.GetComponent<SpriteRenderer>();
        pathfinding = gameObject.GetComponent<Pathfinding>();
        search = searchBoxObj.GetComponent<TMP_InputField>();
        iconManager = gameObject.GetComponent<IconManager>();
        mapController = Camera.main.GetComponent<MapController>();
        SetToShortest();
    }

    void Update () {
        if (!search.text.Equals(searchLastValue)) {
            searchLastValue = search.text;
            Search(fromSearch);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (searchCanvas.activeSelf) {
                CloseSearch();
            } else if (searchBox.activeSelf) {
                CloseSearchBox();
            } else if (exitPrompt.activeSelf) {
                CloseExitPrompt();
            } else {
                OpenExitPrompt();
            }
        }
    }

    public void ShowPath () {
        CloseSearchBox();
        if (findRestroom && fromIndex > -1) {
            toFloor = pathfinding.GetClosestRestroomFloor(fromFloor);
            toIndex = pathfinding.GetClosestRestroom(fromFloor, toFloor, fromIndex);
            pathfinding.FindPath(fromFloor, toFloor, fromIndex, toIndex);
            searchCanvas.SetActive(false);
            mapCanvas.SetActive(true);
        } else if (fromIndex > -1 && toIndex > -1) {
            pathfinding.FindPath(fromFloor, toFloor, fromIndex, toIndex);
            searchCanvas.SetActive(false);
            mapCanvas.SetActive(true);
        } else if (fromIndex > -1) {
            Search(fromSearch);
        } else {
            ShowMessage("Some fields are empty!");
        }
    }

    public void Search (bool b) {
        DeleteSearchResults();
        string input = search.text;
        int index = 0;
        int floorIndex = 0;
        bool restroom = false;
        if (input.Length <= 2) {
            if (("WC").Substring(0, input.Length).ToLower().Equals(input.ToLower()) && !b) {
                restroom = true;
            }
        }
        if (input.Length <= 8) {
            if (("Restroom").Substring(0, input.Length).ToLower().Equals(input.ToLower()) && !b) {
                restroom = true;
            }
        }
        foreach (Vertex[] vertices in pathfinding.graphs) {
            foreach (Vertex v in vertices)
            {
                if (v.type.Length >= input.Length) {
                    if ((v.type.Substring(0, input.Length).ToLower().Equals(input.ToLower()) && v.room) || restroom) {
                        var button = (GameObject)Instantiate(Resources.Load("SearchResultButton", typeof(GameObject))) as GameObject;
                        if (button == null) continue;
                        button.transform.SetParent (searchResultObj.transform);
                        button.transform.localScale = Vector3.one;
                        button.transform.localRotation = Quaternion.Euler (Vector3.zero);
                        button.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(index, index, 0);
                        if (restroom) {
                            button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Closest Restroom";
                        } else {
                            button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = v.type;
                        }
                        v.floorIndex = floorIndex;
                        if (restroom) {
                            button.GetComponent<Button>().onClick.AddListener(delegate { 
                                toText.text = "Closest Restroom";
                                findRestroom = true;
                                toFloor = -1;
                                toIndex = -1;
                                CloseSearch();
                                DeleteSearchResults();
                            });
                        } else {
                            button.GetComponent<Button>().onClick.AddListener(delegate { 
                                if (b) {
                                    fromFloor = v.floorIndex;
                                    fromIndex = v.index;
                                    fromText.text = v.type;
                                } else {
                                    findRestroom = false;
                                    toFloor = v.floorIndex;
                                    toIndex = v.index;
                                    toText.text = v.type;
                                }
                                CloseSearch();
                                DeleteSearchResults();
                            });
                        }
                        index++;
                        restroom = false;
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
        mapController.controllerEnabled = true;
    }

    public void OpenExitPrompt () {
        exitPrompt.SetActive(true);
    }

    public void CloseExitPrompt () {
        exitPrompt.SetActive(false);
    }

    public void CloseApplication () {
        Application.Quit();
    }

    public void OpenSearch (bool b) {
        fromSearch = b;
        searchCanvas.SetActive(true);
        mapCanvas.SetActive(false);
        searchBox.SetActive(false);
        search.Select();
        search.ActivateInputField();
        mapController.controllerEnabled = false;
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

    public void MainBackButton () {
        if (searchCanvas.activeSelf) {
            CloseSearch();
        } else if (searchBox.activeSelf) {
            CloseSearchBox();
        } else if (exitPrompt.activeSelf) {
            CloseExitPrompt();
        } else if (showPath) {
            pathfinding.RestoreDefaultTextures();
        } else {
            OpenExitPrompt();
        }
    }

    // map switching
    public void OnClicked(Button button)
    {
        print(button.name);
    }

    public void SetToShortest () {
        if (!pathfinding.shortestPath) {
            noObstaclesButton.GetComponent<Image>().enabled = false;
            noObstaclesButton.transform.GetChild(0).GetComponent<TMP_Text>().fontStyle = FontStyles.Normal;
            noObstaclesButton.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.black;
            shortestButton.GetComponent<Image>().enabled = true;
            shortestButton.transform.GetChild(0).GetComponent<TMP_Text>().fontStyle = FontStyles.Bold;
            shortestButton.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.white;
            pathfinding.shortestPath = true;
        }
    }

    public void SetToNoObstacles () {
        if (pathfinding.shortestPath) {
            noObstaclesButton.GetComponent<Image>().enabled = true;
            noObstaclesButton.transform.GetChild(0).GetComponent<TMP_Text>().fontStyle = FontStyles.Bold;
            noObstaclesButton.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.white;
            shortestButton.GetComponent<Image>().enabled = false;
            shortestButton.transform.GetChild(0).GetComponent<TMP_Text>().fontStyle = FontStyles.Normal;
            shortestButton.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.black;
            pathfinding.shortestPath = false;
        }
    }

    public void SwitchMap (int floor) {
        currentDisplayFloor = floor;
        pathFrom.SetActive(false);
        pathTo.SetActive(false);
        if (fromFloor == floor && showPath) {
            pathTo.SetActive(true);
            int index = onSwitchBubble ? 1 : 0;
            Vector3 camPos = pathTo.gameObject.transform.GetChild(index).position;
            Camera.main.transform.position = new Vector3(camPos.x, camPos.y, -1);
            Camera.main.orthographicSize = mapController.zoomOutMin;
            lineMaterial.SetFloat("_Tiling", pathTwoLen * tilingFactor);
        } else if (toFloor == floor && showPath) {
            pathFrom.SetActive(true);
            Vector3 camPos = pathFrom.gameObject.transform.GetChild(0).position;
            Camera.main.transform.position = new Vector3(camPos.x, camPos.y, -1);
            Camera.main.orthographicSize = mapController.zoomOutMin;
            lineMaterial.SetFloat("_Tiling", pathOneLen * tilingFactor);
        }
        spriteRenderer.sprite = floors[floor];
        floorObj.transform.position = floorPositions[floor];
        UnSelectedColor(currentFloorButton);
        currentFloorButton = floorButtons[floor];
        SelectedColor(floorButtons[floor]);
        floorText.text = "Astra/Silva " + floorLabels[floor] + " floor";
        currentFloor = floor;
        // caution icons
        iconManager.DeleteCautionIcons();
        iconManager.GenerateCautionIcons(pathfinding.graphs[floor]);
    }

    private void SelectedColor (Button button) {
        ColorBlock cb = button.colors;
        Color tlu = new Color(0.8157f, 0.0157f, 0.235f);
        cb.normalColor = tlu;
        cb.highlightedColor = tlu;
        cb.pressedColor = tlu;
        cb.selectedColor = tlu;
        button.colors = cb;
        button.gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color = Color.white;
    }

    private void UnSelectedColor (Button button) {
        ColorBlock cb = button.colors;
        Color tlu = new Color(0.8157f, 0.0157f, 0.235f);
        cb.normalColor = Color.white;
        cb.highlightedColor = Color.white;
        cb.pressedColor = Color.white;
        cb.selectedColor = Color.white;
        button.colors = cb;
        button.gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color = tlu;
    }

    public void SwitchDirections () {
        if (findRestroom || (fromIndex == -1 && toIndex == -1)) {
            return;
        }
        int helperFloor = fromFloor;
        fromFloor = toFloor;
        toFloor = helperFloor;
        int helperIndex = fromIndex;
        fromIndex = toIndex;
        toIndex = helperIndex;
        string helperText = fromText.text;
        fromText.text = toText.text;
        toText.text = helperText;
    }

    public void ShowMessage (string text1, string text2 = "") {
        ResetUI();
        Color active = new Color(0, 0, 0, 0.6f);
        Color inActive = new Color(0, 0, 0, 0.2f);
        activeMessageHolder = text1;
        messageText.transform.parent.gameObject.SetActive(true);
        bubbleFrom.SetActive(false);
        bubbleSwitch.SetActive(true);
        bubbleSwitch.GetComponent<Image>().color = active;
        bubbleTo.SetActive(false);
        if (text2.Length > 0) {
            activeMessageHolder = "";
            canSwitch = true;
            hiddenMessageHolder = text2;
            string[] sentences = text1.Split('.');
            hiddenMessageSwitchHolder = sentences[sentences.Length - 2] + ".";
            foreach (string s in sentences) {
                if (hiddenMessageSwitchHolder.Equals(s + ".")) {
                    break;
                }
                activeMessageHolder += s + ".";
            }
            bubbleFrom.SetActive(true);
            bubbleSwitch.SetActive(true);
            bubbleTo.SetActive(true);
            bubbleFrom.GetComponent<Image>().color = active;
            bubbleSwitch.GetComponent<Image>().color = inActive;
            bubbleTo.GetComponent<Image>().color = inActive;
        } else {
            if (showPath) {
                singleFloorText = true;
            }
        }
        messageText.text = activeMessageHolder;
    }

    public void HideMessage () {
        Color active = new Color(0, 0, 0, 0.6f);
        Color inActive = new Color(0, 0, 0, 0.2f);
        if (canSwitch) {
            bubbleSwitch.SetActive(true);
            bubbleTo.SetActive(true);
            bubbleFrom.SetActive(true);
            if (messageBoxIndex == 0) {
                messageBoxIndex++;
                messageText.text = hiddenMessageSwitchHolder;
                bubbleFrom.GetComponent<Image>().color = inActive;
                bubbleSwitch.GetComponent<Image>().color = active;
                bubbleTo.GetComponent<Image>().color = inActive;
                onSwitchBubble = true;
                SwitchMap(fromFloor);
            } else if (messageBoxIndex == 1) {
                messageBoxIndex++;
                messageText.text = hiddenMessageHolder;
                bubbleFrom.GetComponent<Image>().color = inActive;
                bubbleSwitch.GetComponent<Image>().color = inActive;
                bubbleTo.GetComponent<Image>().color = active;
                onSwitchBubble = false;
                SwitchMap(toFloor);
            } else if (messageBoxIndex == 2) {
                messageBoxIndex = 0;
                messageText.text = activeMessageHolder;
                bubbleFrom.GetComponent<Image>().color = active;
                bubbleSwitch.GetComponent<Image>().color = inActive;
                bubbleTo.GetComponent<Image>().color = inActive;
                SwitchMap(fromFloor);
            }
        } else if (displayingIconText && showPath) {
            int oldFloor = currentDisplayFloor;
            if (fromFloor != currentDisplayFloor) {
                SwitchMap(fromFloor);
            }
            ShowMessage(pathfinding.pathText, pathfinding.pathText2);
            if (toFloor == oldFloor) {
                HideMessage();
                HideMessage();
            }
        } else if (!singleFloorText) {
            messageText.transform.parent.gameObject.SetActive(false);
        }
    }

    public void ResetUI () {
        messageBoxIndex = 0;
        activeMessageHolder = "";
        hiddenMessageSwitchHolder = "";
        hiddenMessageHolder = "";
        canSwitch = false;
        onSwitchBubble = false;
        singleFloorText = false;
        displayingIconText = false;
    }
}
