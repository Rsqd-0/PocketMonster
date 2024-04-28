using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemSlotUI : MonoBehaviour
{
    private ItemSO item;
    private int count;
    
    [SerializeField] private TMP_Text nameUI;
    [SerializeField] private TMP_Text countUI;

    public TMP_Text NameUI => nameUI;

    public void Set(ItemSlot itemToSet)
    {
        item = itemToSet.Item;
        nameUI.text = item.name;
        count = itemToSet.Count;
        countUI.text = $"x " + count;
    }

    public void UpdateUI()
    {
        nameUI.text = item.name;
        countUI.text = "x " + count;
    }

    public void ChangeAmount(int amount)
    {
        count += amount;
    }
}
