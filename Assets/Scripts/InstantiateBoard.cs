using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateBoard : MonoBehaviour {

    public GameObject board;
    GameObject newBoard;
	// Use this for initialization
	void Start () {
        newBoard = Instantiate(board);
        newBoard.transform.SetParent(gameObject.transform);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
