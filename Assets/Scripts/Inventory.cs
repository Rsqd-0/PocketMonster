using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemSlot> items;
    [SerializeField] private List<PokemonSO> pokemons;
    [SerializeField] private int currentPokemon;

    public List<ItemSlot> Items => items;
    public List<PokemonSO> Pokemons => pokemons;
    public int CurrentPokemon => currentPokemon;
    
    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerMovement>().GetComponent<Inventory>();
    }
    public void SetCurrentPokemon(int index)
    {
        currentPokemon = index;
    }

    public PokemonSO GetCurrentPokemon()
    {
        return pokemons[currentPokemon];
    }
    
    public void ModifyItem(ItemSO item, int quantityToAdd)
    {
        ItemSlot itemSlot = items.FirstOrDefault(slot => slot.Item == item);
        if (itemSlot == null)
        {
            itemSlot = new ItemSlot(item, 1);
            items.Add(itemSlot);
        }

        itemSlot.Count += quantityToAdd;
        if (itemSlot.Count < 0) itemSlot.Count = 0;
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
