using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InventoryManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject parent2;
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private GameObject pokemonMenu;
    
    [SerializeField] private List<ItemSlotUI> itemList = new List<ItemSlotUI>();
    [SerializeField] private ItemSlotUI itemSlotUI;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text description;
    
    private List<PokemonSlotUI> pokemonList = new List<PokemonSlotUI>();
    [SerializeField] private PokemonSlotUI pokemonSlotUI;
    [SerializeField] private Image pokemonIcon;
    [SerializeField] private TMP_Text characteristics;

    [SerializeField] private TMP_Dropdown heals;

    [SerializeField] private Transform playerStation;
    private Vector3 pokemonBasePosition;
    
    private int selectedItem = 0;
    private int selectedPokemon = 0;
    private int currentPokemon = 0;
    private Inventory inventory;
    private bool inventoryOpened;
    private bool pokemonOpened;
    
    public bool inBattle;

    void Awake()
    {
        inventory = Inventory.GetInventory();
        pokemonBasePosition = playerStation.position;
    }

    private void Start()
    {
        CreateItemList();
        pokemonList[currentPokemon].NameUI.fontStyle = FontStyles.Underline;
        UpdateDropdown();
    }

    private void Update()
    {
        HandleUpdate();
    }

    void CreateItemList()
    {
        foreach (var item in inventory.Items)
        {
            var slotObj = Instantiate(itemSlotUI, parent.transform);
            slotObj.gameObject.SetActive(true);
            slotObj.Set(item);
            itemList.Add(slotObj);
        }
        UpdateItemSelection();
        foreach (var pokemon in inventory.Pokemons)
        {
            pokemon.gameObject.transform.position = pokemonBasePosition;
            var slotPkm = Instantiate(pokemonSlotUI, parent2.transform);
            slotPkm.gameObject.SetActive(true);
            slotPkm.Set(pokemon);
            pokemonList.Add(slotPkm);
        }
        UpdatePokemonSelection();
    }

    public void UpdateItemList()
    {
        itemList.Clear();
        foreach (Transform child in parent.transform)
            Destroy(child.gameObject);
        foreach (var item in inventory.Items)
        {
            var slotObj = Instantiate(itemSlotUI, parent.transform);
            slotObj.gameObject.SetActive(true);
            slotObj.Set(item);
            itemList.Add(slotObj);
        }
        UpdateItemSelection();
    }

    public void UpdatePokemonList()
    {
        if (inventory.Pokemons[currentPokemon].currentHp == 0)
        {
            for (int i = 0; i < inventory.Pokemons.Count; i++)
            {
                if (inventory.Pokemons[i].currentHp > 0)
                {
                    SetCurrentPokemon(i);
                    break;
                }
            }
            Game.Instance.gameOver();
        }
        pokemonList.Clear();
        foreach (Transform child in parent2.transform)
            Destroy(child.gameObject);
        foreach (var pokemon in inventory.Pokemons)
        {
            pokemon.gameObject.transform.position = pokemonBasePosition;
            var slotPkm = Instantiate(pokemonSlotUI, parent2.transform);
            slotPkm.gameObject.SetActive(true);
            slotPkm.Set(pokemon);
            pokemonList.Add(slotPkm);
        }
        UpdatePokemonSelection();
    }

    public void UpdateDropdown()
    {
        List<ItemSlot> potions = inventory.Items.Where(slot => slot.Item is PotionSO && slot.Count > 0).ToList();
        
        List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();
        foreach (ItemSlot potionSlot in potions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(potionSlot.Item.name);
            dropdownOptions.Add(option);
        }
        
        heals.ClearOptions();
        heals.AddOptions(dropdownOptions);
    }

    public void HandleUpdate()
    {
        if (Input.GetKey(KeyCode.I) && !inBattle)
        {
            inventoryOpened = true;
            pokemonOpened = false;
        }

        if (Input.GetKey(KeyCode.P) && !inBattle)
        {
            pokemonOpened = true;
            inventoryOpened = false;
        }

        if (inventoryOpened || pokemonOpened)
        {
            Time.timeScale = 0f;
            Game.CursorVisible();
        }
        if (inventoryOpened)
        {
            pokemonOpened = false;
            inventoryMenu.SetActive(true);
            int prevSelection = selectedItem;

            if (Input.GetKeyDown(KeyCode.DownArrow)) ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.UpArrow)) --selectedItem;

            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Items.Count - 1);

            if (prevSelection != selectedItem) UpdateItemSelection();
            //if (Input.GetKeyDown(KeyCode.X)) onBack?.Invoke();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Quit();
                inventoryOpened = false;
            }
        }
        else
        {
            inventoryMenu.SetActive(false);
        }

        if (pokemonOpened)
        {
            inventoryOpened = false;
            pokemonMenu.SetActive(true);
            int prevSelection = selectedPokemon;

            if (Input.GetKeyDown(KeyCode.DownArrow)) ++selectedPokemon;
            else if (Input.GetKeyDown(KeyCode.UpArrow)) --selectedPokemon;

            selectedPokemon = Mathf.Clamp(selectedPokemon, 0, inventory.Pokemons.Count - 1);

            if (prevSelection != selectedPokemon) UpdatePokemonSelection();
            //if (Input.GetKeyDown(KeyCode.X)) onBack?.Invoke();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Quit();
                pokemonOpened = false;
            } 
            if (Input.GetKeyDown(KeyCode.Return)) CurrentPokemon();
        }
        else
        {
            
            pokemonMenu.SetActive(false);
        }
    }

    private void Quit()
    {
        Time.timeScale = 1f;
        Game.CursorInvisible(); 
    }

    void UpdateItemSelection()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (i == selectedItem) itemList[i].NameUI.color = Color.blue;
            else itemList[i].NameUI.color = Color.white;
        }

        var slot = inventory.Items[selectedItem];
        itemIcon.sprite = slot.Item.icon;
        description.text = slot.Item.description;
    }
    void UpdatePokemonSelection()
    {
        for (int i = 0; i < pokemonList.Count; i++)
        {
            if (i == selectedPokemon) pokemonList[i].NameUI.color = Color.blue;
            else pokemonList[i].NameUI.color = Color.white;
        }
        pokemonList[currentPokemon].NameUI.fontStyle = FontStyles.Underline;
        var slot = inventory.Pokemons[selectedPokemon];
        //itemIcon.sprite = slot;
        characteristics.text = "Level: " + slot.lvl + "\nHP: " + slot.currentHp + " / " + slot.maxHp + "\n\nAttack: " + slot.atk + "\nDefense: " + slot.def + "\nSpeed: " +
                           slot.spd;
    }

    public void CurrentPokemon()
    {
        SetCurrentPokemon(selectedPokemon);
    }

    public void SetCurrentPokemon(int index)
    {
        currentPokemon = index != selectedPokemon ? index : selectedPokemon;
        inventory.SetCurrentPokemon(currentPokemon);
        foreach (var names in pokemonList)
        {
            names.NameUI.fontStyle = FontStyles.Normal;
        }
        UpdatePokemonList();
    }

    public void Throw()
    {
        inventory.ModifyItem(inventory.Items[selectedItem].Item,-1);
        UpdateItemList();
    }
    
    public void UseSelectedHeal()
    {
        int selectedIndex = heals.value;
        List<ItemSlot> potions = inventory.Items.Where(slot => slot.Item is PotionSO && slot.Count > 0).ToList();

        if (selectedIndex >= 0 && selectedIndex < potions.Count)
        {
            PotionSO potion = (PotionSO)potions[selectedIndex].Item;
            if (inventory.Pokemons[selectedPokemon].currentHp == 0 && !potion.revive) return;
            inventory.ModifyItem(potion,-1);
            inventory.Pokemons[selectedPokemon].Heal(potion.hpAmount);
            UpdateItemList();
            UpdatePokemonList();
            UpdateDropdown();
        }
    }

    public void FreeCreature()
    {
        if (inventory.Pokemons.Count != 1)
        {
            inventory.Pokemons.Remove(inventory.Pokemons[selectedPokemon]);
            UpdatePokemonList();
            UpdatePokemonSelection();
        }
    }
}
