using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemSlot> items;
}

[Serializable]
public class ItemSlot
{
    [SerializeField] private ItemSO item;
    [SerializeField] private int count;

    public ItemSO Item => item;
    public int Count => count;
}
