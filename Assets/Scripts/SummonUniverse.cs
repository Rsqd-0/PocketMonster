using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonUniverse : MonoBehaviour
{
    [SerializeField] private Unit pkm1;
    [SerializeField] private Unit pkm2;
    [SerializeField] private Unit pkm3;
    [SerializeField] private GameObject pkmUniverse;


    private void Update()
    {
        if ((pkm1 == null || pkm1.captured) && (pkm2 == null || pkm2.captured) && (pkm3 == null || pkm3.captured))
        {
            pkmUniverse.SetActive(true);
            Destroy(gameObject);
        }
    }

}
