using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonOverworld : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovement player))
        {
            Debug.Log("Start Combat");
        }
    }
}
