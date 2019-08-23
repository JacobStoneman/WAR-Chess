using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class panelYCheck : MonoBehaviour {

    float midPoint;
    public GameObject left, right, panel2;
    public float speed = 8f;
    public Sprite tileBuff, fire, enrage, tank;
    public Image bi1, bi2, bi3;
    public Text unitName, attack, range, abilityName, abilityText;
    public bool isLeft;

	// Use this for initialization
	void Start () {
        transform.position = new Vector3(midPoint, transform.position.y);
	}

	void OnEnable()
    {
        midPoint = Screen.width / 2;
        panel2.GetComponent<panel2YCheck>().checkPos();
        StartCoroutine(AnimatePanelIn());
    }

    void OnDisable()
    {
        panel2.GetComponent<panel2YCheck>().checkPos();
    }
	// Update is called once per frame
	void Update () {
        if (Input.mousePosition.x <= midPoint-75)
        {
            transform.position = right.transform.position;
            isLeft = false;
            panel2.GetComponent<panel2YCheck>().checkPos();
        }
        else if (Input.mousePosition.x > midPoint+75)
        {
            transform.position = left.transform.position;
            isLeft = true;
            panel2.GetComponent<panel2YCheck>().checkPos();
        }
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
}