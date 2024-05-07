using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum Type
    {
        Arcane,
        Time,
        Cosmic,
    }
[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new Pokemon")]
public class PokemonSO : ScriptableObject
{
    public string pokeName;
    public int level;
    public int hp;
    public int attack;
    public int defense;
    public int speed;
    public Type type;
    public List<ItemSO> droppableItems;
    public float catchRate;

}
