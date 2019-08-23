using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSizeCheck : MonoBehaviour {

    public float maxWidth, maxHeight;
    RectTransform rect;
    // Use this for initialization
    void Start () {
        rect = GetComponent<RectTransform>();
        maxWidth = rect.rect.width;
        maxHeight = rect.rect.height;
    }
    public void checkImage(Color col)
    {
        rect = GetComponent<RectTransform>();
        GetComponent<Image>().color = col;
        if (rect.rect.width > maxWidth)
        {
            rect.sizeDelta = new Vector2(maxWidth,rect.sizeDelta.y);
        }
        if (rect.rect.height > maxHeight)
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, maxHeight);
        }
    }
}
