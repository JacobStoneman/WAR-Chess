using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRingInstantiate : MonoBehaviour {

	public bool collided = false;
	public int parentNum;
	public float damage;
	// Use this for initialization
	void Start () {
		Destroy (gameObject, 2.5f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = transform.parent.position;
	}

	void OnTriggerStay2D(Collider2D col){
		if (col.tag == "Unit") {
			BasicMovement baseMove = col.GetComponent<BasicMovement> ();
			if (baseMove.playNum != parentNum && !collided) {
				print (baseMove.name + " has been hit");
				baseMove.health = baseMove.health - damage;
				collided = true;
				StartCoroutine ("cooldown");
			}
		}
	}

	IEnumerator cooldown(){
		yield return new WaitForSeconds (1);
		collided = false;
	}
}
