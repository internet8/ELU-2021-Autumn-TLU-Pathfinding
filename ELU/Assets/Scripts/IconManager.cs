using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconManager : MonoBehaviour
{
    public List<Sprite> pathSprites = new List<Sprite>();

    public void CreateIcon (Vector2 position, Sprite sprite) {
        GameObject go = new GameObject("New Icon");
        go.transform.position = position;
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
    }
}
