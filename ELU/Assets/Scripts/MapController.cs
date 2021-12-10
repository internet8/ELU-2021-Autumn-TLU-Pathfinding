using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    Vector3 touchStart;
    public float zoomOutMin = 100;
    public float zoomOutMax = 1080;
    public float scrollSpeed = 10;
    public GameObject scriptsObj;
    private UI ui;
    public bool controllerEnabled = true;

    void Start() {
        ui = scriptsObj.GetComponent<UI>();
    }

    void Update() {
        if (!controllerEnabled) {
            return;
        }
        if (Input.GetMouseButtonDown(0)) {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // detect if danger icon
            CastRay(new Vector2(touchStart.x, touchStart.y));
        }
        if (Input.touchCount == 2) {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * 0.002f * scrollSpeed);
        } else if (Input.GetMouseButton(0)) {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if ((Camera.main.transform.position + direction).x < 1600 / Remap(500, 2160, 2, 1, Camera.main.orthographicSize) && (Camera.main.transform.position + direction).x > -1600 / Remap(500, 2160, 2, 1, Camera.main.orthographicSize) && (Camera.main.transform.position + direction).y < 1600 / Remap(500, 2160, 2, 1, Camera.main.orthographicSize) && (Camera.main.transform.position + direction).y > -1600 / Remap(500, 2160, 2, 1, Camera.main.orthographicSize)) {
                Camera.main.transform.position += direction;
            }
        }
        Zoom(Input.GetAxis("Mouse ScrollWheel") * scrollSpeed);
    }

    void Zoom (float increment) {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }

    bool OnMapArea () {
        return false;
    }

    void CastRay (Vector2 touchPosWorld2D) {
        RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);
        if (hitInformation.collider != null) {
            GameObject touchedObject = hitInformation.transform.gameObject;
            //Debug.Log(touchedObject.transform.name);
            ui.ShowMessage(touchedObject.transform.name);
            ui.displayingIconText = true;
        }
    }

    public float Remap (float OldMin, float OldMax, float NewMin, float NewMax, float OldValue){
 
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
    
        return(NewValue);
    }
}
