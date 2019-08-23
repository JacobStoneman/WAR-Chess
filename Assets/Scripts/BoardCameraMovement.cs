using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoardCameraMovement : MonoBehaviour {
    public class burningTile
    {
        public Vector3Int pos;
        public int endPoint;
        public GameObject icon;
        public burningTile()
        {
            pos = new Vector3Int(0, 0, 0);
            endPoint = 0;
            icon = null;
        }
    }
    public GameObject flameIcon;
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
    // Use this for initialization
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        currentTurn = "alien";
        camera = GameObject.Find("Main Camera");
        selector = GameObject.Find("Selection");
        healthBar = GameObject.Find("HealthBar");
        infoPanel = GameObject.Find("InfoPanel");
        healthBarFill = healthBar.GetComponent<Slider>();
        units = GameObject.FindGameObjectsWithTag("Unit");
        zoomButton = GameObject.Find("ZoomButton");
        healthTarget = 100;
        healthBarFill.value = 100;
        foreach (Vector3Int tile in tilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int tileToAdd = new Vector3Int();
            TileBase curTile = tilemap.GetTile(tile);
            int i = 0;
            foreach (TileBase tile2 in nonConTiles)
            {
                if (curTile == tile2)
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
                    }
                }
                i++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(lossCount >= 2)
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
        if(wWinCount >= 100)
        {
            StaticDataScript.gameWinner = "Wizards";
            SceneManager.LoadScene("EndScreen");
        }
        else if (aWinCount >= 100)
        {
            StaticDataScript.gameWinner = "Aliens";
            SceneManager.LoadScene("EndScreen");
        }
        else if (rWinCount >= 100)
        {
            StaticDataScript.gameWinner = "Robots";
            SceneManager.LoadScene("EndScreen");
        }
        //// Turn switch on key press, remove at some point //////////////////////////////////
        if (Input.GetKeyDown("r"))
        {
            GameObject.FindGameObjectWithTag("Unit").GetComponent<Interactable>().turnSwitch();
        }
        //////////////////////////////////////////////////////////////////////////////////////
        turnText.text = "Turn: " + currentTurn;
        camState = zoomButton.GetComponent<BoardZoom>().cameraState;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        coordinate = grid.WorldToCell(mouseWorldPos);
        targetTile = tilemap.GetTile(new Vector3Int(coordinate.x,coordinate.y,0));
        if (targetTile != null)
        {
            selector.SetActive(true);
            selector.transform.position = new Vector3(grid.CellToWorld(coordinate).x, grid.CellToWorld(coordinate).y, -1);
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

    void SpawnInfo()
    {
        infoPanel.SetActive(true);
        units = GameObject.FindGameObjectsWithTag("Unit");
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
                panCheck.bi1.sprite = null;
                panCheck.bi2.sprite = null;
                panCheck.bi3.sprite = null;
                panCheck.bi1.color = new Color(0f, 0f, 0f, 0f);
                panCheck.bi2.color = new Color(0f, 0f, 0f, 0f);
                panCheck.bi3.color = new Color(0f, 0f, 0f, 0f);
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
                    if (tile.pos == interactable.coord)
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
        units = GameObject.FindGameObjectsWithTag("Unit");
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
            camera.transform.position = boardCentre;
            reached = true;
        }
    }
    void OnEnable()
    {
        units = GameObject.FindGameObjectsWithTag("Unit");
    }

    public void newBurnTile(Vector3Int pos, int endPoint)
    {
        burningTile newTile = new burningTile();
        newTile.pos = pos;
        newTile.endPoint = endPoint;
        GameObject flame = Instantiate(flameIcon);
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
            foreach (GameObject unit in wizardUnits)
            {
                Destroy(unit);
            }
        }
        if (aDestroy)
        {
            foreach (GameObject unit in alienUnits)
            {
                Destroy(unit);
            }
        }
        if (rDestroy)
        {
            foreach (GameObject unit in robotUnits)
            {
                Destroy(unit);
            }
        }
        blockedTiles.Clear();
        units = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject unit in units)
        {
            unit.GetComponent<Interactable>().AddBlock();
        }
        units = GameObject.FindGameObjectsWithTag("Unit");
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
        wizardS.value = wWinCount;
        alienS.value = aWinCount;
        robotS.value = rWinCount;
        print("Wizards: " + wWinCount + ", Aliens: " + aWinCount + ", Robots: " + rWinCount);
        if (burningTiles.Count > 0)
        {
            burningTile tileToRemove = null;
            foreach (burningTile tile in burningTiles)
            {
                if (tile.endPoint >= turnCount)
                {
                    foreach (GameObject unit in units)
                    {
                        Interactable interactable = unit.GetComponent<Interactable>();
                        if (interactable.coord == tile.pos)
                        {
                            interactable.health = interactable.health - 30;
                        }
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
                Destroy(tileToRemove.icon);
            }
        }
    }
}
