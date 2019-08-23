using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class tilemapCompress : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        Tilemap tilemap = GetComponent<Tilemap>();
        tilemap.origin = new Vector3Int(0,0,0);
        tilemap.CompressBounds();
    }
}
