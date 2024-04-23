using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
    private UnityEvent<bool> onPause = new UnityEvent<bool>();

    public static Game Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0f)
            {
                Time.timeScale = 1f;
                onPause.Invoke(false);
            }
            else
            {
                Time.timeScale = 0f;
                onPause.Invoke(true);
            }
        }
    }

    public void AddOnPauseListener(UnityAction<bool> listener)
    {
        onPause.AddListener(listener);
    }

    public void EndGame()
    {
        
    }
}
