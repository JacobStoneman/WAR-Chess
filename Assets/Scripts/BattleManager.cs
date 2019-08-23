using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class BattleManager : MonoBehaviour {

    public bool bothAlive = true;
    public GameObject player1;
    public GameObject player2;

    // Use this for initialization
    void OnEnable () {
        print("Attacker: " + StaticDataScript.player1 + " at " + StaticDataScript.player1Health + " health and on " + StaticDataScript.player1Faction + " team");
        print("Defender: " + StaticDataScript.player2 + " at " + StaticDataScript.player2Health + " health and on " + StaticDataScript.player2Faction + " team");
        player1 = Instantiate(Resources.Load(StaticDataScript.player1), new Vector3(-2.78f, -0.39f, 0), transform.rotation) as GameObject;
        player1.GetComponent<BasicMovement>().playNum = 1;
        player1.GetComponent<BasicMovement>().maxHealth = StaticDataScript.player1MaxHealth;
        player1.GetComponent<BasicMovement>().health = StaticDataScript.player1Health;
        player1.GetComponent<BasicMovement>().damage = StaticDataScript.player1Damage;
        player1.GetComponent<SpriteRenderer>().color = StaticDataScript.player1Colour;
        player1.name = player1.name.Replace("(Clone)", "");
        player2 = Instantiate(Resources.Load(StaticDataScript.player2), new Vector3(3.79f, -0.39f, 0), transform.rotation) as GameObject;
        player2.GetComponent<BasicMovement>().playNum = 2;
        player2.GetComponent<BasicMovement>().maxHealth = StaticDataScript.player2MaxHealth;
        player2.GetComponent<BasicMovement>().health = StaticDataScript.player2Health;
        player2.GetComponent<BasicMovement>().damage = StaticDataScript.player2Damage;
        player2.GetComponent<SpriteRenderer>().color = StaticDataScript.player2Colour;
        player2.name = player2.name.Replace("(Clone)", "");
    }
	
	// Update is called once per frame
	void Update () {
		if (!bothAlive)
        {
            print("Fight Over");
            if (player1.GetComponent<BasicMovement>().dead)
            {
                StaticDataScript.factionWin = StaticDataScript.player2Faction;
                StaticDataScript.winHealth = player2.GetComponent<BasicMovement>().health;
                StaticDataScript.fighterNum = player1.GetComponent<BasicMovement>().playNum;
            } else if (player2.GetComponent<BasicMovement>().dead)
            {
                StaticDataScript.factionWin = StaticDataScript.player1Faction;
                StaticDataScript.winHealth = player1.GetComponent<BasicMovement>().health;
                StaticDataScript.fighterNum = player2.GetComponent<BasicMovement>().playNum;
            }
            Destroy(player1);
            Destroy(player2);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("HexBoard"));
            bothAlive = true;
        }
	}
}
