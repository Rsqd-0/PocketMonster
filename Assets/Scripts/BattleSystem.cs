using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum BattleState
{
    START,
    PLAYERTURN,
    ENEMYTURN,
    WON,
    LOST,
    ESCAPED,
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerBattleStation;
    [SerializeField] private Transform enemyBattleStation;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private BattleHUD enemyHUD;
    
    private Inventory inventory;
    private GameObject playerGO;
    private GameObject enemyGO;
    
    public BattleState state;
    
    public Unit playerUnit;
    public Unit enemyUnit;
    
    /// <summary>
    ///   <para>Start the battle</para>
    /// </summary>
    void Start()
    {
        Game.CursorVisible();
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }
    
    /// <summary>
    ///   <para>Setup the battle phase</para>
    /// </summary>
    IEnumerator SetupBattle()
    {
        playerGO = Instantiate(Inventory.GetInventory().GetCurrentPokemon().gameObject, playerBattleStation.position, playerBattleStation.rotation);
        playerUnit = playerGO.GetComponent<Unit>();
        
        enemyGO = SaveData.GetEnemyData();
        enemyGO.transform.position = enemyBattleStation.position;
        enemyGO.transform.rotation = enemyBattleStation.rotation;
        enemyUnit = enemyGO.GetComponent<Unit>();
        
        //Game.CursorVisible();
        
        dialogueText.text = "A wild " + enemyUnit.pokeName + " approaches...";
        playerHUD.SetHud(playerUnit);
        enemyHUD.SetHud(enemyUnit);
        

        yield return new WaitForSeconds(2f);
        
        if (SpeedCheck())
        {
            state = BattleState.PLAYERTURN;
        }
        else
        {
            state = BattleState.ENEMYTURN;
        }
        
        PlayerTurn();
    }

    private void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.A))
        {
            Destroy(playerGO);
            Destroy(enemyGO);
            SceneManager.UnloadSceneAsync("Fight");
        }
    }

    /// <summary>
    ///   <para>Handle the what happens at this end of the battle</para>
    /// </summary>
    void EndBattle()
    {
        switch (state)
        {
            case BattleState.WON:
                dialogueText.text = "You won the battle!";
                SaveData.SetPlayerWon(true);
                Inventory.GetInventory().AddToInventory(enemyUnit.lootTable[Random.Range(0,enemyUnit.lootTable.Count)]);
                break;
            case BattleState.LOST:
                dialogueText.text = "You were defeated!";
                SaveData.SetPlayerWon(false);
                break;
            case BattleState.ESCAPED:
                dialogueText.text = "You ran away!";
                SaveData.SetPlayerWon(false);
                SceneManager.UnloadSceneAsync("Fight");
                break;
        }
        playerUnit.XPGain(enemyUnit.lvl);
        //Loot + XP + Level up + change scene
        Game.CursorInvisible();
    }
    

    // Region with the player's action
    
    #region Player  

    /// <summary>
    ///   <para>Change text to show that the player can do an action</para>
    /// </summary>
    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
    }
    
    /// <summary>
    ///   <para>Heal the pokemon of the player</para>
    /// </summary>
    IEnumerator PlayerHeal()
    {
        playerUnit.currentHp += playerUnit.def + Random.Range(-3,4);
        playerHUD.SetHP(playerUnit.currentHp);
        dialogueText.text = "You feel renewed strength!";

        state = BattleState.ENEMYTURN;
        yield return new WaitForSeconds(2f);

        StartCoroutine(EnemyTurn());
    }

    /// <summary>
    ///   <para>Buff the pokemon of the player</para>
    /// </summary>
    IEnumerator PlayerBuff()
    {
        if (playerUnit.buffCounter < 6)
        {
            playerUnit.Buff();
            playerUnit.buffCounter++;
            dialogueText.text = "You feel stronger!";
        }
        else
        {
            dialogueText.text = "You don't feel any stronger!";
        }

        state = BattleState.ENEMYTURN;
        yield return new WaitForSeconds(2f);

        StartCoroutine(EnemyTurn());
    }
    
    /// <summary>
    ///   <para>Attack the enemy and test if the enemy is dead</para>
    /// </summary>
    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage( (playerUnit.atk + Random.Range(-3,3)) * EffectiveTypeCheck(playerUnit,enemyUnit) );
        enemyHUD.SetHP(enemyUnit.currentHp);
        dialogueText.text = "The attack was successful!";
        
        state = BattleState.ENEMYTURN;
        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            
            StartCoroutine(EnemyTurn());
        }
    }

    /// <summary>
    ///   <para>Debuff the enemy</para>
    /// </summary>
    IEnumerator PlayerDistract()
    {
        if (playerUnit.buffCounter < 6)
        {
            enemyUnit.Buff();
            enemyUnit.buffCounter--;
            dialogueText.text = enemyUnit.pokeName + "feel weaker!";
        }
        else
        {
            dialogueText.text = enemyUnit.pokeName + "don't feel any weaker!";
        }
        
        state = BattleState.ENEMYTURN;
        yield return new WaitForSeconds(2f);

        StartCoroutine(EnemyTurn());
    }

    #endregion

    // Region with the enemy's action
    
    #region Enemy

    /// <summary>
    ///   <para>Randomly choose what action the enemy will do</para>
    /// </summary>
    IEnumerator EnemyTurn()
    {
        int move = Random.Range(0, 5);

        switch (move)
        {
            case 0:
            case 1:
                yield return StartCoroutine(EnemyAttack());
                break;
            case 2:
                yield return StartCoroutine(EnemyHeal());
                break;
            case 3:
                yield return StartCoroutine(EnemyDistract());
                break;
            default:
                yield return StartCoroutine(EnemyBuff());
                break;
        }
    }
    
    /// <summary>
    ///   <para>Attack the player's pokemon and test if the pokemon is dead</para>
    /// </summary>
    IEnumerator EnemyAttack()
    {
        dialogueText.text = enemyUnit.pokeName + " attacks!";
        
        yield return new WaitForSeconds(1f);
        bool isDead = playerUnit.TakeDamage( (enemyUnit.atk + Random.Range(-3,4)) * EffectiveTypeCheck(enemyUnit,playerUnit) );
        playerHUD.SetHP(playerUnit.currentHp);
        
        state = BattleState.PLAYERTURN;
        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            
            PlayerTurn();
        }
    }
    
    /// <summary>
    ///   <para>Heal the enemy</para>
    /// </summary>
    IEnumerator EnemyHeal()
    {
        enemyUnit.currentHp += enemyUnit.def + Random.Range(-3,4);
        enemyHUD.SetHP(enemyUnit.currentHp);
        dialogueText.text = enemyUnit.pokeName + " heals!";
        
        state = BattleState.PLAYERTURN;
        yield return new WaitForSeconds(2f);
        
        PlayerTurn();
    }
    
    /// <summary>
    ///   <para>Buff The enemy</para>
    /// </summary>
    IEnumerator EnemyBuff()
    {
        if (enemyUnit.buffCounter < 6)
        {
            enemyUnit.Buff();
            enemyUnit.buffCounter++;
            dialogueText.text = enemyUnit.pokeName + " feels stronger!";
        }
        else
        {
            dialogueText.text = enemyUnit.pokeName + " doesn't feel any stronger!";
        }
        
        state = BattleState.PLAYERTURN;
        yield return new WaitForSeconds(2f);
        
        PlayerTurn();
    }
    
    IEnumerator EnemyDistract()
    {
        if (enemyUnit.buffCounter < 6)
        {
            playerUnit.Buff();
            playerUnit.buffCounter--;
            dialogueText.text = playerUnit.pokeName + "feel weaker!";
        }
        else
        {
            dialogueText.text = playerUnit.pokeName + "don't feel any weaker!";
        }
        
        state = BattleState.PLAYERTURN;
        yield return new WaitForSeconds(2f);

        PlayerTurn();
    }

    #endregion
    
    // Region with function used on buttons
    
    #region OnBtn   

    /// <summary>
    ///   <para>Damaging move is used</para>
    /// </summary>
    public void OnDamageAttack()
    {
        if (state != BattleState.PLAYERTURN) return;
        
        StartCoroutine(PlayerAttack());
    }
    
    /// <summary>
    ///   <para>Heal move is used</para>
    /// </summary>
    public void OnHealAttack()
    {
        if (state != BattleState.PLAYERTURN) return;
        
        StartCoroutine(PlayerHeal());
    }
    
    /// <summary>
    ///   <para>Buff move is used</para>
    /// </summary>
    public void OnBuffAttack()
    {
        if (state != BattleState.PLAYERTURN) return;
        
        StartCoroutine(PlayerBuff());
    }
    
    /// <summary>
    ///   <para>Run button is used</para>
    /// </summary>
    public void OnRunButton()
    {
        if (state != BattleState.PLAYERTURN) return;
        
        dialogueText.text = "You ran away!";
        state = BattleState.ESCAPED;
        EndBattle();
    }

    /// <summary>
    ///   <para>Debuff button is used</para>
    /// </summary>
    public void OnDistractAttack()
    {
        if (state != BattleState.PLAYERTURN) return;
        
        StartCoroutine(PlayerDistract());
    }

    #endregion
    
    // Region with function that checks thing
    #region Check

    /// <summary>
    ///   <para>Check whose speed is higher</para>
    /// </summary>
    private bool SpeedCheck()
    {
        return playerUnit.spd > enemyUnit.spd;
    }

    /// <summary>
    ///   <para>Test type effectiveness</para>
    /// </summary>
    private float EffectiveTypeCheck(Unit attacker, Unit defender)
    {
        switch (attacker.type)
        {
            case Type.Arcane when defender.type == Type.Time:
            case Type.Time when defender.type == Type.Cosmic:
            case Type.Cosmic when defender.type == Type.Arcane:
                return 1.5f;
            case Type.Time when defender.type == Type.Arcane:
            case Type.Cosmic when defender.type == Type.Time:
            case Type.Arcane when defender.type == Type.Cosmic:
                return 0.5f;
            default:
                return 1f;
        }
    }
    
    #endregion
   
    
}


