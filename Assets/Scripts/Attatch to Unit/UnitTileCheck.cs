using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class UnitTileCheck : MonoBehaviour {

    public Vector3Int coordinate;
    public Grid grid; //Grid tilemap is attached to
    SpriteRenderer spriteRen;

    void Awake()
    {
        coordinate = grid.WorldToCell(transform.position);
    }

    // Use this for initialization
    void Start () {
        spriteRen = GetComponent<SpriteRenderer>();
	}

    // Update is called once per frame
    void Update() {
        coordinate = grid.WorldToCell(transform.position);
        coordinate.z = -500;
        if(coordinate.x < 8)
        {
            spriteRen.flipX = false;   
        } else if (coordinate.x > 8)
        {
            spriteRen.flipX = true;
        }
    }
}
