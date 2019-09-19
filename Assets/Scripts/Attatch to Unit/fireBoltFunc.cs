using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireBoltFunc : MonoBehaviour {

    public List<GameObject> fireBoltUnits = new List<GameObject>();
    public bool fireBoltSelected;
    public int damage = 65;
    Vector3Int coordinate;

	// Update is called once per frame
	void Update () {
        if (fireBoltSelected)
        {
            if (Input.GetMouseButtonUp(0))
            {
                coordinate = GetComponent<Interactable>().boardCamMove.coordinate;
                foreach (GameObject unit in fireBoltUnits)
                {
                    UnitTileCheck UTC = unit.GetComponent<UnitTileCheck>();
                    Interactable interactable = unit.GetComponent<Interactable>();
                    if (UTC.coordinate == coordinate)
                    {
                        BoardCameraMovement.boardTile cTile = null;
                        foreach(BoardCameraMovement.boardTile bTile in interactable.boardCamMove.allTiles)
                        {
                            if (coordinate == bTile.pos)
                            {
                                cTile = bTile;
                                break;
                            }
                        }
                        interactable.health = interactable.health - damage;
                        interactable.boardCamMove.newLogEntry(cTile, "firebolt", unit, gameObject);
                        GetComponent<Interactable>().boardCamMove.turnSwitch();
                    }
                    unit.GetComponent<Interactable>().unitSelected = false;
                }
                fireBoltSelected = false;
            }
        }
    }
}
