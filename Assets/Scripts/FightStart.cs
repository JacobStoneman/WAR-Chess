using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class FightStart : MonoBehaviour {

    public GameObject child;
	// Update is called once per frame
	void Update () {
		if(SceneManager.GetActiveScene().name == "Arena")
        {
            child.SetActive(true);
        } else
        {
            child.SetActive(false);
        }
	}
}
