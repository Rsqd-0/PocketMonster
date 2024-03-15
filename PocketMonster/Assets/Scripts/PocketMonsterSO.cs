using UnityEngine;

public enum Type
{
    Dark,
    Psy,
    Fight,
}

[CreateAssetMenu(fileName = "New PocketMonster", menuName = "Creatures/Generate PocketMonster", order = 1)]
public class PocketMonsterSO : ScriptableObject
{
    public Type type;
    public int maxHp;
    public int maxAttack;
    public int maxDefense;
}