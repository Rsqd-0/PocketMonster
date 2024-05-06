using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Time = UnityEngine.Time;

public class InventoryBattleUI : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject inventoryMenu;
    
    [SerializeField] private List<ItemSlotUI> itemList = new List<ItemSlotUI>();
    [SerializeField] private ItemSlotUI itemSlotUI;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text description;

    private int selectedItem = 0;
    private Inventory inventory;
    private bool inventoryOpened=true;

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
        if (inventoryOpened)
        {
            inventoryMenu.SetActive(true);
            int prevSelection = selectedItem;

            if (Input.GetKeyDown(KeyCode.DownArrow)) ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.UpArrow)) --selectedItem;

            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Items.Count - 1);

            if (prevSelection != selectedItem) UpdateItemSelection();
        }
        else
        {
            inventoryMenu.SetActive(false);
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

    public void UseItem()
    {
        if (inventory.Items[selectedItem].Item is PotionSO && inventory.Items[selectedItem].Count > 0)
        {
            PotionSO potion = (PotionSO)inventory.Items[selectedItem].Item;
            inventory.ModifyItem(potion,-1);
            inventory.GetCurrentPokemon().Heal(potion.hpAmount);
        }

        if (inventory.Items[selectedItem].Item is PokeballSO && inventory.Items[selectedItem].Count > 0)
        {
            
        }
    }
}
