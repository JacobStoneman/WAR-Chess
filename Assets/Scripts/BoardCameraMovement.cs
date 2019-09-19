using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoardCameraMovement : MonoBehaviour {

    public class boardTile
    {
        public Vector3Int pos;
        public float baseVal;
        public float currentVal;
        public GameObject inhabitant;
        public TileBase state;
        public bool burning;
    }

    public class potentialMove
    {
        public GameObject targetPiece;
        public GameObject attacker;
        public float moveValue;
        public string action;
        public boardTile bTile;
    }

    public class burningTile
    {
        public boardTile bTile;
        public int endPoint;
        public GameObject icon;
    }
    public bool fightSimulated;
    public int burningTileDamage = 30;
    public bool closeToWinnning;
    public List<potentialMove> allMoves = new List<potentialMove>();
    int prevSize = 51; //Should be number of total units
    bool inhabSet = false;
    public bool debugMode = false;
    public GameObject flameIcon, sceneMaster, content, logTextPrefab;
    public Vector3Int coordinate;
    Vector3 camPos;
    public Grid grid; //Grid tilemap is attached to
    public float smoothTime = 0.2f;
    private Vector3 velocity = Vector3.zero;
    public new GameObject camera; //Camera for the game board
    public Vector3 startPos; //Should be 3,2,-500
    public bool reached;
    public Vector3 boardCentre; // 2.7,2.05,-500
    GameObject zoomButton;
    string camState;
    GameObject healthBar;
    public GameObject infoPanel;
    Slider healthBarFill;
    public Slider wizardS, alienS, robotS;
    float healthMax;
    float healthCurrent;
    bool healthFull;
    float healthPercentage;
    public float healthTarget; 
    GameObject selector;
    public Sprite selecYellow;
    public Sprite selecRed;
    Tilemap tilemap;
    public TileBase targetTile;
    public GameObject[] units;
    public List<GameObject> wizardUnits = new List<GameObject>();
    public List<GameObject> robotUnits = new List<GameObject>();
    public List<GameObject> alienUnits = new List<GameObject>();
    public List<Vector3Int> blockedTiles = new List<Vector3Int>();
    public List<burningTile> burningTiles = new List<burningTile>();
    public List<boardTile> allTiles = new List<boardTile>();
    public List<TileBase> nonConTiles = new List<TileBase>();
    public List<Vector3Int> nonConPos1 = new List<Vector3Int>();
    public List<Vector3Int> nonConPos2 = new List<Vector3Int>();
    public string currentTurn;
    public Text turnText;
    public bool wDestroy;
    public bool aDestroy;
    public bool rDestroy;
    public int lossCount;
    public int turnCount = 1;
    public Image unitImage;
    public Image Crown;
    public GameObject unitSelectedGUI;
    public string buttonHover;
    public int wWinCount;
    public int aWinCount;
    public int rWinCount;
    public int wPos, aPos, rPos;
    public string winningTeam = null;
    public int countNeeded = 200;
    public float aiWaitTime = 0.5f;
    // Use this for initialization
    void Start()
    {
        print(PlayerPrefs.GetInt("alienWinRate"));
        print(PlayerPrefs.GetInt("wizardWinRate"));
        print(PlayerPrefs.GetInt("robotWinRate"));
        aPos = wPos = rPos = 2;
        tilemap = GetComponent<Tilemap>();
        currentTurn = "wizard";
        camera = GameObject.Find("Main Camera");
        selector = GameObject.Find("Selection");
        healthBar = GameObject.Find("HealthBar");
        infoPanel = GameObject.Find("InfoPanel");
        healthBarFill = healthBar.GetComponent<Slider>();
        findUnits();
        zoomButton = GameObject.Find("ZoomButton");
        healthTarget = 100;
        healthBarFill.value = 100;
        foreach (Vector3Int tile in tilemap.cellBounds.allPositionsWithin)
        {
            //possibly change this to use new boardTile class
            //Works out whether the current tile can be controlled
            Vector3Int tileToAdd = new Vector3Int();                //gets the position of the new tile
            TileBase curTile = tilemap.GetTile(tile);               //gets the tilebase (colour) of that position
            int i = 0;
            foreach (TileBase tile2 in nonConTiles)                 //for each type of non controllable tile
            {
                if (curTile == tile2)                               //if the tile at current position being looked at is one of these tiles
                {
                    tileToAdd = tile;
                    if (i == 0)
                    {
                        nonConPos1.Add(tileToAdd);
                        tileToAdd = new Vector3Int(0, 0, 0);
                    }
                    else if (i == 1)
                    {
                        nonConPos2.Add(tileToAdd);
                        tileToAdd = new Vector3Int(0, 0, 0);
                    }                                               //add it to relevant list of tiles that cannot be controlled
                }
                i++;
            }
            if (tilemap.GetTile(tile) != null)
            {
                //Creates boardTile objects for each actual tile
                boardTile newTile = new boardTile();
                newTile.pos = tile;
                float tempX = newTile.pos.x;
                float tempY = newTile.pos.y;
                if (curTile == nonConTiles[1])
                {
                    newTile.baseVal = 500;
                } else {
                    if (curTile == nonConTiles[0])
                    {
                        newTile.baseVal = 250;
                    } else
                    {
                        newTile.baseVal = 0;
                    }
                    float xDist;
                    if(newTile.pos.y % 2 == 0)
                    {
                        xDist = Mathf.Abs((startPos.x + 0.5f) - tempX);
                    } else
                    {
                        xDist = Mathf.Abs((startPos.x) - tempX);
                    }
                    float yDist = Mathf.Abs((startPos.y+0.5f) - tempY);
                    float finalX = startPos.x - xDist;
                    float finalY = startPos.y - yDist;
                    newTile.baseVal = newTile.baseVal + (finalY * 10);
                    newTile.baseVal = newTile.baseVal + (finalX * 10);
                }
                newTile.currentVal = newTile.baseVal;
                newTile.inhabitant = null;
                newTile.state = tilemap.GetTile(tile);
                newTile.burning = false;
                allTiles.Add(newTile);
            }
        }
        StartCoroutine(startAiTurn());
    }

    // Update is called once per frame
    void Update()
    {
        if (!inhabSet)
        {
            findUnits();
            foreach (GameObject unit in units)
            {
                unit.GetComponent<Interactable>().setInhabitant();
                unit.GetComponent<Interactable>().AddBlock();
            }
            inhabSet = true;
        }
        if (lossCount >= 2)
        {
            if (!wDestroy)
            {
                StaticDataScript.gameWinner = "Wizards";
            }
            else if (!aDestroy)
            {
                StaticDataScript.gameWinner = "Aliens";
            }
            else if (!rDestroy)
            {
                StaticDataScript.gameWinner = "Robots";
            }
            SceneManager.LoadScene("EndScreen");
        }
        if(wWinCount >= countNeeded)
        {
            int wizardWinRate = 0;
            wizardWinRate = PlayerPrefs.GetInt("wizardWinRate");
            StaticDataScript.gameWinner = "Wizards";
            PlayerPrefs.SetInt("wizardWinRate", wizardWinRate + 1);
            SceneManager.LoadScene("EndScreen");
        }
        else if (aWinCount >= countNeeded)
        {
            int alienWinRate = 0;
            alienWinRate = PlayerPrefs.GetInt("alienWinRate");
            StaticDataScript.gameWinner = "Aliens";
            PlayerPrefs.SetInt("alienWinRate", alienWinRate + 1);
            SceneManager.LoadScene("EndScreen");
        }
        else if (rWinCount >= countNeeded)
        {
            int robotWinRate = 0;
            robotWinRate = PlayerPrefs.GetInt("robotWinRate");
            StaticDataScript.gameWinner = "Robots";
            PlayerPrefs.SetInt("robotWinRate", robotWinRate + 1);
            SceneManager.LoadScene("EndScreen");
        }
        //// Turn switch on key press, remove at some point //////////////////////////////////
        if (Input.GetKeyDown("r"))
        {
            turnSwitch();
        }
        //////////////////////////////////////////////////////////////////////////////////////
        //// enable debug mode, remove at some point /////////////////////////////////////////
        if (Input.GetKeyDown("p"))
        {
            if (debugMode)
            {
                debugMode = false;
            }
            else
            {
                debugMode = true;
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////
        turnText.text = "Turn: " + currentTurn;
        camState = zoomButton.GetComponent<BoardZoom>().cameraState;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        coordinate = grid.WorldToCell(mouseWorldPos);
        targetTile = tilemap.GetTile(new Vector3Int(coordinate.x,coordinate.y,0));
        
        //Selector on tile hover over
        if (targetTile != null)
        {
            selector.SetActive(true);
            selector.transform.position = new Vector3(grid.CellToWorld(coordinate).x, grid.CellToWorld(coordinate).y, -1);
            if (Input.GetKeyUp("p"))
            {
                foreach (boardTile bTile in allTiles)
                {
                    if (new Vector3(coordinate.x, coordinate.y, 0) == bTile.pos)
                    {
                        print("Tile is at position " + bTile.pos);
                        print("baseVal = " + bTile.baseVal);
                        print("currentVal = " + bTile.currentVal);
                        print("Tile is inhabited by " + bTile.inhabitant);
                        print("Tile state = " + bTile.state);
                        if (bTile.burning)
                        {
                            print("Tile is burning");
                        }
                        else
                        {
                            print("Tile is not burning");
                        }
                    }
                }
            }
            if (blockedTiles.Contains(new Vector3Int(coordinate.x, coordinate.y, 0))) {
                selector.GetComponent<SpriteRenderer>().sprite = selecRed;
                if (unitSelectedGUI == null)
                {
                    SpawnInfo();
                }
            } else
            {
                selector.GetComponent<SpriteRenderer>().sprite = selecYellow;
                if (unitSelectedGUI == null)
                {
                    infoPanel.SetActive(false);
                }
            }
        } else
        {
            selector.SetActive(false);
            if (unitSelectedGUI == null)
            {
                infoPanel.SetActive(false);
            }
        }

        if(healthBarFill.value < healthTarget)
        {
            healthBarFill.value = healthBarFill.value + 1;
        } else if (healthBarFill.value > healthTarget)
        {
            healthBarFill.value = healthBarFill.value - 1;
        }
        healthBar.GetComponentInChildren<Text>().text = healthBarFill.value.ToString() + "%";
        if (unitSelectedGUI != null)
        {
            unitImage.sprite = unitSelectedGUI.GetComponent<SpriteRenderer>().sprite;
            unitImage.SetNativeSize();
            unitImage.GetComponent<ImageSizeCheck>().checkImage(unitSelectedGUI.GetComponent<Interactable>().colour);
            panelYCheck panCheck = infoPanel.GetComponent<panelYCheck>();
            panCheck.abilityName.text = buttonHover;
            switch (buttonHover)
            {
                case null:
                    panCheck.abilityText.text = null;
                    break;
                case "Move":
                    panCheck.abilityText.text = "Moves a unit";
                    break;
                case "Fire Bolt":
                    panCheck.abilityText.text = "Deals a large amount of damage to a single target";
                    break;
                case "Lightning Strike":
                    panCheck.abilityText.text = "Deals a large amount of damage to a single target";
                    break;
                case "Flame Bomb":
                    panCheck.abilityText.text = "Sets a tile on fire for a number of turns";
                    break;
                case "Enrage":
                    panCheck.abilityText.text = "Increases unit's attack for a number of turns";
                    break;
                case "Armour Up":
                    panCheck.abilityText.text = "Increases unit's attack for a number of turns";
                    break;
                case "Poison":
                    panCheck.abilityText.text = "Reduces a unit's maximum health by 20%";
                    break;
                case "Heal":
                    panCheck.abilityText.text = "Heals a unit for a certain amount";
                    break;
                case "Hivemind":
                    panCheck.abilityText.text = "Increases team's attack for a short amount of time";
                    break;
            }
        }
        CameraMovement();
    }

    public void convertTile(TileBase fTile)
    {
        List<Vector3Int> tilesFound = new List<Vector3Int>();
        List<boardTile> tilesToConvert = new List<boardTile>();
        boardTile centreTile = new boardTile();
        foreach(boardTile bTile in allTiles)
        {
            if (bTile.pos == StaticDataScript.coord)
            {
                centreTile = bTile;
            }
        }
        Vector3Int cellPos;
        cellPos = new Vector3Int(centreTile.pos.x, centreTile.pos.y + 1, 0);
        tilesFound.Add(cellPos);
        if(centreTile.pos.y % 2 == 1)
        {
            cellPos = new Vector3Int(centreTile.pos.x + 1, centreTile.pos.y + 1, 0);
            tilesFound.Add(cellPos);
            cellPos = new Vector3Int(centreTile.pos.x + 1, centreTile.pos.y - 1, 0);
            tilesFound.Add(cellPos);
        } else if (centreTile.pos.y % 2 == 0)
        {
            cellPos = new Vector3Int(centreTile.pos.x - 1, centreTile.pos.y + 1, 0);
            tilesFound.Add(cellPos);
            cellPos = new Vector3Int(centreTile.pos.x - 1, centreTile.pos.y - 1, 0);
            tilesFound.Add(cellPos);
        }
        cellPos = new Vector3Int(centreTile.pos.x + 1, centreTile.pos.y, 0);
        tilesFound.Add(cellPos);
        cellPos = new Vector3Int(centreTile.pos.x, centreTile.pos.y - 1, 0);
        tilesFound.Add(cellPos);
        cellPos = new Vector3Int(centreTile.pos.x - 1, centreTile.pos.y, 0);
        tilesFound.Add(cellPos);
        tilesFound.Add(centreTile.pos);
        foreach(Vector3Int tileFound in tilesFound)
        {
            foreach(boardTile bTile in allTiles)
            {
                if(tileFound == bTile.pos)
                {
                    if(bTile.state != nonConTiles[0] && bTile.state != nonConTiles[1])
                    {
                        tilesToConvert.Add(bTile);
                    }
                }
            }
        }
        foreach(boardTile bTile in tilesToConvert)
        {
            tilemap.SetTile(bTile.pos, fTile);
            bTile.state = fTile;
        }
        tilesFound.Clear();
        tilesToConvert.Clear();
    }

    public void turnSwitch()
    {
        if (currentTurn == "alien")
        {
            currentTurn = "wizard";
        }

        else if (currentTurn == "wizard")
        {
            currentTurn = "robot";
        }
        else if (currentTurn == "robot")
        {
            currentTurn = "alien";
        }
        turnCount++;
        checkTurn();
        foreach (GameObject unit in units)
        {
            unit.GetComponent<Interactable>().setInhabitant();
        }
        allMoves.Clear();
        StartCoroutine(startAiTurn());
    }

    void simulateFight(GameObject f1, GameObject f2)
    {
        print("fight simulated between " + f1.name + " and " + f2.name);
        StaticDataScript.noFight = false;
        Interactable f1Int = f1.GetComponent<Interactable>();
        Interactable f2Int = f2.GetComponent<Interactable>();
        float totalNum = f1Int.origPRating + f2Int.origPRating;
        while (f1Int.health > 0 && f2Int.health > 0)
        {
            float ranNum = Random.Range(0, totalNum);
            if (ranNum <= f1Int.origPRating)
            {
                f2Int.health = f2Int.health - f1Int.damage;
                print(f2.name + " hit");
            } else if (ranNum > f1Int.origPRating)
            {
                f1Int.health = f1Int.health - f2Int.damage;
                print(f1.name + " hit");
            }
        }
        findUnits();
    }

    IEnumerator startAiTurn()
    {
        yield return new WaitForSeconds(aiWaitTime);
        aiTurn();
    }

    void aiTurn() {
        float currentMax = -9999999;
        List<GameObject> units = new List<GameObject>();

        //Find which units to use and whether they are 1st, 2nd or 3rd
        if (currentTurn == "alien")
        {
            units = alienUnits;
            print("Alien Turn Done");
        }
        else if (currentTurn == "robot")
        {
            units = robotUnits;
            print("Robot Turn Done");
        }
        else if (currentTurn == "wizard"){
            units = wizardUnits;
            print("Wizard Turn Done");
        }
        foreach (GameObject unit in units)
        {
            boardTile currentPosition = null;
            foreach (boardTile bTile in allTiles)
            {
                if (unit.GetComponent<Interactable>().coord == bTile.pos)
                {
                    currentPosition = bTile;
                }
            }
            List<Vector3Int> possibleMoves = unit.GetComponent<Interactable>().findPossibleMoves();
            foreach (Vector3Int move in possibleMoves)
            {
                foreach (boardTile bTile2 in allTiles)
                {
                    if (move == bTile2.pos)
                    {
                        if (bTile2.currentVal > currentPosition.currentVal)     //Tile buff bonuses should change the current values
                        {
                            float valueRecalc = Mathf.Abs(bTile2.currentVal - currentPosition.currentVal);

                            //Changes value of move based on chances of surviving fight and whether attacking a unit on the team that is closest to winning
                            if (bTile2.inhabitant != null)
                            {
                                if(unit.GetComponent<Interactable>().pRating >= (bTile2.inhabitant.GetComponent<Interactable>().pRating * 1.5f))
                                {
                                    valueRecalc = valueRecalc + (bTile2.inhabitant.GetComponent<Interactable>().pRating / 10);
                                    if(closeToWinnning && bTile2.inhabitant.GetComponent<Interactable>().faction == winningTeam)
                                    {
                                        valueRecalc = valueRecalc * 2;
                                    }
                                } else if (bTile2.inhabitant.GetComponent<Interactable>().pRating >= (unit.GetComponent<Interactable>().pRating * 1.5f))
                                {
                                    valueRecalc = valueRecalc - (bTile2.inhabitant.GetComponent<Interactable>().pRating / 10);
                                } else
                                {
                                    valueRecalc = valueRecalc + ((unit.GetComponent<Interactable>().pRating - bTile2.inhabitant.GetComponent<Interactable>().pRating)/10);
                                    if (closeToWinnning && bTile2.inhabitant.GetComponent<Interactable>().faction == winningTeam)
                                    {
                                        valueRecalc = valueRecalc * 1.5f;
                                    }
                                }
                            }

                            //Changes the value of the move based on the teams score
                            if (currentTurn == "alien" && aPos == 1)
                            {
                                valueRecalc = valueRecalc / 2;
                            }
                            else if (currentTurn == "alien" && aPos == 3)
                            {
                                valueRecalc = valueRecalc * 2;
                            }
                            else if (currentTurn == "robot" && rPos == 1)
                            {
                                valueRecalc = valueRecalc / 2;
                            }
                            else if (currentTurn == "robot" && rPos == 3)
                            {
                                valueRecalc = valueRecalc * 2;
                            }
                            else if (currentTurn == "wizard" && wPos == 1)
                            {
                                valueRecalc = valueRecalc / 2;
                            }
                            else if (currentTurn == "wizard" && wPos == 3)
                            {
                                valueRecalc = valueRecalc * 2;
                            }
                            //If a move is found that is better than any other previously found
                            if (valueRecalc > currentMax)
                            {
                                currentMax = valueRecalc;
                                //Reset move list and add new move
                                allMoves.Clear();
                                allMoves.Add(createNewMove(bTile2, "movement", valueRecalc, unit, null));
                            } else if (valueRecalc == currentMax)
                            {
                                allMoves.Add(createNewMove(bTile2, "movement", valueRecalc, unit, null));
                            }
                        }
                    }
                }
            }
            if (unit.name == "Wizard")
            {
                //Checks values of wizards abilities
                currentMax = evaluateWizard("wizard", unit, currentMax);
                currentMax = evaluateWizard("alien", unit, currentMax);
                currentMax = evaluateWizard("robot", unit, currentMax);
            } else if (unit.name == "Cleric")
            {
                if (currentTurn == "alien")
                {

                }
                else if (currentTurn == "robot")
                {

                }
            }
            else if (unit.name == "Berserker")
            {

            }
            else if (unit.name == "Sentinel")
            {

            }
            else if (unit.name == "Rogue")
            {

            }
        }
        foreach(potentialMove move in allMoves)
        {
            print("possible " + move.action + " at " + move.bTile.pos + " with " + move.targetPiece + " with value of " + move.moveValue);
        }
        print("//////////////////////////////////////////");

        //Choose a random action out of list of best possible actions
        int ranPos = Random.Range(0, allMoves.Count);
        newLogEntry(allMoves[ranPos].bTile, allMoves[ranPos].action, allMoves[ranPos].targetPiece, allMoves[ranPos].attacker);
        //Move selected piece to new position
        if (allMoves[ranPos].action == "movement")
        {
            Vector3Int prevPos = allMoves[ranPos].targetPiece.GetComponent<Interactable>().currentTile.pos;
            allMoves[ranPos].targetPiece.transform.position = grid.CellToWorld(allMoves[ranPos].bTile.pos);
            allMoves[ranPos].targetPiece.GetComponent<Interactable>().coord = allMoves[ranPos].bTile.pos;           
            bool fight = false;
            //If there is a piece on new position
            foreach (boardTile bTile in allTiles)
            {
                if (bTile.pos == allMoves[ranPos].bTile.pos)
                {
                    if (bTile.inhabitant != null)
                    {
                        Interactable interactable = allMoves[ranPos].targetPiece.GetComponent<Interactable>();
                        StaticDataScript.noFight = false;
                        StaticDataScript.fighterNum = 0;
                        StaticDataScript.winHealth = 0;
                        StaticDataScript.coord = allMoves[ranPos].bTile.pos;

                        StaticDataScript.player1 = interactable.gameObject.name;
                        StaticDataScript.player1Health = interactable.health;
                        StaticDataScript.player1MaxHealth = interactable.maxHealth;
                        StaticDataScript.player1Faction = interactable.faction;
                        StaticDataScript.player1Damage = interactable.damage;
                        StaticDataScript.player1Colour = interactable.colour;
                        interactable.fighterNum = 1;

                        Interactable interactable2 = bTile.inhabitant.GetComponent<Interactable>();
                        StaticDataScript.player2 = bTile.inhabitant.gameObject.name;
                        StaticDataScript.player2Health = interactable2.health;
                        StaticDataScript.player2MaxHealth = interactable2.maxHealth;
                        StaticDataScript.player2Faction = interactable2.faction;
                        StaticDataScript.player2Damage = interactable2.damage;
                        StaticDataScript.player2Colour = interactable2.colour;
                        interactable2.fighterNum = 2;
                        if (!fightSimulated)
                        {
                            interactable.fightStarted = true;
                            fight = true;
                        } else
                        {
                            simulateFight(allMoves[ranPos].targetPiece, bTile.inhabitant);
                        }
                    }
                }
            }

            foreach (boardTile bTile in allTiles)
            {
                if (bTile.pos == prevPos)
                {
                    bTile.inhabitant = null;
                }
                else if (bTile.pos == allMoves[ranPos].bTile.pos)
                {
                    allMoves[ranPos].targetPiece.GetComponent<Interactable>().currentTile = bTile;
                    bTile.inhabitant = allMoves[ranPos].targetPiece;
                }
            }
            allMoves[ranPos].targetPiece.GetComponent<Interactable>().tileBuffCheck(allMoves[ranPos].targetPiece.GetComponent<Interactable>().currentTile.pos);
            if (!fight)
            {
                turnSwitch();
            }
        } else if (allMoves[ranPos].action == "firebolt")
        {
            allMoves[ranPos].targetPiece.GetComponent<Interactable>().health = allMoves[ranPos].targetPiece.GetComponent<Interactable>().health - allMoves[ranPos].attacker.GetComponent<fireBoltFunc>().damage;
            turnSwitch();
        } else if (allMoves[ranPos].action == "flamebomb")
        {
            newBurnTile(allMoves[ranPos].bTile.pos, turnCount + 18);
            turnSwitch();
        } else if (allMoves[ranPos].action == "hivemind")
        {
            allMoves[ranPos].targetPiece.GetComponent<TeamBuffFunc>().buffableUnits = alienUnits;
            allMoves[ranPos].targetPiece.GetComponent<TeamBuffFunc>().buffEnabled = true;
            turnSwitch();
        }   
    }

    float evaluateWizard(string faction, GameObject unit, float currentMax)
    {
        List<GameObject> targetUnits = new List<GameObject>();
        if (faction == "alien")
        {
            targetUnits = alienUnits;
        } else if (faction == "wizard")
        {
            targetUnits = wizardUnits;
        } else if (faction == "robot")
        {
            targetUnits = robotUnits;
        }
        //Firebolt
        if (currentTurn != faction)
        {
            if (closeToWinnning && winningTeam == faction)
            {
                foreach (GameObject target in targetUnits)
                {
                    if (target.name != "Wizard")
                    {
                        Interactable tInteractable = target.GetComponent<Interactable>();
                        foreach (boardTile bTile in allTiles)
                        {
                            if (tInteractable.coord == bTile.pos)
                            {
                                foreach (TileBase nonCon in nonConTiles)
                                {
                                    if (bTile.state == nonCon)
                                    {
                                        if (tInteractable.health <= unit.GetComponent<fireBoltFunc>().damage)
                                        {
                                            if (tInteractable.pRating * 10 > currentMax)
                                            {
                                                currentMax = tInteractable.pRating * 10;
                                                allMoves.Clear();
                                                allMoves.Add(createNewMove(bTile, "firebolt", tInteractable.pRating * 10, target, unit));
                                            }
                                            else if (tInteractable.pRating * 10 == currentMax)
                                            {
                                                allMoves.Add(createNewMove(bTile, "firebolt", tInteractable.pRating * 10, target, unit));
                                            }
                                        }
                                        else if (tInteractable.health <= unit.GetComponent<fireBoltFunc>().damage * 2)
                                        {
                                            if (tInteractable.pRating > currentMax)
                                            {
                                                currentMax = tInteractable.pRating;
                                                allMoves.Clear();
                                                allMoves.Add(createNewMove(bTile, "firebolt", tInteractable.pRating, target, unit));
                                            }
                                            else if (tInteractable.pRating == currentMax)
                                            {
                                                allMoves.Add(createNewMove(bTile, "firebolt", tInteractable.pRating, target, unit));
                                            }
                                        }
                                        else
                                        {
                                            if (tInteractable.pRating / 10 > currentMax)
                                            {
                                                currentMax = tInteractable.pRating / 10;
                                                allMoves.Clear();
                                                allMoves.Add(createNewMove(bTile, "firebolt", tInteractable.pRating / 10, target, unit));
                                            }
                                            else if (tInteractable.pRating / 10 == currentMax)
                                            {
                                                allMoves.Add(createNewMove(bTile, "firebolt", tInteractable.pRating / 10, target, unit));
                                            }
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach(GameObject target in targetUnits)
                {
                    if (target.name != "Wizard")
                    {
                        Interactable tInteractable = target.GetComponent<Interactable>();
                        foreach(boardTile bTile in allTiles)
                        {
                            if (tInteractable.coord == bTile.pos)
                            {
                                if (tInteractable.health <= unit.GetComponent<fireBoltFunc>().damage)
                                {
                                    if (tInteractable.pRating > currentMax)
                                    {
                                        currentMax = tInteractable.pRating;
                                        allMoves.Clear();
                                        allMoves.Add(createNewMove(bTile, "firebolt", tInteractable.pRating, target, unit));
                                    }
                                    else if (tInteractable.pRating== currentMax)
                                    {
                                        allMoves.Add(createNewMove(bTile, "firebolt", tInteractable.pRating, target, unit));
                                    }
                                } else
                                {
                                    float pDiv = tInteractable.pRating / 1000;
                                    if (pDiv > currentMax)
                                    {
                                        currentMax = pDiv;
                                        allMoves.Clear();
                                        allMoves.Add(createNewMove(bTile, "firebolt", pDiv, target, unit));
                                    }
                                    else if (pDiv == currentMax)
                                    {
                                        allMoves.Add(createNewMove(bTile, "firebolt", pDiv, target, unit));
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            //Flamebomb
            if(currentTurn == "robot" || currentTurn == "wizard") //if current turn is of team that can actually cast flamebomb (make this work for wizards when their ai is implemented)
            {
                Interactable interactable = unit.GetComponent<Interactable>();
                bool suitableFound = false;
                if (interactable.faction == winningTeam)
                {
                    foreach(GameObject target in targetUnits)
                    {
                        if (target.name != "Wizard")
                        {
                            Interactable tInteractable = target.GetComponent<Interactable>();
                            foreach (boardTile bTile in allTiles)
                            {
                                if (tInteractable.coord == bTile.pos)
                                {
                                    foreach (TileBase nonCon in nonConTiles)
                                    {
                                        if (bTile.state == nonCon)
                                        {
                                            suitableFound = true;
                                            if (tInteractable.health <= burningTileDamage) //If it will kill the unit
                                            {
                                                if (tInteractable.pRating > currentMax)
                                                {
                                                    currentMax = tInteractable.pRating;
                                                    allMoves.Clear();
                                                    allMoves.Add(createNewMove(bTile, "flamebomb", tInteractable.pRating, target, unit));
                                                }
                                                else if (tInteractable.pRating == currentMax)
                                                {
                                                    allMoves.Add(createNewMove(bTile, "flamebomb", tInteractable.pRating, target, unit));
                                                }
                                            }
                                            else //Otherwise target strongest
                                            {
                                                float pDiv = tInteractable.pRating / 1000;
                                                if (pDiv > currentMax)
                                                {
                                                    currentMax = pDiv;
                                                    allMoves.Clear();
                                                    allMoves.Add(createNewMove(bTile, "flamebomb", pDiv, target, unit));
                                                }
                                                else if (pDiv == currentMax)
                                                {
                                                    allMoves.Add(createNewMove(bTile, "flamebomb", pDiv, target, unit));
                                                }
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            suitableFound = false;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (!suitableFound)
                    {
                        int valueToAdd = 0; //This will all also need to change when wizards implemented
                        if (aPos == 2)
                        {
                            valueToAdd = rWinCount - aWinCount;
                        } else if (wPos == 2)
                        {
                            valueToAdd = rWinCount - wWinCount;
                        }
                        foreach(Vector3 pos in nonConPos2)
                        {
                            foreach(boardTile bTile in allTiles)
                            {
                                if (bTile.pos == pos && bTile.burning == false)
                                {
                                    if (bTile.inhabitant == null)
                                    {
                                        float newVal = valueToAdd * 1.5f;
                                        if(newVal > currentMax)
                                        {
                                            currentMax = newVal;
                                            allMoves.Clear();
                                            allMoves.Add(createNewMove(bTile, "flamebomb", newVal, null, unit));
                                        } else if (newVal == currentMax)
                                        {
                                            allMoves.Add(createNewMove(bTile, "flamebomb", newVal, null, unit));
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        foreach (Vector3 pos2 in nonConPos1)
                        {
                            foreach (boardTile bTile in allTiles)
                            {
                                if (bTile.pos == pos2 && bTile.burning == false)
                                {
                                    if (bTile.inhabitant == null)
                                    {
                                        if (valueToAdd > currentMax)
                                        {
                                            currentMax = valueToAdd;
                                            allMoves.Clear();
                                            allMoves.Add(createNewMove(bTile, "flamebomb", valueToAdd, null, unit));
                                        }
                                        else if (valueToAdd == currentMax)
                                        {
                                            allMoves.Add(createNewMove(bTile, "flamebomb", valueToAdd, null, unit));
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        } else
        {
            //Hivemind
            if (currentTurn == "alien")
            {
                Interactable interactable = unit.GetComponent<Interactable>();
                if (!interactable.teamBuffed) //Should only be considered if hivemind isn't applied
                {
                    if (aPos != 2)
                    {
                        float totalAlien = 0;
                        float totalSecond = 0;
                        int secondWinCount = 0;
                        float valueToAdd = 0;
                        foreach (GameObject aUnit in alienUnits)
                        {
                            totalAlien = totalAlien + aUnit.GetComponent<Interactable>().pRating;
                        }
                        if (wPos == 2)
                        {
                            secondWinCount = wWinCount;
                            foreach (GameObject wUnit in wizardUnits)
                            {
                                totalSecond = totalSecond + wUnit.GetComponent<Interactable>().pRating;
                            }
                        }
                        if (rPos == 2)
                        {
                            secondWinCount = rWinCount;
                            foreach (GameObject rUnit in robotUnits)
                            {
                                totalSecond = totalSecond + rUnit.GetComponent<Interactable>().pRating;
                            }
                        }
                        if (totalAlien < totalSecond)
                        {
                            print("Alien team isnt strongest");
                            if (winningTeam == "alien" && closeToWinnning)
                            {
                                valueToAdd = (aWinCount - secondWinCount) * 2;
                            }
                            else
                            {
                                valueToAdd = aWinCount - secondWinCount;
                            }
                        }
                        else
                        {
                            valueToAdd = Mathf.Abs((aWinCount - secondWinCount) / 2);
                        }
                        if (valueToAdd > currentMax)
                        {
                            currentMax = valueToAdd;
                            allMoves.Clear();
                            allMoves.Add(createNewMove(allTiles[0], "hivemind", valueToAdd, unit, null));
                        }
                        else if (valueToAdd == currentMax)
                        {
                            allMoves.Add(createNewMove(null, "hivemind", valueToAdd, unit, null));
                        }
                    }
                }
            }
        }
        return currentMax;
    }

    public potentialMove createNewMove(boardTile bTile, string action, float value, GameObject unit, GameObject attacker)
    {
        potentialMove newMove = new potentialMove();
        newMove.bTile = bTile;
        newMove.action = action;
        newMove.moveValue = value;
        newMove.targetPiece = unit;
        newMove.attacker = attacker;
        return newMove;
    }

    public void findUnits()
    {
        units = GameObject.FindGameObjectsWithTag("Unit");
        int listSize = units.Length;
        if (listSize < prevSize)
        {
            prevSize = listSize;
            foreach(GameObject unit in units)
            {
                unit.GetComponent<Interactable>().setInhabitant();
            }
        }
    }

    void SpawnInfo()
    {
        infoPanel.SetActive(true);
        findUnits();
        foreach (GameObject unit in units)
        {
            Interactable interactable = unit.GetComponent<Interactable>();
            if (interactable.coord == new Vector3Int(coordinate.x, coordinate.y, 0))
            {
                unitImage.sprite = interactable.GetComponent<SpriteRenderer>().sprite;
                unitImage.SetNativeSize();
                unitImage.GetComponent<ImageSizeCheck>().checkImage(interactable.colour);
                if (interactable.mainUnit)
                {
                    Crown.enabled = true;
                }
                else
                {
                    Crown.enabled = false;
                }
                healthTarget = interactable.healthPercentage;
                panelYCheck panCheck = infoPanel.GetComponent<panelYCheck>();
                int i = 1;
                panCheck.abilityText.text = "";
                panCheck.abilityName.text = "";
                buttonHover = null;
                panCheck.unitName.text = interactable.name;
                panCheck.attack.text = "Attack: " + interactable.damage;
                panCheck.range.text = "Range: " + interactable.range;
                if (debugMode)
                {
                    panCheck.pRating.enabled = true;
                    panCheck.pRating.text = "pRating: " + interactable.pRating;
                } else
                {
                    panCheck.pRating.text = "";
                }
                panCheck.bi1.sprite = null;
                panCheck.bi2.sprite = null;
                panCheck.bi3.sprite = null;
                panCheck.bi4.sprite = null;
                panCheck.bi1.color = new Color(0f, 0f, 0f, 0f);
                panCheck.bi2.color = new Color(0f, 0f, 0f, 0f);
                panCheck.bi3.color = new Color(0f, 0f, 0f, 0f);
                panCheck.bi4.color = new Color(0f, 0f, 0f, 0f);
                if (interactable.poisoned)
                {
                    panCheck.bi4.sprite = panCheck.poison;
                    panCheck.bi4.color = Color.white;
                }
                if (interactable.tileBuffed)
                {
                    switch (i)
                    {
                        case 1:
                            panCheck.bi1.sprite = panCheck.tileBuff;
                            panCheck.bi1.color = Color.white;
                            i++;
                            break;
                        case 2:
                            panCheck.bi2.sprite = panCheck.tileBuff;
                            panCheck.bi2.color = Color.white;
                            i++;
                            break;
                        case 3:
                            panCheck.bi3.sprite = panCheck.tileBuff;
                            panCheck.bi3.color = Color.white;
                            i++;
                            break;
                    }
                }
                foreach (burningTile tile in burningTiles)
                {
                    if (tile.bTile.pos == interactable.coord)
                    {
                        switch (i)
                        {
                            case 1:
                                panCheck.bi1.sprite = panCheck.fire;
                                panCheck.bi1.color = Color.white;
                                i++;
                                break;
                            case 2:
                                panCheck.bi2.sprite = panCheck.fire;
                                panCheck.bi2.color = Color.white;
                                i++;
                                break;
                            case 3:
                                panCheck.bi3.sprite = panCheck.fire;
                                panCheck.bi3.color = Color.white;
                                i++;
                                break;
                        }
                    }
                }
                switch (interactable.name)
                {
                    case "Berserker":
                        if (interactable.GetComponent<enrageFunc>().enrageApplied)
                        {
                            switch (i)
                            {
                                case 1:
                                    panCheck.bi1.sprite = panCheck.enrage;
                                    panCheck.bi1.color = Color.white;
                                    i++;
                                    break;
                                case 2:
                                    panCheck.bi2.sprite = panCheck.enrage;
                                    panCheck.bi2.color = Color.white;
                                    i++;
                                    break;
                                case 3:
                                    panCheck.bi3.sprite = panCheck.enrage;
                                    panCheck.bi3.color = Color.white;
                                    i++;
                                    break;
                            }
                        }
                        break;
                    case "Sentinel":
                        if (interactable.GetComponent<tankFunc>().tankApplied)
                        {
                            switch (i)
                            {
                                case 1:
                                    panCheck.bi1.sprite = panCheck.tank;
                                    panCheck.bi1.color = Color.white;
                                    i++;
                                    break;
                                case 2:
                                    panCheck.bi2.sprite = panCheck.tank;
                                    panCheck.bi2.color = Color.white;
                                    i++;
                                    break;
                                case 3:
                                    panCheck.bi3.sprite = panCheck.tank;
                                    panCheck.bi3.color = Color.white;
                                    i++;
                                    break;
                            }
                        }
                        break;
                }
            }
        }
    }

    void OnMouseDown()
    {
        camPos = grid.CellToWorld(coordinate);
        if (targetTile != null)
        {
            reached = false;
            foreach (GameObject unit in units)
            {
                if (coordinate == unit.GetComponent<UnitTileCheck>().coordinate)
                {
                    unitSelectedGUI = unit;
                    Interactable interactable = unit.GetComponent<Interactable>();
                    interactable.Invoke("MouseDown", 0f);
                    break;
                }
            }
        }
    }
    void OnMouseUp()
    {
        unitSelectedGUI = null;
        findUnits();
        foreach (GameObject unit in units)
        {
            unit.GetComponent<SpriteRenderer>().color = unit.GetComponent<Interactable>().colour;
        }
    }
    void CameraMovement()
    {
        if (camState == "in")
        {
            if (reached == false)
            {
                float newXPos = Mathf.SmoothDamp(camera.transform.position.x, camPos.x, ref velocity.x, smoothTime);
                float newYPos = Mathf.SmoothDamp(camera.transform.position.y, camPos.y, ref velocity.y, smoothTime);
                camera.transform.position = new Vector3(newXPos, newYPos, -500);
            }
            if (camera.transform.position == camPos)
            {
                reached = true;
            }
        }
        else
        {
            camera.transform.position = new Vector3 (boardCentre.x, boardCentre.y + 0.25f, boardCentre.z);

            reached = true;
        }
    }
    void OnEnable()
    {
        findUnits();
    }

    //Creates a new burningTile object and adds it to the list
    public void newBurnTile(Vector3Int pos, int endPoint)
    {
        burningTile newTile = new burningTile();
        foreach (boardTile bTile in allTiles)
        {
            if (bTile.pos == pos)
            {
                newTile.bTile = bTile;
                newTile.bTile.burning = true;
                newTile.bTile.currentVal = newTile.bTile.currentVal - 500;
            }
        }
        newTile.endPoint = endPoint;
        GameObject flame = Instantiate(flameIcon);
        flame.transform.parent = sceneMaster.transform;
        newTile.icon = flame;
        newTile.icon.transform.position = grid.CellToWorld(pos);
        burningTiles.Add(newTile);
    }

    public void checkTurn()
    {
        if (currentTurn == "wizard" && wDestroy)
        {
            currentTurn = "robot";
        }
        if (currentTurn == "alien" && aDestroy)
        {
            currentTurn = "wizard";
        }
        if (currentTurn == "robot" && rDestroy)
        {
            currentTurn = "alien";
        }
        if (wDestroy)
        {
            closeToWinnning = false;
            foreach (GameObject unit in wizardUnits)
            {
                Destroy(unit);
            }
        }
        if (aDestroy)
        {
            closeToWinnning = false;
            foreach (GameObject unit in alienUnits)
            {
                Destroy(unit);
            }
        }
        if (rDestroy)
        {
            closeToWinnning = false;
            foreach (GameObject unit in robotUnits)
            {
                Destroy(unit);
            }
        }
        blockedTiles.Clear();
        findUnits();
        foreach (GameObject unit in units)
        {
            Interactable interactable = unit.GetComponent<Interactable>();
            interactable.AddBlock();
            interactable.setPRating();
            interactable.setInhabitant();
        }
        findUnits();
        foreach (Vector3Int tile in nonConPos1)
        {
            foreach(GameObject unit in units)
            {
                Interactable interactable = unit.GetComponent<Interactable>();
                if(interactable.coord == tile)
                {
                    switch (interactable.faction)
                    {
                        case "wizard":                    
                            wWinCount++;
                            break;
                        case "alien":
                            aWinCount++;
                            break;
                        case "robot":
                            rWinCount++;
                            break;
                    }
                }
            }
        }
        foreach (Vector3Int tile in nonConPos2)
        {
            foreach (GameObject unit in units)
            {
                Interactable interactable = unit.GetComponent<Interactable>();
                if (interactable.coord == tile)
                {
                    switch (interactable.faction)
                    {
                        case "wizard":
                            wWinCount = wWinCount + 2;
                            break;
                        case "alien":
                            aWinCount = aWinCount + 2;
                            break;
                        case "robot":
                            rWinCount = rWinCount + 2;
                            break;
                    }
                }
            }
        }
        if (aWinCount > wWinCount && aWinCount > rWinCount)
        {
            winningTeam = "alien";
            aPos = 1;
            wPos = 2;
            rPos = 2; 
        } else if (wWinCount > aWinCount && wWinCount > rWinCount)
        {
            winningTeam = "wizard";
            wPos = 1;
            aPos = 2;
            rPos = 2;
        } else if (rWinCount > aWinCount && rWinCount > wWinCount)
        {
            winningTeam = "robot";
            rPos = 1;
            aPos = 2;
            wPos = 2;
        }
        if (aWinCount < wWinCount && aWinCount < rWinCount)
        {
            aPos = 3;
        } else if (wWinCount < aWinCount && wWinCount < rWinCount)
        {
            wPos = 3;
        } else if (rWinCount < aWinCount && rWinCount < wWinCount)
        {
            rPos = 3;
        }
        foreach(GameObject unit in units)
        {
            Interactable interactable = unit.GetComponent<Interactable>();
            if (interactable.faction == winningTeam)
            {
                interactable.onWinnningTeam = true;
            } else
            {
                interactable.onWinnningTeam = false;
            }
        }
        if (!closeToWinnning) //Checks if a team is close to winning
        {
            if (wWinCount >= (countNeeded - (countNeeded / 5)))
            {
                closeToWinnning = true;
            }
            else if (aWinCount >= (countNeeded - (countNeeded / 5)))
            {
                closeToWinnning = true;
            }
            else if (aWinCount >= (countNeeded - (countNeeded / 5)))
            {
                closeToWinnning = true;
            }
        }
        wizardS.value = wWinCount;
        alienS.value = aWinCount;
        robotS.value = rWinCount;
        calcBurningTile();  
    }
    void calcBurningTile()
    {
        if (burningTiles.Count > 0)
        {
            burningTile tileToRemove = null;
            foreach (burningTile tile in burningTiles)
            {
                if (tile.endPoint >= turnCount)
                {
                    if (tile.bTile.inhabitant != null)
                    {
                        Interactable interactable = tile.bTile.inhabitant.GetComponent<Interactable>();
                        interactable.health = interactable.health - burningTileDamage;
                    }

                }
                else
                {
                    tileToRemove = tile;
                }
            }
            burningTiles.Remove(tileToRemove);
            if (tileToRemove != null)
            {
                tileToRemove.bTile.currentVal = tileToRemove.bTile.currentVal + 500;
                tileToRemove.bTile.burning = false;
                Destroy(tileToRemove.icon);
            }
        }
    }
    public void newLogEntry(boardTile bTile, string action, GameObject target, GameObject attacker)
    {
        GameObject LogText = Instantiate(logTextPrefab, content.transform);
        Text lt = LogText.GetComponent<Text>();
        LogInfo info = lt.GetComponent<LogInfo>();
        info.attacker = attacker;
        info.target = target;
        info.bTile = bTile;
        info.action = action;
        if(action == "movement")
        {
            lt.text = target.name + " on team " + target.GetComponent<Interactable>().faction + " moved to position " + bTile.pos;
        }
        if (action == "firebolt")
        {
            lt.text = attacker.name + " on team " + attacker.GetComponent<Interactable>().faction + " shot a firebolt at " + target.name + " on team " + target.GetComponent<Interactable>().faction + " for " + attacker.GetComponent<fireBoltFunc>().damage + " damage";
        }
        if (action == "flamebomb")
        {
            lt.text = attacker.name + " on team " + attacker.GetComponent<Interactable>().faction + " set " + bTile.pos + " on fire";
        }
        if (action == "hivemind")
        {
            lt.text = target.name + " on team " + target.GetComponent<Interactable>().faction + " activated the hivemind";
        }

        RectTransform rect = content.GetComponent<RectTransform>();
        if(rect.rect.height - lt.GetComponent<RectTransform>().rect.height > 200)
        {
            //rect.SetPositionAndRotation(new Vector3(rect.position.x, rect.position.y + lt.GetComponent<RectTransform>().rect.height, rect.position.z), rect.rotation);
        }
    }
    public void removeBurningTile(burningTile oldBurnTile)
    {
        burningTiles.Remove(oldBurnTile);
        oldBurnTile.bTile.currentVal = oldBurnTile.bTile.currentVal + 500;
        oldBurnTile.bTile.burning = false;
        Destroy(oldBurnTile.icon);
    }
}