using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;

    private Inventory inventory;

    void Awake()
    {
        inventory = Inventory.GetInventory();
    }

    private void Start()
    {
        UpdateItemList();
    }

    void UpdateItemList()
    {
        //pour tous les enfants de itemList, il faut soit update le count (si l'obj est déjà là) soit créer l'objet
        
    }
}
