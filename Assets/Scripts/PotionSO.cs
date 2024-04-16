using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new potion")]
public class PotionSO : ItemSO
{
    public int hpAmount;
    public bool revive;
}
