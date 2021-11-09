using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller360 : MonoBehaviour
{
    public Transform container;
    public float turnSpeed = 1;
    float horizontal;
    float vertical;
    Vector2 startPos, direction;

    void Update () 
    {
        if (Input.GetMouseButton(0)) {
            horizontal = Input.GetAxis("Mouse X");
            vertical = Input.GetAxis("Mouse Y");

            container.Rotate(new Vector3(0, horizontal * (-1), 0f) * Time.deltaTime * turnSpeed);
        }
        if (Input.touchCount == 1) {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    direction = touch.position - startPos;
                    Vector3 wordPos = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(direction.x, 0, GetComponent<Camera>().nearClipPlane));
                    container.Rotate(new Vector3(0, wordPos.y * (-1), 0f) * Time.deltaTime * turnSpeed);
                    break;
                case TouchPhase.Ended:
                    break;
            }
        }
    }
}
