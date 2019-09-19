using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RadialMenu : MonoBehaviour {

    public RadialButton buttonPrefab;
    public RadialButton selected;
    Interactable interactable;

	public void SpawnButtons (Interactable obj) {
        StartCoroutine(AnimateButtons(obj));
	}

    IEnumerator AnimateButtons(Interactable obj)
    {
        interactable = obj.GetComponent<Interactable>(); //This might break stuff in the future I have no idea :/
        for (int i = 0; i < obj.options.Length; i++)
        {
            RadialButton newButton = Instantiate(buttonPrefab) as RadialButton;
            newButton.transform.SetParent(transform, false);
            float theta = (2 * Mathf.PI / obj.options.Length) * i;
            float xPos = Mathf.Sin(theta);
            float YPos = Mathf.Cos(theta);
            newButton.transform.localPosition = new Vector3(xPos, YPos, 0f) * 125f;
            newButton.circle.color = obj.options[i].colour;
            newButton.icon.sprite = obj.options[i].sprite;
            newButton.title = obj.options[i].title;
            newButton.func = obj.options[i].func;
            newButton.menu = this;
            newButton.Anim();
            yield return new WaitForSeconds(0.06f);
        }
    }

	void Update(){
		if (Input.GetMouseButtonUp(0)){
            if (selected)
            {
                selected.Invoke(selected.func,0f);
            } else
            {
                interactable.unitSelected = false;
            }
			Destroy (gameObject);
		}

	}
}