using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureEditor : MonoBehaviour
{
    public void DrawLine (int x1, int y1, int x2, int y2, Texture2D texture, Color color)
    {
        int w = x2 - x1;
        int h = y2 - y1;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            texture.SetPixel(x1, y1, color);
            texture.SetPixel(x1+1, y1, color);
            texture.SetPixel(x1, y1+1, color);
            texture.SetPixel(x1+1, y1+1, color);
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x1 += dx1;
                y1 += dy1;
            }
            else
            {
                x1 += dx2;
                y1 += dy2;
            }
        }
        texture.Apply();
    }

    public void DrawEllipse (int startX, int startY, int width, int height, Texture2D texture, Color color)
    {
        int hh = height * height;
        int ww = width * width;
        int hhww = hh * ww;
        int x0 = width;
        int dx = 0;

        for (int x = -width; x <= width; x++)
        {
            texture.SetPixel(startX + x, startY, color);
        }

        for (int y = 1; y <= height; y++)
        {
            int x1 = x0 - (dx - 1);
            for (; x1 > 0; x1--)
                if (x1 * x1 * hh + y * y * ww <= hhww)
                    break;
            dx = x0 - x1;
            x0 = x1;

            for (int x = -x0; x <= x0; x++)
            {
                texture.SetPixel(startX + x, startY - y, color);
                texture.SetPixel(startX + x, startY + y, color);
            }
        }
        texture.Apply();
    }
}
