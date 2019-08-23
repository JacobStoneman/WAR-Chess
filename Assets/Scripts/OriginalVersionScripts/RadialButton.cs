using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Tilemaps;

public class RadialButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Image circle;
	public Image icon;
	public string title;
    public string func;
    public RadialMenu menu;
    Color defaultColour;
    public float speed = 8f;
    GameObject[] units;
    public GameObject boardTilemap;
    public Tilemap board;
    List <Vector3Int> tilesInRange = new List<Vector3Int>();
    List <TileBase> prevState = new List<TileBase>();
    List <Vector3Int> tempTilesInRange = new List<Vector3Int>();
    List<Vector3Int> ignoreList = new List<Vector3Int>();
    public TileBase rangeTile;
    String faction;
    BoardCameraMovement boardCamMove;

    public void Start()
    {
        boardTilemap = GameObject.Find("Tilemap");
        board = boardTilemap.GetComponent<Tilemap>();
        boardCamMove = board.GetComponent<BoardCameraMovement>();
    }

    public void Anim()
    {
        tilesInRange.Clear();
        StartCoroutine(AnimateButtonIn());
    }

    IEnumerator AnimateButtonIn()
    {
        transform.localScale = Vector3.zero;
        float timer = 0f;
        while (timer < (1 / speed))
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.one * timer * speed;
            yield return null;
        }
        transform.localScale = Vector3.one;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        menu.selected = this;
        defaultColour = circle.color;
        circle.color = Color.grey;
        boardCamMove.buttonHover = title;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        menu.selected = null;
        circle.color = defaultColour;
        boardCamMove.buttonHover = null;
    }

    void Move()
    {
        units = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject unit in units)
        {
            Interactable interactable = unit.GetComponent<Interactable>();
            UnitTileCheck tileCheck = unit.GetComponent<UnitTileCheck>();
            if (interactable.unitSelected)
            {
                faction = interactable.faction;
                int range = interactable.range;
                Vector3Int coord = tileCheck.coordinate;
                tilesInRange.Add(new Vector3Int(coord.x,coord.y,0));
                
                for (int i = 1; i <= range; i++)
                {
                    tempTilesInRange.Clear();
                    foreach(Vector3Int val in tilesInRange)
                    {
                        tempTilesInRange.Add(val);
                    }
                    foreach (Vector3Int tile in tempTilesInRange)
                    {
                        Vector3Int cellPos;
                        cellPos = new Vector3Int(tile.x, tile.y + 1, 0);
                        checkCell(cellPos);
                        if (tile.y%2 == 1)
                        {
                            cellPos = new Vector3Int(tile.x + 1, tile.y + 1, 0);
                            checkCell(cellPos);
                            cellPos = new Vector3Int(tile.x + 1, tile.y - 1, 0);
                            checkCell(cellPos);
                        } else if (tile.y%2 == 0)
                        {
                            cellPos = new Vector3Int(tile.x - 1, tile.y + 1, 0);
                            checkCell(cellPos);
                            cellPos = new Vector3Int(tile.x - 1, tile.y - 1, 0);
                            checkCell(cellPos);
                        }
                        cellPos = new Vector3Int(tile.x + 1, tile.y, 0);
                        checkCell(cellPos);
                        cellPos = new Vector3Int(tile.x, tile.y - 1, 0);
                        checkCell(cellPos);
                        cellPos = new Vector3Int(tile.x-1, tile.y, 0);
                        checkCell(cellPos);
                    }
                }
                tilesInRange.RemoveAt(0);
                foreach (Vector3Int tile in tilesInRange)
                {
                    board.SetTile(tile, rangeTile);
                }
                interactable.tileRange = tilesInRange;
                interactable.prevState = prevState;
                interactable.moveSelected = true;
                deselectUnits();
            }
        }
    }

    void checkCell(Vector3Int cellPos)
    {
        
        if (board.GetTile(cellPos) != null && !tilesInRange.Contains(cellPos))
        {
            List<GameObject> checkUnits = new List<GameObject>();
            if (faction == "wizard")
            {
                checkUnits = boardCamMove.wizardUnits;
            } else if(faction == "robot"){
                checkUnits = boardCamMove.robotUnits;
            } else if(faction == "alien")
            {
                checkUnits = boardCamMove.alienUnits;
            }
            units = GameObject.FindGameObjectsWithTag("Unit");
            foreach (GameObject unit in checkUnits)
            {
                Vector3Int coord = new Vector3Int (unit.GetComponent<UnitTileCheck>().coordinate.x, unit.GetComponent<UnitTileCheck>().coordinate.y,0);
                if(coord == cellPos)
                {
                    ignoreList.Add(cellPos);
                }
            }
            if (!ignoreList.Contains(cellPos))
            {
                prevState.Add(board.GetTile(cellPos));
                tilesInRange.Add(cellPos);
            }
            ignoreList.Clear();
        }
    }

    void FireBolt()
    {
        units = GameObject.FindGameObjectsWithTag("Unit");
        List<GameObject> fireBoltUnits = new List<GameObject>();
        foreach (GameObject unit in units)
        {
            Interactable interactable = unit.GetComponent<Interactable>();
            if (interactable.unitSelected)
            {
                faction = interactable.faction;
                foreach (GameObject unit2 in units)
                {
                    interactable = unit2.GetComponent<Interactable>();
                    if (interactable.faction == faction || interactable.mainUnit)
                    {
                        interactable.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
                    }
                    else if (interactable.faction != faction && !interactable.mainUnit)
                    {
                        fireBoltUnits.Add(unit2);
                    }
                }
                unit.GetComponent<fireBoltFunc>().fireBoltSelected = true;
                unit.GetComponent<fireBoltFunc>().fireBoltUnits = fireBoltUnits;
            }
        }
        deselectUnits();
    }

    void FlameBomb()
    {
        units = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject unit in units)
        {
            if (unit.GetComponent<Interactable>().unitSelected)
            {
                unit.GetComponent<flameBombFunc>().flameBombSelected = true;
            }
        }
        deselectUnits();
    }

    void Heal()
    {
        units = GameObject.FindGameObjectsWithTag("Unit");     
        List<GameObject> healableUnits = new List<GameObject>();
        foreach (GameObject unit in units)
        {
            Interactable interactable = unit.GetComponent<Interactable>();
            if (interactable.unitSelected)
            {
                faction = interactable.faction;
                foreach (GameObject unit2 in units)
                {
                    interactable = unit2.GetComponent<Interactable>();
                    if (interactable.health >= interactable.maxHealth && interactable.faction == faction)
                    {
                        interactable.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
                    } else if (interactable.health < interactable.maxHealth && interactable.faction == faction)
                    {
                        healableUnits.Add(unit2);
                    }
                }
                unit.GetComponent<HealFunc>().healSelected = true;
                unit.GetComponent<HealFunc>().healableUnits = healableUnits;
            }
        }
            deselectUnits();
    }

    void Hivemind()
    {
        units = GameObject.FindGameObjectsWithTag("Unit");
        List<GameObject> buffableUnits = new List<GameObject>();
        foreach (GameObject unit in units)
        {
            Interactable interactable = unit.GetComponent<Interactable>();
            if (interactable.unitSelected)
            {
                interactable.GetComponent<TeamBuffFunc>().buffEnabled = true;
                faction = interactable.faction;
                foreach (GameObject unit2 in units)
                {
                    interactable = unit2.GetComponent<Interactable>();
                    if (interactable.faction == faction)
                    {
                        buffableUnits.Add(unit2);
                    }
                }
                unit.GetComponent<TeamBuffFunc>().buffableUnits = buffableUnits;
            }
        }
        deselectUnits();
    }

    void Enrage()
    {
        units = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject unit in units)
        {
            if (unit.GetComponent<Interactable>().unitSelected)
            {
                unit.GetComponent<enrageFunc>().enrageEnabled = true;
            }
        }
        deselectUnits();
    }

    void Tank()
    {
        units = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject unit in units)
        {
            if (unit.GetComponent<Interactable>().unitSelected)
            {
                unit.GetComponent<tankFunc>().tankEnabled = true;
            }
        }
        deselectUnits();
    }

    void Poison()
    {
        units = GameObject.FindGameObjectsWithTag("Unit");
        List<GameObject> poisonUnits = new List<GameObject>();
        foreach (GameObject unit in units)
        {
            Interactable interactable = unit.GetComponent<Interactable>();
            if (interactable.unitSelected)
            {
                faction = interactable.faction;
                foreach (GameObject unit2 in units)
                {
                    interactable = unit2.GetComponent<Interactable>();
                    if (interactable.faction == faction || interactable.mainUnit)
                    {
                        interactable.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
                    } else if (interactable.faction != faction && !interactable.mainUnit)
                    {
                        poisonUnits.Add(unit2);
                    }
                }
                unit.GetComponent<poisonFunc>().poisonSelected = true;
                unit.GetComponent<poisonFunc>().poisonUnits = poisonUnits;
            }
        }
        deselectUnits();
    }

    void deselectUnits()
    {
        units = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject unit in units)
        {
            Interactable interactable = unit.GetComponent<Interactable>();
            interactable.unitSelected = false;
        }
    }
}