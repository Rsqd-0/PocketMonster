using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum Type
    {
        Dark,
        Psy,
        Fight,
    }
[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new Pokemon")]
public class PokemonSO : ScriptableObject
{
    
    public string name;
    public int hp;
    public int attack;
    public int defense;
    public int speed;
    public Type type;
    
}
