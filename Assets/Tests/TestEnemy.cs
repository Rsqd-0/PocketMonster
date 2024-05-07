using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TestEnemy
{

    private GameObject enemy;
    private Unit enemyUnit;
    
    [UnitySetUp]
    public IEnumerator EnemyInstantiation()
    {
        SceneManager.LoadScene("Scenes/GameTest");
        var loadHandle = Addressables.LoadAssetsAsync<GameObject>("areaA", null);

        yield return loadHandle;
        
        enemy = GameObject.Instantiate(loadHandle.Result[Random.Range(0, loadHandle.Result.Count)]);
        enemyUnit = enemy.GetComponent<Unit>();
    }
    
    [UnityTest]
    public IEnumerator TestCorrectType()
    {
        //areaA possède CosmicB et ArcaneA
        bool typeEquals = enemyUnit.type is Type.Arcane or Type.Cosmic;
        yield return null;
        Assert.True(typeEquals);
    }
    
    [UnityTest]
    public IEnumerator TestDistract()
    {
        enemyUnit.Distracted();
        bool attackTest = enemyUnit.atk < enemyUnit.baseAtk && enemyUnit.def < enemyUnit.baseDef && enemyUnit.buffCounter == -1;
        yield return null;
        Assert.True(attackTest);
    }
    
    [UnityTest]
    public IEnumerator TestAttack()
    {
        enemyUnit.TakeDamage(1);
        bool hpTest = enemyUnit.currentHp < enemyUnit.maxHp;
        yield return null;
        Assert.True(hpTest);
    }
    
    [UnityTest]
    public IEnumerator TestHeal()
    {
        enemyUnit.TakeDamage(1);
        float hpBeforeHeal = enemyUnit.currentHp;
        enemyUnit.Heal(5);
        bool healTest = enemyUnit.currentHp > hpBeforeHeal && enemyUnit.currentHp <= enemyUnit.maxHp;
        yield return null;
        Assert.True(healTest);
    }
    
    [UnityTest]
    public IEnumerator TestBuff()
    {
        enemyUnit.Buff();
        bool buffTest =
            enemyUnit.baseAtk < enemyUnit.atk && enemyUnit.baseDef < enemyUnit.def && enemyUnit.buffCounter == 1;
        yield return null;
        Assert.True(buffTest);
    }
    
    [UnityTest]
    public IEnumerator TestResetBuff()
    {
        enemyUnit.Buff();
        enemyUnit.ResetBuff();
        bool resetBuffTest =
            enemyUnit.baseAtk == enemyUnit.atk && enemyUnit.baseDef == enemyUnit.def && enemyUnit.buffCounter == 0;
        enemyUnit.Distracted();
        enemyUnit.ResetBuff();
        bool resetDistractTest =
            enemyUnit.baseAtk == enemyUnit.atk && enemyUnit.baseDef == enemyUnit.def && enemyUnit.buffCounter == 0;
        yield return null;
        Assert.True(resetBuffTest && resetDistractTest);
    }
}
