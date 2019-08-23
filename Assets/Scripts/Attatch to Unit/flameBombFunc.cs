using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flameBombFunc : MonoBehaviour {

    public bool flameBombSelected;
    Vector3Int coordinate;
    GameObject tileMap;
    BoardCameraMovement bcm;

    void Start()
    {
        tileMap = GameObject.Find("Tilemap");
        bcm = tileMap.GetComponent<BoardCameraMovement>();
    }
	// Update is called once per frame
	void Update () {
		if (flameBombSelected)
        {
            if (Input.GetMouseButtonUp(0))
            {
                coordinate = new Vector3Int(bcm.coordinate.x,bcm.coordinate.y,0);
                if (bcm.targetTile != null)
                {
                    int endPoint = bcm.turnCount + 9;
                    bcm.newBurnTile(coordinate, endPoint);
                    GetComponent<Interactable>().turnSwitch();
                }
                flameBombSelected = false;
            }
        }
	}
}
