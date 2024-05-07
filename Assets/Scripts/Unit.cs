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
    public float catchRate;
    public bool captured;

    public int lvl;
    public float currentHp;
    public int buffCounter;
    public int xP;
    public List<ItemSO> lootTable;

    public int maxXP;
    public int baseAtk;
    public int baseDef;
    
    
    
    private void Awake()
    {
        lvl = data.level;
        pokeName = data.pokeName;
        maxHp = data.hp;
        currentHp = maxHp;
        atk = data.attack;
        def = data.defense;
        spd = data.speed;
        type = data.type;
        baseAtk = atk;
        baseDef = def;
        lootTable = data.droppableItems;
        catchRate = data.catchRate;
    }
    
    public bool TakeDamage(float damage)
    {
        if (damage <= 0)
        {
            currentHp -= 1;
        }
        else
        {
            currentHp -= damage;
        }
        
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
        if (amount <= 0)
        {
            currentHp += 1;
        }
        else
        {
            currentHp += amount;
        }
        
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
    }

    public void Buff()
    {
        buffCounter++;
        atk += baseAtk / 2;
        def += baseDef / 2;
    }
    
    public void Distracted()
    {
        buffCounter--;
        atk -= baseAtk / 3;
        def -= baseDef / 3;
    }
    
    public void XPGain(int xp)
    {
        xP += xp;
        if (xP >= maxXP)
        {
            lvl++;
            maxHp += 10;
            currentHp = maxHp;
            baseAtk += 2;
            baseDef += 2;
            spd += 2;
            xP = 0;
            maxXP = lvl * 5;
        }
    }
    
    public void ResetBuff()
    {
        buffCounter = 0;
        atk = baseAtk;
        def = baseDef;
        currentHp = Mathf.RoundToInt(currentHp);
        if (currentHp < 0) currentHp = 0;
    }
    
    
}
;