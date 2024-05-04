using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] private ItemSO item;
    private Inventory inventory;

    private void Awake()
    {
        inventory = Inventory.GetInventory();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovement player))
        {
            inventory.AddToInventory(item);
            Destroy(gameObject);
        }
    }
}
