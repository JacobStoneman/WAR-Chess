using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenuSpawner : MonoBehaviour {

    public static RadialMenuSpawner ins;
    public RadialMenu menuPrefab;
    public GameObject infoPanel;
	Vector3 pos;

    void Awake()
    {
        ins = this;
        infoPanel.SetActive(true);
    }
	public void SpawnMenu(Interactable obj)
    {
        RadialMenu newMenu = Instantiate(menuPrefab) as RadialMenu;
		newMenu.transform.SetParent(transform, false);
		newMenu.SpawnButtons(obj);
    }
}
