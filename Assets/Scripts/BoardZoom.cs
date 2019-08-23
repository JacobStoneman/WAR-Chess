using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardZoom : MonoBehaviour {
    public float minSize;
    public float maxSize;
    public new GameObject camera;
    Camera cameraComp;
    public GameObject panelParent;
    public string cameraState = "in";
    public GameObject tilemap;
    BoardCameraMovement boardCamMove;
    public float zoomSpeed;
    
    // Use this for initialization
    void Start () {
        cameraComp = camera.GetComponent<Camera>();
        boardCamMove = tilemap.GetComponent<BoardCameraMovement>();
        zOut();
	}
	
	// Update is called once per frame
	void Update () {
		if (cameraState == "in" && cameraComp.orthographicSize != minSize)
        {
            if (cameraComp.orthographicSize < minSize)
            {
                cameraComp.orthographicSize = minSize;
            }
            else
            {
                camera.transform.position = new Vector3(7.69f, 5.41f,-500f);
                boardCamMove.reached = true;
                zIn();
            }
        }
        else if (cameraState == "out" && cameraComp.orthographicSize != maxSize)
        {
            if (cameraComp.orthographicSize > maxSize)
            {
                cameraComp.orthographicSize = maxSize;
            }
            else
            {
                zOut();
            }
        }
    }
    public void OnButtonClick()
    {
        if (cameraState == "in")
        {
            cameraState = "out";
        } else if (cameraState == "out")
        {
            cameraState = "in";
        }
    }
    public void zOut()
    {
        cameraComp.orthographicSize = cameraComp.orthographicSize+(zoomSpeed*Time.deltaTime);
        panelParent.gameObject.SetActive(false);
        cameraState = "out";
    }
    public void zIn()
    {       
        cameraComp.orthographicSize = cameraComp.orthographicSize - (zoomSpeed * Time.deltaTime);
        panelParent.gameObject.SetActive(true);
        cameraState = "in";      
    }
}
