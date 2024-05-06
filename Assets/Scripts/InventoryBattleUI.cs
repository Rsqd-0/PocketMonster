using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Time = UnityEngine.Time;

public class InventoryBattleUI : MonoBehaviour
{
    [SerializeField] private BattleSystem battle;
    [SerializeField] private BattleHUD battleUI;
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject inventoryMenu;
    
    [SerializeField] private List<ItemSlotUI> itemList = new List<ItemSlotUI>();
    [SerializeField] private ItemSlotUI itemSlotUI;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text description;

    private int selectedItem = 0;
    private Inventory inventory;

    void Awake()
    {
        inventory = Inventory.GetInventory();
    }

    private void Start()
    {
        CreateItemList();
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

    public void HandleUpdate()
    {
            int prevSelection = selectedItem;

            if (Input.GetKeyDown(KeyCode.DownArrow)) ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.UpArrow)) --selectedItem;

            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Items.Count - 1);

            if (prevSelection != selectedItem) UpdateItemSelection();
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

    public void UseItem()
    {
        bool itemUsed = false;
        if (inventory.Items[selectedItem].Item is PotionSO && inventory.Items[selectedItem].Count > 0)
        {
            PotionSO potion = (PotionSO)inventory.Items[selectedItem].Item;
            inventory.ModifyItem(potion,-1);
            inventory.GetCurrentPokemon().Heal(potion.hpAmount);
            battleUI.SetHP(inventory.GetCurrentPokemon().currentHp);
            UpdateItemList();
            itemUsed = true;
        }

        if (inventory.Items[selectedItem].Item is PokeballSO && inventory.Items[selectedItem].Count > 0)
        {
            if (inventory.Pokemons.Count == 6)
            {
                
            }
            else
            {
                PokeballSO pokeball = (PokeballSO)inventory.Items[selectedItem].Item;
                Unit pokemonToAdd = battle.CapturePokemon(pokeball);
                if (pokemonToAdd != null) inventory.AddToPokemon(pokemonToAdd);
                inventory.ModifyItem(pokeball, -1);
                UpdateItemList();
                itemUsed = true;
            }
        }

        if (itemUsed)
        {
            StartCoroutine(battle.PlayerItem());
        }
    }
}
