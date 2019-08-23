using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        SceneManager.LoadScene("Arena", LoadSceneMode.Additive);
	}
}
