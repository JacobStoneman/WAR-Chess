using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealFunc : MonoBehaviour {

    public List<GameObject> healableUnits = new List<GameObject>();
    public bool healSelected;
    public float healVal;
    Vector3Int coordinate;
	
	// Update is called once per frame
	void Update () {
        healVal = GetComponent<Interactable>().damage * 3;
        if (healSelected)
        {
            if (Input.GetMouseButtonUp(0))
            {
                coordinate = GetComponent<Interactable>().boardCamMove.coordinate;
                foreach(GameObject unit in healableUnits)
                {
                    UnitTileCheck UTC = unit.GetComponent<UnitTileCheck>();
                    Interactable interactable = unit.GetComponent<Interactable>();
                    if (UTC.coordinate == coordinate)
                    {
                        interactable.health = interactable.health + healVal;
                        if (interactable.health > interactable.maxHealth)
                        {
                            interactable.health = interactable.maxHealth;
                        }
                        GetComponent<Interactable>().turnSwitch();
                    }
                }
                healSelected = false;
            }
        }
	}
}
