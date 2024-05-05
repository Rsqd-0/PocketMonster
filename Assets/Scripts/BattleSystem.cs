using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    
    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        playerGO = Instantiate(playerPrefab, playerBattleStation.position, playerBattleStation.rotation);
        playerUnit = playerGO.GetComponent<Unit>();
        
        enemyGO = Instantiate(SaveData.GetEnemyData(), enemyBattleStation.position, enemyBattleStation.rotation);
        enemyUnit = enemyGO.GetComponent<Unit>();
        
        
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


    void EndBattle()
    {
        switch (state)
        {
            case BattleState.WON:
                dialogueText.text = "You won the battle!";
                SaveData.SetPlayerWon(true);
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
    }
    


    #region Player  

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
    }
    
    IEnumerator PlayerHeal()
    {
        playerUnit.currentHp += playerUnit.def + Random.Range(-3,4);
        playerHUD.SetHP(playerUnit.currentHp);
        dialogueText.text = "You feel renewed strength!";

        state = BattleState.ENEMYTURN;
        yield return new WaitForSeconds(2f);

        StartCoroutine(EnemyTurn());
    }

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

    IEnumerator PlayerDistract()
    {
        enemyUnit.Distracted();
    }

    #endregion

    
    #region Enemy

    IEnumerator EnemyTurn()
    {
        //Random sur les attaques de l'ennemi
        int move = Random.Range(0, 4);

        switch (move)
        {
            case 0:
            case 1:
                yield return StartCoroutine(EnemyAttack());
                break;
            case 2:
                yield return StartCoroutine(EnemyHeal());
                break;
            default:
                yield return StartCoroutine(EnemyBuff());
                break;
        }
    }
    
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
    
    IEnumerator EnemyHeal()
    {
        enemyUnit.currentHp += enemyUnit.def + Random.Range(-3,4);
        enemyHUD.SetHP(enemyUnit.currentHp);
        dialogueText.text = enemyUnit.pokeName + " heals!";
        
        state = BattleState.PLAYERTURN;
        yield return new WaitForSeconds(2f);
        
        PlayerTurn();
    }
    
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

    #endregion
    
    
    #region OnBtn   

    public void OnDamageAttack()
    {
        if (state != BattleState.PLAYERTURN) return;
        
        StartCoroutine(PlayerAttack());
    }
    
    public void OnHealAttack()
    {
        if (state != BattleState.PLAYERTURN) return;
        
        StartCoroutine(PlayerHeal());
    }
    
    public void OnBuffAttack()
    {
        if (state != BattleState.PLAYERTURN) return;
        
        StartCoroutine(PlayerBuff());
    }
    
    public void OnRunButton()
    {
        if (state != BattleState.PLAYERTURN) return;
        
        dialogueText.text = "You ran away!";
        state = BattleState.ESCAPED;
        EndBattle();
    }

    public void OnDistractAttack()
    {
        if (state != BattleState.PLAYERTURN) return;
        
        StartCoroutine(PlayerDistract());
    }

    #endregion
    

    #region Check

        private bool SpeedCheck()
        {
            return playerUnit.spd > enemyUnit.spd;
        }

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


