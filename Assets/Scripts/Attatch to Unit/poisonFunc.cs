using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poisonFunc : MonoBehaviour {

    public List<GameObject> poisonUnits = new List<GameObject>();
    public bool poisonSelected;
    Vector3Int coordinate;

	// Update is called once per frame
	void Update () {
		if (poisonSelected)
        {
            if (Input.GetMouseButtonUp(0))
            {
                coordinate = GetComponent<Interactable>().boardCamMove.coordinate;
                foreach(GameObject unit in poisonUnits)
                {
                    UnitTileCheck UTC = unit.GetComponent<UnitTileCheck>();
                    Interactable interactable = unit.GetComponent<Interactable>();
                    if (UTC.coordinate == coordinate)
                    {
                        interactable.maxHealth = interactable.maxHealth - ((interactable.maxHealth / 100) * 20);
                        interactable.tileBuffCheck(coordinate);
                        GetComponent<Interactable>().turnSwitch();
                    }                   
                }
                poisonSelected = false;
            }
        }
	}
}
