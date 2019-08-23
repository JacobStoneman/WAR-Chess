using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CheckDead : MonoBehaviour {

    public bool wizard;
    public bool robot;
    public bool alien;
    public Tilemap board;
    BoardCameraMovement BCM;

    void Start()
    {
        BCM = board.GetComponent<BoardCameraMovement>();
    }

    void OnDestroy()
    {
        if (wizard)
        {
            BCM.wDestroy = true;
        } else if (alien)
        {
            BCM.aDestroy = true;
        } else if (robot)
        {
            BCM.rDestroy = true;
        }
        BCM.lossCount++;
        BCM.checkTurn();
    }
}
