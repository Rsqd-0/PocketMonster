using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField] private GameObject pokemonPosition;
    [SerializeField] private InventoryManagerUI inventoryManagerUI;
    [SerializeField] private GameObject overworld;

    [SerializeField] private List<Spawner> spawners;
    [SerializeField] private List<String> spawnable;
    private bool cursor;

    public static Game Instance;

    private void Awake()
    {
        SaveData.SetPokemonPosition(pokemonPosition.transform.position);
        for (int i=0; i<spawners.Count;i++)
        {
            StartCoroutine(spawners[i].StartSpawn(spawnable[i]));
        }
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
    
    public void EndGame()
    {
        
    }
}
