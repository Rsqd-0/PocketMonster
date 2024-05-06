using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField] private InventoryManagerUI inventoryManagerUI;
    [SerializeField] private GameObject overworld;

    [SerializeField] private Spawner spawnerAreaA;
    private UnityEvent<bool> onPause = new UnityEvent<bool>();
    private bool cursor;

    public static Game Instance;

    private void Awake()
    {
        StartCoroutine(spawnerAreaA.StartSpawn("areaA"));
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public static void CursorVisible()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void CursorInvisible()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void FightScene(GameObject pokemon)
    {
        PokemonOverworld pO = pokemon.GetComponentInChildren<PokemonOverworld>();
        pO.StopMovement();
        pO.enabled = false;
        SaveData.SaveEnemyData(pokemon.gameObject.transform.parent.gameObject);
        SaveData.SetInventoryUI(inventoryManagerUI);
        SceneManager.LoadScene("Fight", LoadSceneMode.Additive);
        //overworld.SetActive(false);
    }
    
    public void AddOnPauseListener(UnityAction<bool> listener)
    {
        onPause.AddListener(listener);
    }

    public void EndGame()
    {
        
    }
}
