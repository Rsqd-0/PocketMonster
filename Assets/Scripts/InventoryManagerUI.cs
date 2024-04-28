using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InventoryManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private List<ItemSlotUI> itemList;
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

    void UpdateItemList()
    {
        //l'update est lancée quand on ajoute un obj, ou quand on utilise un obj
    }

    public void HandleUpdate()
    {
        int prevSelection = selectedItem;

        if (Input.GetKeyDown(KeyCode.DownArrow)) ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Items.Count - 1);

        if (prevSelection != selectedItem) UpdateItemSelection();
        //if (Input.GetKeyDown(KeyCode.X)) onBack?.Invoke();
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
}
