using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class resetOnTimer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(one());
        SceneManager.LoadScene("Hexboard");
    }

    IEnumerator one()
    {
        yield return new WaitForSeconds(1);
    }
}
