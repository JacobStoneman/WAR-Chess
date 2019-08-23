using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enrageFunc : MonoBehaviour {

    public bool enrageEnabled;
    public bool enrageExists;
    public bool enrageApplied;
    public GameObject board;
    public int endPoint;
    public Interactable interactable;
    public BoardCameraMovement bcm;

    // Use this for initialization
    void Start () {
        interactable = GetComponent<Interactable>();
        board = GameObject.Find("Tilemap");
        bcm = board.GetComponent<BoardCameraMovement>();
    }
	
	// Update is called once per frame
	void Update () { 
		if (enrageEnabled && !enrageApplied)
        {
            endPoint = bcm.turnCount + 9;
            enrageEnabled = false;
            enrageExists = true;
            interactable.turnSwitch();
        }
        if (enrageExists)
        {
            if (bcm.turnCount <= endPoint && !enrageApplied)
            {
                enrageApplied = true;
                interactable.tileBuffCheck(interactable.coord);
            } else if(bcm.turnCount > endPoint)
            {
                enrageApplied = false;
                enrageExists = false;
                interactable.tileBuffCheck(interactable.coord);
            }
            
        }
	}
}
