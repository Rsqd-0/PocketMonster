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
    public float currentHp;
    public int buffCounter;
    public int xP;

    private int maxXP;
    private int baseAtk;
    private int baseDef;
    
    
    private void Awake()
    {
        pokeName = data.pokeName;
        maxHp = data.hp;
        currentHp = maxHp;
        atk = data.attack;
        def = data.defense;
        spd = data.speed;
        type = data.type;
        baseAtk = atk;
        baseDef = def;
    }
    
    public bool TakeDamage(float damage)
    {
        currentHp -= damage;
        
        if (currentHp <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public void Heal(int amount)
    {
        currentHp += amount;
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
    }

    public void Buff()
    {
        atk += baseAtk * (buffCounter + 3) / 2;
        def += baseDef / 2;
    }
    
    public void XPGain(int xp)
    {
        xP += xp;
        if (xP >= maxXP)
        {
            lvl++;
            maxHp += 10;
            currentHp = maxHp;
            atk += 2;
            def += 2;
            spd += 2;
            xP = 0;
            maxXP = lvl * 5;
        }
    }
    
    
}
;