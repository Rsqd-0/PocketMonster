using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    void Start()
    {
        //GetComponent<Game>().AddOnPauseListener(ToggleSelf);
    }
    
    private void ToggleSelf(bool active)
    {
        gameObject.SetActive(active);
    }

    private void QuitButton()
    {
        Application.Quit();
    }
}
