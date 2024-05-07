using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] private ItemSO item;
    [SerializeField] private AudioSource lootSound;
    private Inventory inventory;

    private void Awake()
    {
        inventory = Inventory.GetInventory();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovement player))
        {
            lootSound.Play();
            inventory.AddToInventory(item);
            Destroy(gameObject);
        }
    }
}
