using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PannelUI : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> pokemons;

    private List<PokemonSO> listPokemons;
    private Inventory inventory;
    
    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.GetInventory();
        UpdatePokemon();
    }

    void UpdatePokemon()
    {
        listPokemons = inventory.Pokemons;
        for (int i = 0; i < listPokemons.Count; i++)
        {
            pokemons[i].fontStyle =  FontStyles.Normal; 
            pokemons[i].text = listPokemons[i].pokeName;
                switch (listPokemons[i].type)
                {
                    case Type.Arcane:
                        pokemons[i].color = Color.Lerp(Color.red, Color.blue, 0.5f);
                        break;
                    case Type.Time:
                        pokemons[i].color = Color.yellow;
                        break;
                    case Type.Cosmic:
                        pokemons[i].color = Color.magenta;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }
        pokemons[inventory.CurrentPokemon].fontStyle = FontStyles.Underline;
    }
}
