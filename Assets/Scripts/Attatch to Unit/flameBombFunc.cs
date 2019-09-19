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
    }
	// Update is called once per frame
	void Update () {
		if (flameBombSelected)
        {
            bcm = tileMap.GetComponent<BoardCameraMovement>();
            foreach (GameObject unit in bcm.units)
            {
                if (unit.name == "Wizard")
                {
                    unit.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                coordinate = new Vector3Int(bcm.coordinate.x,bcm.coordinate.y,0);
                if (bcm.targetTile != null)
                {
                    foreach (BoardCameraMovement.boardTile bTile in bcm.allTiles)
                    {
                        if (bTile.pos == coordinate)
                        {
                            if (bTile.inhabitant == null || bTile.inhabitant.name != "Wizard")
                            {
                                int endPoint = bcm.turnCount + 18;
                                foreach (BoardCameraMovement.burningTile burningTile in bcm.burningTiles)
                                {
                                    if (burningTile.bTile.pos == coordinate)
                                    {
                                        bcm.removeBurningTile(burningTile);
                                        break;
                                    }
                                }
                                bcm.newBurnTile(coordinate, endPoint);
                                bcm.turnSwitch();
                                break;
                            }
                        }
                    }
                }
                flameBombSelected = false;
                foreach (GameObject unit in bcm.units)
                {
                    if (unit.name == "Wizard")
                    {
                        unit.GetComponent<SpriteRenderer>().color = unit.GetComponent<Interactable>().colour;
                    }
                }
            }
        }
	}
}
