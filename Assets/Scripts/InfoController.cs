using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoController : MonoBehaviour
{
    public GameObject scorePan, logPan, settingsPan;
    public Button score, log, settings;
    bool scoreActive, logActive, settingsActive;
    // Start is called before the first frame update
    void Start()
    {
             
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void checkButton(Button btn)
    {
        if(btn == score)
        {
            scoreActive = true;
            logActive = false;
            settingsActive = false;
        } else if (btn == log)
        {
            logActive = true;
            settingsActive = false;
            scoreActive = true;
        } else if (btn == settings)
        {
            settingsActive = true;
            logActive = true;
            scoreActive = false;
        }
        if (scoreActive)
        {
            scorePan.transform.SetAsLastSibling();
        }
        if (logActive)
        {
            logPan.transform.SetAsLastSibling();
        }
        if (settingsActive)
        {
            settingsPan.transform.SetAsLastSibling();
        }
    }
}
