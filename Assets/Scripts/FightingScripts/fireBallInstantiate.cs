using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireBallInstantiate : MonoBehaviour {

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
			transform.Translate (-speed * Time.deltaTime, 0, 0);
		} else {
			transform.Translate (speed * Time.deltaTime, 0, 0);
		}
		if (transform.position.x < minX || transform.position.x > maxX) {
			Destroy (gameObject,0f);
		}
			
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
