using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamBuffFunc : MonoBehaviour {

    public List<GameObject> buffableUnits = new List<GameObject>();
    public bool buffEnabled;
    public bool buffExists;
    public bool buffApplied;
    public GameObject board;
    public Interactable interactable;
    public BoardCameraMovement bcm;
    public float buffVal;
    public int endPoint;

    // Use this for initialization
    void Start () {
        interactable = GetComponent<Interactable>();
        board = GameObject.Find("Tilemap");
        bcm = board.GetComponent<BoardCameraMovement>();
	}
	
	// Update is called once per frame
	void Update () {
		if (buffEnabled && !buffApplied)
        {
            endPoint = bcm.turnCount + 6;
            buffEnabled = false;
            buffExists = true;
        }
        if (buffExists)
        {
            if (bcm.turnCount <= endPoint && !buffApplied)
            {
                buffApplied = true;
                foreach(GameObject unit in buffableUnits)
                {
                    interactable = unit.GetComponent<Interactable>();
                    interactable.teamBuffed = true;
                    interactable.tileBuffCheck(interactable.coord);
                    interactable.setPRating();
                }
            } else if (bcm.turnCount > endPoint)
            {
                buffApplied = false;
                buffExists = false;
                foreach (GameObject unit in buffableUnits)
                {
                    interactable = unit.GetComponent<Interactable>();
                    interactable.teamBuffed = false;
                    interactable.tileBuffCheck(interactable.coord);
                    interactable.setPRating();
                }
            }
        }
	}
}
