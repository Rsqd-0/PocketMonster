using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test
{
    [UnitySetUp]
    public IEnumerator CharacterControllerInstanciation()
    {
        handle = Addressables.LoadAssetAsync<GameObject>("Assets/CharacterA/Character.prefab");
        yield return handle;
        player = GameObject.Instantiate(handle.Result,Vector3.zero,Quaternion.identity);
        
    }
}
