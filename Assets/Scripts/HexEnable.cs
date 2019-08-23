﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class HexEnable : MonoBehaviour {

    public GameObject child;
    public GameObject tilemap;

	// Update is called once per frame
	void Update () {
        if (SceneManager.GetActiveScene().name == "HexBoard")
        {
            child.SetActive(true);
        } else
        {
            tilemap.GetComponent<BoardCameraMovement>().blockedTiles.Clear();
            tilemap.GetComponent<BoardCameraMovement>().wizardUnits.Clear();
            tilemap.GetComponent<BoardCameraMovement>().alienUnits.Clear();
            tilemap.GetComponent<BoardCameraMovement>().robotUnits.Clear();
            child.SetActive(false);
        }
	}
}
