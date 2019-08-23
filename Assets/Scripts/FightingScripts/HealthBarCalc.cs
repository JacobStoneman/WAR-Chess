using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthBarCalc : MonoBehaviour {

    public List<GameObject> players = new List<GameObject>();
    public int playNum;
    public Slider self;
    public GameObject pUnit;
    public bool wait;

    // Use this for initialization
    void OnEnable()
    {
        StartCoroutine("waitForBM");       
    }

    void OnDisable()
    {
        players.Clear();
    }

    IEnumerator waitForBM()
    {
        yield return new WaitForSeconds(0.1f);
        print("Called");
        players.AddRange(GameObject.FindGameObjectsWithTag("Unit"));
        foreach (GameObject player in players)
        {
            BasicMovement baseMove = player.GetComponent<BasicMovement>();
            if (baseMove.playNum == playNum)
            {
                pUnit = player;
                self.value = baseMove.healthPercentage;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (pUnit != null) {
			self.value = pUnit.GetComponent<BasicMovement> ().healthPercentage;
		} else {
			self.value = 0;
		}
	}
}
