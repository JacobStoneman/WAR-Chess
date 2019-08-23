using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour {

	public float speed;
	Animator animator;
	SpriteRenderer spriteRend;
	public bool facingLeft;
    public bool cooldown;
	public float cooldownTime = 1;
    public GameObject attack;
    public int playNum;
	public float maxHealth;
	public float health;
	public float healthPercentage;
	public bool dead = false;
	public float damage;
    GameObject controller;
    public int fighterNum;

	void Awake(){
		healthPercentage = (health / maxHealth) * 100;
	}

	// Use this for initialization
	void Start () {
        controller = GameObject.Find("BattleManager");
		animator = GetComponent<Animator> ();
		spriteRend = GetComponent<SpriteRenderer> ();
        cooldown = true;
	}
	
	// Update is called once per frame
	void Update () {
		healthPercentage = (health / maxHealth) * 100;
		if (healthPercentage <= 0) {
			//staticVariableScript.deadUnit = gameObject;
			dead = true;
			animator.SetInteger ("animState", 3);
            controller.GetComponent<BattleManager>().bothAlive = false;

		}
		if (!dead) {
			if (Input.GetKey ("w") && playNum == 1 || Input.GetKey (KeyCode.UpArrow) && playNum == 2) {
				animator.SetInteger ("animState", 1);
				transform.Translate (0, speed * Time.deltaTime, 0);
			}
			if (Input.GetKey ("s") && playNum == 1 || Input.GetKey (KeyCode.DownArrow) && playNum == 2) {
				animator.SetInteger ("animState", 1);
				transform.Translate (0, -speed * Time.deltaTime, 0);
			}
			if (Input.GetKey ("a") && playNum == 1 || Input.GetKey (KeyCode.LeftArrow) && playNum == 2) {
				facingLeft = true;
				animator.SetInteger ("animState", 1);
				transform.Translate (-speed * Time.deltaTime, 0, 0);
				spriteRend.flipX = true;
			}
			if (Input.GetKey ("d") && playNum == 1 || Input.GetKey (KeyCode.RightArrow) && playNum == 2) {
				facingLeft = false;
				animator.SetInteger ("animState", 1);
				transform.Translate (speed * Time.deltaTime, 0, 0);
				spriteRend.flipX = false;
			} else if (!Input.GetKey ("w") && !Input.GetKey ("s") && !Input.GetKey ("d") && !Input.GetKey ("a") && playNum == 1 || !Input.GetKey (KeyCode.UpArrow) && !Input.GetKey (KeyCode.DownArrow) && !Input.GetKey (KeyCode.RightArrow) && !Input.GetKey (KeyCode.LeftArrow) && playNum == 2) {
				animator.SetInteger ("animState", 0);
			}
			if (Input.GetKey ("c") && playNum == 1 || Input.GetKey ("right shift") && playNum == 2) {
				if (cooldown) {
					switch (gameObject.name) {
					case "Wizard":
						GameObject newWizardAttack = Instantiate (attack, transform.position, transform.rotation) as GameObject;
						newWizardAttack.GetComponent<fireBallInstantiate> ().facingLeft = facingLeft;
						newWizardAttack.GetComponent<fireBallInstantiate> ().parentNum = playNum;
						newWizardAttack.GetComponent<fireBallInstantiate> ().damage = damage;
						break;
					case "Grunt":
						GameObject newGruntAttack = Instantiate (attack, transform.position, transform.rotation) as GameObject;
						newGruntAttack.transform.parent = transform;
						newGruntAttack.GetComponent<GruntAttackInstantiate> ().damage = damage;
						newGruntAttack.GetComponent<GruntAttackInstantiate> ().offset = 0.45f;
						newGruntAttack.GetComponent<GruntAttackInstantiate> ().parentNum = playNum;
						break;
					case "Berserker":
						GameObject newBerserkerAttack = Instantiate (attack, transform.position, transform.rotation) as GameObject;
						newBerserkerAttack.GetComponent<AxeInstantiate> ().facingLeft = facingLeft;
						newBerserkerAttack.GetComponent<AxeInstantiate> ().parentNum = playNum;
						newBerserkerAttack.GetComponent<AxeInstantiate> ().damage = damage;
						break;
					case "Knight":
						GameObject newKnightAttack = Instantiate (attack, transform.position, transform.rotation) as GameObject;
						newKnightAttack.transform.parent = transform;
						newKnightAttack.GetComponent<GruntAttackInstantiate> ().damage = damage;
						newKnightAttack.GetComponent<GruntAttackInstantiate> ().offset = 0.45f;
						newKnightAttack.GetComponent<GruntAttackInstantiate> ().parentNum = playNum;
						break;
					case "Cleric":
						GameObject newClericAttack = Instantiate (attack, transform.position, transform.rotation) as GameObject;
						newClericAttack.GetComponent<AxeInstantiate> ().facingLeft = facingLeft;
						newClericAttack.GetComponent<AxeInstantiate> ().parentNum = playNum;
						newClericAttack.GetComponent<AxeInstantiate> ().damage = damage;
						break;
					case "Dragon":
						GameObject newDragonAttack = Instantiate (attack, transform.position, transform.rotation) as GameObject;
						newDragonAttack.transform.parent = transform;
						newDragonAttack.GetComponent<FireRingInstantiate> ().parentNum = playNum;
						newDragonAttack.GetComponent<FireRingInstantiate> ().damage = damage;
						break;
					case "Ranger":
						GameObject newRangerAttack = Instantiate (attack, transform.position, transform.rotation) as GameObject;
						newRangerAttack.GetComponent<fireBallInstantiate> ().facingLeft = facingLeft;
						newRangerAttack.GetComponent<fireBallInstantiate> ().parentNum = playNum;
						newRangerAttack.GetComponent<fireBallInstantiate> ().damage = damage;
						break;
					case "Rogue":
						GameObject newRogueAttack = Instantiate (attack, transform.position, transform.rotation) as GameObject;
						newRogueAttack.transform.parent = transform;
						newRogueAttack.GetComponent<GruntAttackInstantiate> ().damage = damage;
						newRogueAttack.GetComponent<GruntAttackInstantiate> ().offset = 0.45f;
						newRogueAttack.GetComponent<GruntAttackInstantiate> ().parentNum = playNum;
						break;
					case "Sentinel":
						GameObject newSentinelAttack = Instantiate (attack, transform.position, transform.rotation) as GameObject;
						newSentinelAttack.transform.parent = transform;
						newSentinelAttack.GetComponent<GruntAttackInstantiate> ().damage = damage;
						newSentinelAttack.GetComponent<GruntAttackInstantiate> ().offset = 0.45f;
						newSentinelAttack.GetComponent<GruntAttackInstantiate> ().parentNum = playNum;
						break;
					default:
						break;
					}
					StartCoroutine ("Cooldown");
					animator.SetInteger ("animState", 2);
				}
			}
		}
	}
    IEnumerator Cooldown()
    {
        cooldown = false;
		yield return new WaitForSeconds(cooldownTime);
        cooldown = true;
    }
}
