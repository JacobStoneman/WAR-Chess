using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntAttackInstantiate : MonoBehaviour {

	public bool facingLeft;
	public int parentNum;
	public float damage; 
	public float offset;
	Vector3 newPos;
	public float aliveTime;
	public bool collided = false;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, aliveTime);
	}
	
	// Update is called once per frame
	void Update () {
		facingLeft = GetComponentInParent<BasicMovement> ().facingLeft;
		if (facingLeft) {
			newPos = new Vector3(transform.parent.position.x - offset,transform.parent.position.y,transform.parent.position.z);
			transform.position = newPos;
		} else {
			newPos = new Vector3(transform.parent.position.x + offset,transform.parent.position.y,transform.parent.position.z);
			transform.position = newPos;
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Unit") {
			BasicMovement baseMove = col.GetComponent<BasicMovement> ();
			if (baseMove.playNum != parentNum && !collided) {
				collided = true;
				print (baseMove.name + " has been hit");
				baseMove.health = baseMove.health - damage;
				Destroy (gameObject, 0f);
			}
		}
	}
}