using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireBoltFunc : MonoBehaviour {

    public List<GameObject> fireBoltUnits = new List<GameObject>();
    public bool fireBoltSelected;
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
                        interactable.health = interactable.health - 80;
                        GetComponent<Interactable>().turnSwitch();
                    }
                }
                fireBoltSelected = false;
            }
        }
    }
}
