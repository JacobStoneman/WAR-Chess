using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tankFunc : MonoBehaviour {

    public bool tankEnabled;
    public bool tankExists;
    public bool tankApplied;
    Interactable interactable;
    BoardCameraMovement bcm;
    public int endPoint;
    public GameObject board;

	// Use this for initialization
	void Start () {
        interactable = GetComponent<Interactable>();
        board = GameObject.Find("Tilemap");
        bcm = board.GetComponent<BoardCameraMovement>();
	}
	
	// Update is called once per frame
	void Update () {
		if (tankEnabled && !tankApplied)
        {
            endPoint = bcm.turnCount + 9;
            tankEnabled = false;
            tankExists = true;
            interactable.turnSwitch();
        }
        if (tankExists)
        {
            if(bcm.turnCount <= endPoint && !tankApplied)
            {
                tankApplied = true;
                interactable.health = interactable.health - (interactable.maxHealth - interactable.baseMax);
                interactable.maxHealth = interactable.baseMax;
                interactable.tileBuffCheck(interactable.coord);
            } else if(bcm.turnCount > endPoint)
            {
                tankApplied = false;
                tankExists = false;
                interactable.health = interactable.health - (interactable.maxHealth - interactable.baseMax);
                interactable.maxHealth = interactable.baseMax;
                interactable.tileBuffCheck(interactable.coord);
            }
        }
	}
}
