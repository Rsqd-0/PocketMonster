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

    // Ajoutez une étiquette en paramètre pour choisir quel groupe d'adressables charger
    public IEnumerator StartSpawn(string label)
    {
        // Charge les actifs selon l'étiquette spécifiée
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
        yield return new WaitForSeconds(5);

        var enemyPrefab = loadHandle.Result[Random.Range(0, loadHandle.Result.Count)];
        var enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity, transform);
        onSpawn.Invoke(enemy);
    }
}