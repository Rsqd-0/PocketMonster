using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private InventoryManagerUI inventoryManagerUI;
    [SerializeField] private List<ItemSlot> items;
    [SerializeField] private List<Unit> pokemons;
    [SerializeField] private int currentPokemon;

    public List<ItemSlot> Items => items;
    public List<Unit> Pokemons => pokemons;
    public int CurrentPokemon => currentPokemon;
    
    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerMovement>().GetComponent<Inventory>();
    }
    public void SetCurrentPokemon(int index)
    {
        currentPokemon = index;
    }

    public Unit GetCurrentPokemon()
    {
        return pokemons[currentPokemon];
    }

    public void AddToInventory(ItemSO itemToAdd)
    {
        ModifyItem(itemToAdd,1);
        if (inventoryManagerUI != null) inventoryManagerUI.UpdateItemList();
    }
    
    public void ModifyItem(ItemSO item, int quantityToAdd)
    {
        ItemSlot itemSlot = items.FirstOrDefault(slot => slot.Item == item);
        if (itemSlot == null)
        {
            itemSlot = new ItemSlot(item, 0);
            items.Add(itemSlot);
        }

        itemSlot.Count += quantityToAdd;
        if (itemSlot.Count < 0) itemSlot.Count = 0;
    }

    public void AddToPokemon(Unit pokemon)
    {
        pokemons.Add(pokemon);
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] private ItemSO item;
    [SerializeField] private int count;

    public ItemSO Item
    {
        get => item;
        set => item = value;
    }
    public int Count
    {
        get => count;
        set => count = value;
    }

    public ItemSlot(ItemSO item, int count)
    {
        this.item = item;
        this.count = count;
    }
}
