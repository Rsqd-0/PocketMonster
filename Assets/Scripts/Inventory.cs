using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemSlot> items;
    [SerializeField] private List<PokemonSO> pokemons;
    [SerializeField] private GameObject currentPokemon;

    public List<ItemSlot> Items => items;
    public List<PokemonSO> Pokemons => pokemons;
    public GameObject CurrentPokmeon => currentPokemon;
    
    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] private ItemSO item;
    [SerializeField] private int count;

    public ItemSO Item => item;
    public int Count => count;
}
