using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogInfo : MonoBehaviour
{
    public GameObject attacker, target;
    public BoardCameraMovement.boardTile bTile;
    public string action;
    void OnMouseOver()
    {
        if(attacker != null)
        {
            attacker.GetComponent<SpriteRenderer>().color = Color.black;
        }
        if (target != null)
        {
            target.GetComponent<SpriteRenderer>().color = Color.black;
        }
    }
    void OnMouseExit()
    {
        if (attacker != null)
        {
            attacker.GetComponent<SpriteRenderer>().color = attacker.GetComponent<Interactable>().colour;
        }
        if (target != null)
        {
            target.GetComponent<SpriteRenderer>().color = target.GetComponent<Interactable>().colour;
        }
    }
}
