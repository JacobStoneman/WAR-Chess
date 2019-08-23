using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBoardMover : MonoBehaviour {

    public Vector3 minCameraPos;
    public Vector3 maxCameraPos;
    public new GameObject camera;
    BoardCameraMovement boardCam;
    public int speed = 5;
    GameObject top;
	// Use this for initialization
	void Start () {
        boardCam = GameObject.Find("Grid").GetComponentInChildren<BoardCameraMovement>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void up() {
        Vector3 pos = camera.transform.position;
        pos.y += speed * Time.deltaTime;
        pos = new Vector3(Mathf.Clamp(pos.x,minCameraPos.x,maxCameraPos.x), Mathf.Clamp(pos.y, minCameraPos.y, maxCameraPos.y), pos.z);
        camera.transform.position = pos;
    }
    void down()
    {
        Vector3 pos = camera.transform.position;
        pos.y -= speed * Time.deltaTime;
        pos = new Vector3(Mathf.Clamp(pos.x, minCameraPos.x, maxCameraPos.x), Mathf.Clamp(pos.y, minCameraPos.y, maxCameraPos.y), pos.z);
        camera.transform.position = new Vector3(pos.x, Mathf.Clamp(pos.y, minCameraPos.y, maxCameraPos.y), pos.z);
    }
    void left()
    {
        Vector3 pos = camera.transform.position;
        pos.x -= speed * Time.deltaTime;
        pos = new Vector3(Mathf.Clamp(pos.x, minCameraPos.x, maxCameraPos.x), Mathf.Clamp(pos.y, minCameraPos.y, maxCameraPos.y), pos.z);
        camera.transform.position = pos;
    }
    void right()
    {
        Vector3 pos = camera.transform.position;
        pos.x += speed * Time.deltaTime;
        pos = new Vector3(Mathf.Clamp(pos.x, minCameraPos.x, maxCameraPos.x), Mathf.Clamp(pos.y, minCameraPos.y, maxCameraPos.y), pos.z);
        camera.transform.position = pos;
    }


    void OnMouseOver()
    {
        if (boardCam.reached)
        {
            switch (gameObject.name) { 
                case "Top":
                    up();
                    break;
                case "Down":
                    down();
                    break;
                case "Left":
                    left();
                    break;
                case "Right":
                    right();
                    break;
                case "TopLeft":
                    up();
                    left();
                    break;
                case "TopRight":
                    up();
                    right();
                    break;
                case "DownLeft":
                    down();
                    left();
                    break;
                case "DownRight":
                    down();
                    right();
                    break;
                default:
                    break;
            }
        }
    }
}
