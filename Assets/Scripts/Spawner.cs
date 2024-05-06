using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    private AsyncOperationHandle<IList<GameObject>> loadHandle;
    private UnityEvent<GameObject> onSpawn = new UnityEvent<GameObject>();
    private GameObject enemy;
    private bool spawned = false;
    private int spawningDelay = 1;
    
    public IEnumerator StartSpawn(string label)
    {
        loadHandle = Addressables.LoadAssetsAsync<GameObject>(label, null);
        loadHandle.Completed += (operation) => { StartCoroutine(SpawnEnemy()); };

        yield return loadHandle;
    }

    private void OnDestroy()
    {
        Addressables.Release(loadHandle);
    }

    public void AddOnSpawnListener(UnityAction<GameObject> listener)
    {
        onSpawn.AddListener(listener);
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(spawningDelay);

        var enemyPrefab = loadHandle.Result[Random.Range(0, loadHandle.Result.Count)];
        enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity, transform);
        spawned = true;
        onSpawn.Invoke(enemy);
    }

    void Update()
    {
        if (spawned && enemy == null)
        {
            spawningDelay = 7;
            spawned = false;
            StartCoroutine(SpawnEnemy());
        }
    }
}