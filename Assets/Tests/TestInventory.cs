using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TestInventory
{

    private GameObject character;
    private Inventory inventory;
    
    [UnitySetUp]
    public IEnumerator CharacterInstantiation()
    {
        SceneManager.LoadScene("Scenes/GameTest");
        var loadHandle = Addressables.LoadAssetAsync<GameObject>("Assets/Character/Character.prefab");

        yield return loadHandle;
        
        character = GameObject.Instantiate(loadHandle.Result);
        character.transform.position = new Vector3(10, 0, 0);
        inventory = Inventory.GetInventory();
    }
    
    [UnityTest]
    public IEnumerator TestAddPokemon()
    {
        var loadHandle = Addressables.LoadAssetsAsync<GameObject>("areaA", null);

        yield return loadHandle;
        
        var enemy = GameObject.Instantiate(loadHandle.Result[Random.Range(0, loadHandle.Result.Count)]);
        var enemyUnit = enemy.GetComponent<Unit>();
        
        inventory.AddToPokemon(enemyUnit);

        bool pokemonTest = inventory.Pokemons.Count == 1;
        yield return null;
        Assert.True(pokemonTest);
    }
}
