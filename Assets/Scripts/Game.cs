using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField] private GameObject characterPosition;
    [SerializeField] private InventoryManagerUI inventoryManagerUI;

    [SerializeField] private List<Spawner> spawners;
    [SerializeField] private List<String> spawnable;
    
    [SerializeField] private AudioSource overworldMusic;
    [SerializeField] private AudioSource fightMusic;
    [SerializeField] private AudioSource bossMusic;
    [SerializeField] private GameObject pauseMenu;
    
    private bool cursor;

    public static Game Instance;

    private void Awake()
    {
        fightMusic.Stop();
        bossMusic.Stop();
        SaveData.SetCharacter(FindObjectOfType<PlayerMovement>());
        SaveData.SetCharacterPosition(characterPosition.transform.position);
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

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
            if (pauseMenu.activeSelf)
            {
                CursorVisible();
            }
            else
            {
                CursorInvisible();
            }
            if (inventoryManagerUI.inBattle)
            {
                CursorVisible();
            }
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
        inventoryManagerUI.UpdatePokemonList();
        PokemonOverworld pO = pokemon.GetComponentInChildren<PokemonOverworld>();
        pO.StopMovement();
        pO.enabled = false;
        if (pO.CompareTag("Mob"))
        {
            overworldMusic.Stop();
            fightMusic.Play();
        }
        else
        {
            overworldMusic.Stop();
            bossMusic.Play();
        }
        SaveData.SaveEnemyData(pokemon.gameObject.transform.parent.gameObject);
        SaveData.SetInventoryUI(inventoryManagerUI);
        SceneManager.LoadScene("Fight", LoadSceneMode.Additive);
    }
    
    public static void overworldMusicPlay()
    {
        Instance.fightMusic.Stop();
        Instance.bossMusic.Stop();
        Instance.overworldMusic.Play();
    }
    
    public void gameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
    
}
