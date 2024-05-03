using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private PokemonSO data;

    public string pokeName;
    public int maxHp;
    public int atk;
    public int def;
    public int spd;
    public Type type;

    public int lvl;
    public int currentHp;
    
    private void Awake()
    {
        pokeName = data.pokeName;
        maxHp = data.hp;
        currentHp = maxHp;
        atk = data.attack;
        def = data.defense;
        spd = data.speed;
        type = data.type;
    }
    
    
}
;