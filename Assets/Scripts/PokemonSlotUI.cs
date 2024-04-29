using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PokemonSlotUI : MonoBehaviour
{
    private PokemonSO pokemon;
    
    [SerializeField] private TMP_Text nameUI;
    [SerializeField] private TMP_Text typeUI;

    public TMP_Text NameUI => nameUI;
    public TMP_Text TypeUI => typeUI;

    public void Set(PokemonSO pokemonToSet)
    {
        pokemon = pokemonToSet;
        nameUI.text = pokemon.name;
        typeUI.text = pokemon.type.ToString();
        switch (pokemon.type)
        {
            case Type.Arcane:
                typeUI.color = Color.Lerp(Color.red,Color.blue,0.5f);
                break;
            case Type.Time:
                typeUI.color = Color.yellow;
                break;
            case Type.Cosmic:
                typeUI.color = Color.magenta;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}