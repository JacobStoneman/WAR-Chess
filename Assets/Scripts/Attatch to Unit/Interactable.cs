using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Interactable : MonoBehaviour {

    public bool onWinnningTeam = false;
    public bool poisoned = false;
    public float pRating;
    public float origPRating;
    public int range;
    public string faction;
    public float maxHealth;
    public TileBase factionTile;
    public float damage;
    public Tilemap board;
    public Grid grid;
    public bool mainUnit;
    GameObject[] units;
    public Vector3Int coordinate;
    public Vector3Int coord;
    public float baseMax;
    public float health;
    public float healthPercentage;
    public bool unitSelected;
    public List<Vector3Int> tileRange;
    public List<TileBase> prevState;
    public bool moveSelected = false;
    public BoardCameraMovement boardCamMove;
    public bool fightStarted;
    public float baseDamage;
    int baseRange;
    public bool sceneLoaded = false;
    GameObject hexMaster;
    public int fighterNum;
    public Color colour;
    public bool tileBuffed;
    public bool teamBuffed;
    List<Vector3Int> tilesInRange = new List<Vector3Int>();
    List<Vector3Int> tempTilesInRange = new List<Vector3Int>();
    List<Vector3Int> ignoreList = new List<Vector3Int>();
    public BoardCameraMovement.boardTile currentTile;

    public void Awake()
    {
        baseMax = maxHealth;
        baseDamage = damage;
        range = range + 1;
        baseRange = range;
        hexMaster = GameObject.Find("HexMaster");
        health = maxHealth;
        if (faction == "wizard")
        {
            List<GameObject> wizardUnits = grid.GetComponentInChildren<BoardCameraMovement>().wizardUnits;
            wizardUnits.Add(gameObject);
        }
        if (faction == "robot")
        {
            List<GameObject> robotUnits = grid.GetComponentInChildren<BoardCameraMovement>().robotUnits;
            robotUnits.Add(gameObject);
        }
        if (faction == "alien")
        {
            List<GameObject> alienUnits = grid.GetComponentInChildren<BoardCameraMovement>().alienUnits;
            alienUnits.Add(gameObject);
        }
        colour = GetComponent<SpriteRenderer>().color;
    }

    public void Start()
    {
        boardCamMove = board.GetComponent<BoardCameraMovement>();
        coord = GetComponent<UnitTileCheck>().coordinate;
        //AddBlock();
        tileBuffCheck(coord);
        setPRating();
        origPRating = pRating;
    }

    public void setInhabitant()
    {
        boardCamMove = board.GetComponent<BoardCameraMovement>();
        foreach (BoardCameraMovement.boardTile bTile in boardCamMove.allTiles)
        {
            if (coord == bTile.pos)
            {
                currentTile = bTile;
                bTile.inhabitant = gameObject;
            }
        }
    }

    public List<Vector3Int> findPossibleMoves()
    {
        tilesInRange.Clear();
        UnitTileCheck tileCheck = GetComponent<UnitTileCheck>();
        //Vector3Int coord = tileCheck.coordinate;
        coord = currentTile.pos;

        tilesInRange.Add(new Vector3Int(coord.x, coord.y, 0));
        for (int i = 1; i <= range; i++)
        {
            tempTilesInRange.Clear();
            foreach (Vector3Int val in tilesInRange)
            {
                tempTilesInRange.Add(val);
            }
            foreach (Vector3Int tile in tempTilesInRange)
            {
                Vector3Int cellPos;
                cellPos = new Vector3Int(tile.x, tile.y + 1, 0);
                CheckCell(cellPos);
                if (tile.y % 2 == 1)
                {
                    cellPos = new Vector3Int(tile.x + 1, tile.y + 1, 0);
                    CheckCell(cellPos);
                    cellPos = new Vector3Int(tile.x + 1, tile.y - 1, 0);
                    CheckCell(cellPos);
                }
                else if (tile.y % 2 == 0)
                {
                    cellPos = new Vector3Int(tile.x - 1, tile.y + 1, 0);
                    CheckCell(cellPos);
                    cellPos = new Vector3Int(tile.x - 1, tile.y - 1, 0);
                    CheckCell(cellPos);
                }
                cellPos = new Vector3Int(tile.x + 1, tile.y, 0);
                CheckCell(cellPos);
                cellPos = new Vector3Int(tile.x, tile.y - 1, 0);
                CheckCell(cellPos);
                cellPos = new Vector3Int(tile.x - 1, tile.y, 0);
                CheckCell(cellPos);
            }
        }
        tilesInRange.RemoveAt(0);
        return tilesInRange;
    }

    void CheckCell(Vector3Int cellPos)
    {
        if (board.GetTile(cellPos) != null && !tilesInRange.Contains(cellPos))
        {
            List<GameObject> checkUnits = new List<GameObject>();
            if (faction == "wizard")
            {
                checkUnits = boardCamMove.wizardUnits;
            }
            else if (faction == "robot")
            {
                checkUnits = boardCamMove.robotUnits;
            }
            else if (faction == "alien")
            {
                checkUnits = boardCamMove.alienUnits;
            }
            units = GameObject.FindGameObjectsWithTag("Unit");
            foreach (GameObject unit in checkUnits)
            {
                Vector3Int coord = new Vector3Int(unit.GetComponent<UnitTileCheck>().coordinate.x, unit.GetComponent<UnitTileCheck>().coordinate.y, 0);
                if (coord == cellPos)
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

    public void setPRating()
    {
        pRating = ((health * damage) / 10) * range;
    }

    public void AddBlock()
    {
        boardCamMove.blockedTiles.Add(currentTile.pos);
    }

    [System.Serializable]
    public class Action
    {
        public Color colour;
        public Sprite sprite;
        public string title;
        public string func;
    }

    public string title;
    public Action[] options;

    public void MouseDown()
    {
        BoardCameraMovement boardCamMove = board.GetComponent<BoardCameraMovement>();
        
        if (boardCamMove.currentTurn == "wizard")
        {
            units = GameObject.FindGameObjectsWithTag("Unit");
            if (faction == "wizard")
            {
                unitSelected = true;
            }
            foreach (GameObject unit in units)
            {
                if (!unit.GetComponent<Interactable>().unitSelected)
                {
                    unit.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
                }
            }
            if (this.faction == "wizard")
            {
                RadialMenuSpawner.ins.SpawnMenu(this);
            }
        }
    }

    void Update()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        if (health <= 0)
        {
            print(":(");
            switch (faction){
                case "wizard":
                    boardCamMove.wizardUnits.Remove(gameObject);
                    boardCamMove.blockedTiles.Remove(coord);
                    break;
                case "alien":
                    boardCamMove.alienUnits.Remove(gameObject);
                    boardCamMove.blockedTiles.Remove(coord);
                    break;
                case "robot":
                    boardCamMove.robotUnits.Remove(gameObject);
                    boardCamMove.blockedTiles.Remove(coord);
                    break;
            }
            Destroy(gameObject);
        }
        if (moveSelected)
        {
            coordinate = boardCamMove.coordinate;
            if (Input.GetMouseButtonUp(0))
            {
                UnitTileCheck UTC = GetComponent<UnitTileCheck>();
                Vector3Int newCoord = new Vector3Int(coordinate.x, coordinate.y, 0); //Gets coordinate of mouse position
                if (tileRange.Contains(newCoord))
                {
                    Vector3Int prevCoord = coord;
                    coord = new Vector3Int (UTC.coordinate.x,UTC.coordinate.y,0);
                    transform.position = grid.CellToWorld(coordinate);
                    tilePrev();
                    tileBuffCheck(newCoord);
                    if (boardCamMove.blockedTiles.Contains(newCoord))
                    {
                        StaticDataScript.noFight = false;
                        StaticDataScript.fighterNum = 0;
                        StaticDataScript.winHealth = 0;
                        StaticDataScript.coord = coord;
                        StaticDataScript.player1 = gameObject.name;
                        StaticDataScript.player1Health = health;
                        StaticDataScript.player1MaxHealth = maxHealth;
                        StaticDataScript.player1Faction = faction;
                        StaticDataScript.player1Damage = damage;
                        StaticDataScript.player1Colour = colour;
                        fighterNum = 1;
                        //units = GameObject.FindGameObjectsWithTag("Unit");
                        foreach (GameObject unit in units)
                        {
                            Vector3Int enemyCoord = unit.GetComponent<UnitTileCheck>().coordinate;
                            if (newCoord == new Vector3Int(enemyCoord.x, enemyCoord.y, 0))
                            {
                                Interactable interactable = unit.GetComponent<Interactable>();
                                StaticDataScript.player2 = unit.gameObject.name;
                                StaticDataScript.player2Health = interactable.health;
                                StaticDataScript.player2MaxHealth = interactable.maxHealth;
                                StaticDataScript.player2Faction = interactable.faction;
                                StaticDataScript.player2Damage = interactable.damage;
                                StaticDataScript.player2Colour = interactable.colour;
                                interactable.fighterNum = 2;
                            } 
                            
                        }
                        StaticDataScript.coord = newCoord;
                        fightStarted = true;
                    }
                    boardCamMove.blockedTiles.Remove(new Vector3Int(coord.x, coord.y, 0));
                    boardCamMove.blockedTiles.Add(newCoord);
                    coord = grid.WorldToCell(transform.position);
                    coord.z = 0;
                    foreach (BoardCameraMovement.boardTile bTile in boardCamMove.allTiles)
                    {
                        if (bTile.pos == coord)
                        {
                            foreach(BoardCameraMovement.boardTile bTile2 in boardCamMove.allTiles)
                            {
                                if (bTile2.pos == prevCoord)
                                {
                                    bTile2.inhabitant = null;
                                    break;
                                }
                            }
                            currentTile = bTile;
                            bTile.inhabitant = gameObject;
                            break;
                        }
                    }
                    if (!fightStarted) { boardCamMove.turnSwitch(); }
                    boardCamMove.newLogEntry(currentTile, "movement", gameObject, null); 
                }
                else
                {
                    print("cannot move to this tile");
                }
                moveSelected = false;
                tilePrev();
            }
        }
        healthPercentage = Mathf.Ceil((health / maxHealth) * 100);
        if (fightStarted)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Arena"));
            fightStarted = false;
        }
    }
    public void OnEnable()
    {
        if (!StaticDataScript.noFight)
        {
            if (fighterNum == StaticDataScript.fighterNum)
            {
                Destroy(gameObject);
            }
            else if (fighterNum != StaticDataScript.fighterNum)
            {
                if (fighterNum == 1 || fighterNum == 2)
                {
                    bool convert = true;
                    foreach(TileBase tile in boardCamMove.nonConTiles)
                    {
                        if (board.GetTile(StaticDataScript.coord) == tile)
                        {
                            convert = false;
                        }
                    }
                    if (convert)
                    {
                        board.GetComponent<BoardCameraMovement>().convertTile(factionTile);
                    }
                    health = StaticDataScript.winHealth;
                }
                if (faction == "wizard")
                {
                    List<GameObject> wizardUnits = grid.GetComponentInChildren<BoardCameraMovement>().wizardUnits;
                    wizardUnits.Add(gameObject);
                }
                if (faction == "robot")
                {
                    List<GameObject> robotUnits = grid.GetComponentInChildren<BoardCameraMovement>().robotUnits;
                    robotUnits.Add(gameObject);
                }
                if (faction == "alien")
                {
                    List<GameObject> alienUnits = grid.GetComponentInChildren<BoardCameraMovement>().alienUnits;
                    alienUnits.Add(gameObject);
                }
                coord = grid.WorldToCell(transform.position);
                coord.z = 0;
                tileBuffCheck(coord);
                boardCamMove.blockedTiles.Add(new Vector3Int(coord.x, coord.y, 0));
                fighterNum = 0;
                setInhabitant();
            }
        }
    }
    public void tileBuffCheck(Vector3Int pos)
    {     
        if (board.GetTile(pos) == factionTile)
        {
            if (!tileBuffed)
            {
                damage = baseDamage;
                damage = damage + ((damage / 100) * 20);
                range = range + 1;
                checkBerserker();
                checkAll();
                tileBuffed = true;
            }
            else
            {
                checkBerserker();
                checkAll();
            }
        }
        else
        {
            if (tileBuffed == true)
            {
                damage = baseDamage;
                range = baseRange;
                checkBerserker();
                checkAll();
            } else
            {
                damage = baseDamage;
                range = baseRange;
                checkBerserker();
                checkAll();
            }
            tileBuffed = false;
        }
    }
    void tilePrev()
    {
        int i = 0;
        foreach (Vector3Int tile in tileRange)
        {
            board.SetTile(tile, prevState[i]);
            i++;
        }
    }

    void checkAll()
    {
        if (teamBuffed)
        {
            damage = damage + 20;
        }
    }

    void checkBerserker()
    {
        if (gameObject.name == "Berserker")
        {
            if (GetComponent<enrageFunc>().enrageApplied)
            {
                damage = damage + 60;
            }
        }
    }
}
