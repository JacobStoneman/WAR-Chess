using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeInstantiate : MonoBehaviour {

	public bool facingLeft;
	SpriteRenderer spriteRen;
	public float speed = 10;
	public float minX;
	public float maxX;
	public int parentNum;
	public float damage;

	// Use this for initialization
	void Start () {
		spriteRen = GetComponent<SpriteRenderer> ();
		spriteRen.flipX = facingLeft;
	}
	
	// Update is called once per frame
	void Update () {
		if (facingLeft) {	
			Vector3 newPos = new Vector3 (transform.position.x - (speed * Time.deltaTime), transform.position.y, transform.position.z);
			transform.position = newPos;
		} else {
			Vector3 newPos = new Vector3 (transform.position.x + (speed * Time.deltaTime), transform.position.y, transform.position.z);
			transform.position = newPos;
		}
		if (transform.position.x < minX || transform.position.x > maxX) {
			Destroy (gameObject,0f);
		}
		transform.Rotate (Vector3.back * (speed *Time.deltaTime * 30));
	}
	void OnCollisionEnter2D(Collision2D col)
	{
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		BasicMovement baseMove = col.GetComponent<BasicMovement>();
		if (baseMove.playNum != parentNum){
			print(baseMove.name + " has been hit");
			baseMove.health = baseMove.health - damage;
			Destroy (gameObject,0f);
		}
	}
}
