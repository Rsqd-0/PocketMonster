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
        
        character = GameObject.Instantiate(loadHandle.Result,new Vector3(20, 0, 0),Quaternion.identity);
        inventory = Inventory.GetInventory();
    }
    
    [UnityTest]
    public IEnumerator TestAddPokemon()
    {
        var loadHandle = Addressables.LoadAssetsAsync<GameObject>("areaA", null);

        yield return loadHandle;
        
        var enemy = GameObject.Instantiate(loadHandle.Result[Random.Range(0, loadHandle.Result.Count)]);
        enemy.transform.GetChild(0).GetComponent<PokemonOverworld>().enabled = false;
        var enemyUnit = enemy.GetComponent<Unit>();
        
        inventory.AddToPokemon(enemyUnit);

        bool pokemonTest = inventory.Pokemons.Count == 1;
        yield return null;
        Assert.True(pokemonTest);
    }
    
    [UnityTest]
    public IEnumerator TestAddItem()
    {
        int countBeforeAdding = inventory.Items[0].Count;
        inventory.AddToInventory(inventory.Items[0].Item);
        int countAfterAdding = inventory.Items[0].Count;

        bool addTest = countBeforeAdding < countAfterAdding;
        yield return null;
        Assert.True(addTest);
    }
    
    [UnityTest]
    public IEnumerator TestCollectItem()
    {
        int countBeforeAdding = inventory.Items[0].Count;
        var loadHandle = Addressables.LoadAssetAsync<GameObject>("Assets/Items/Collectable.prefab");

        yield return loadHandle;
        
        var collectable = GameObject.Instantiate(loadHandle.Result);
        collectable.GetComponentInChildren<CollectableItem>().SetItem(inventory.Items[0].Item);

        collectable.transform.position = character.transform.position;
        yield return new WaitForSeconds(2f);
        
        int countAfterAdding = inventory.Items[0].Count;
        bool collectTest = countBeforeAdding < countAfterAdding;
        yield return null;
        Assert.True(collectTest);
    }
}
