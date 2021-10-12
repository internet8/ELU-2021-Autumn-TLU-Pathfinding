using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public float dragSpeed = 40f;
    public float scrollSpeed = 10f;

    void OnMouseDrag()
    {
        gameObject.transform.position += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) * dragSpeed;
    }

    void Update() {
        var d = Input.GetAxis("Mouse ScrollWheel");
        gameObject.transform.localScale += new Vector3(d, d, 0) * scrollSpeed;
        if (gameObject.transform.localScale.x < 0.5) {
            gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }
}
