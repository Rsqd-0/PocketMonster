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
    [SerializeField] private PannelUI pannelUI;
    
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
    
    private int selectedItem = 0;
    private int selectedPokemon = 0;
    private int currentPokemon = 0;
    private Inventory inventory;
    private bool inventoryOpened;
    private bool pokemonOpened;

    void Awake()
    {
        inventory = Inventory.GetInventory();
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
        pokemonList.Clear();
        foreach (Transform child in parent2.transform)
            Destroy(child.gameObject);
        foreach (var pokemon in inventory.Pokemons)
        {
            var slotPkm = Instantiate(pokemonSlotUI, parent2.transform);
            slotPkm.gameObject.SetActive(true);
            slotPkm.Set(pokemon);
            pokemonList.Add(slotPkm);
        }
        UpdatePokemonSelection();
    }

    void UpdateDropdown()
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
        if (Input.GetKey(KeyCode.I))
        {
            inventoryOpened = true;
            pokemonOpened = false;
        }

        if (Input.GetKey(KeyCode.P))
        {
            pokemonOpened = true;
            inventoryOpened = false;
        }
        if (inventoryOpened || pokemonOpened) pannelUI.gameObject.SetActive(false);
        if (!inventoryOpened && !pokemonOpened) pannelUI.gameObject.SetActive(true);
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
            if (Input.GetKeyDown(KeyCode.Escape)) inventoryOpened = false;
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
            if (Input.GetKeyDown(KeyCode.Escape)) pokemonOpened = false; 
            if (Input.GetKeyDown(KeyCode.Return)) CurrentPokemon();
        }
        else
        {
            pokemonMenu.SetActive(false);
        }
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

        var slot = inventory.Pokemons[selectedPokemon];
        //itemIcon.sprite = slot;
        characteristics.text = "HP: " + slot.hp + "\nAttack: " + slot.attack + "\nDefense: " + slot.defense + "\nSpeed: " +
                           slot.speed;
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
        pokemonList[currentPokemon].NameUI.fontStyle = FontStyles.Underline;
        pannelUI.UpdatePokemon();
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
            ItemSO potion = potions[selectedIndex].Item;
            Debug.Log("Potion utilisée : " + potion.name);
            inventory.ModifyItem(potion,-1);
            UpdateItemList();
            UpdateDropdown();
        }
    }
}
