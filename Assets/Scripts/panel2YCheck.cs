using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class panel2YCheck : MonoBehaviour {

    public GameObject panel1, left, right;
    public float speed = 8f;
    public bool currPosLeft;
    // Use this for initialization
    void Start () {
        StartCoroutine(AnimatePanelIn());
        currPosLeft = true;
    }
    IEnumerator AnimatePanelIn()
    {
        transform.localScale = Vector3.zero;
        float timer = 0f;
        while (timer < (1 / speed))
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.one * timer * speed;
            yield return null;
        }
        transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    public void checkPos () {
        if (panel1.activeInHierarchy)
        {
            if (panel1.GetComponent<panelYCheck>().isLeft)
            {
                transform.position = right.transform.position;
                if (currPosLeft)
                {
                    StartCoroutine(AnimatePanelIn());
                }
                currPosLeft = false;
            } else
            {
                transform.position = left.transform.position;
                if (!currPosLeft)
                {
                    StartCoroutine(AnimatePanelIn());
                }
                currPosLeft = true;
            }
        } else
        {
            transform.position = left.transform.position;
            if (!currPosLeft)
            {
                StartCoroutine(AnimatePanelIn());
            }
            currPosLeft = true;
        }
	}
}
