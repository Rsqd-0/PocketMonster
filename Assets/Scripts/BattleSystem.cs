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
    CAPTURED
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private Transform playerBattleStation;
    [SerializeField] private Transform enemyBattleStation;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private BattleHUD enemyHUD;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource powerUpSound;
    [SerializeField] private AudioSource distractSound;
    [SerializeField] private AudioSource healSound;
    
    [SerializeField] private GameObject inventoryBattle;
    [SerializeField] private GameObject buttons;
    
    private Inventory inventory;
    private GameObject playerGO;
    private Transform playerChild;
    private GameObject enemyGO;
    private Transform enemyChild;
    private InventoryManagerUI inventoryManagerUI;
    private PokemonOverworld pokemonOverworld;
    private PlayerMovement character;
    
    public BattleState state;
    
    public Unit playerUnit;
    public Unit enemyUnit;
    
    /// <summary>
    ///   <para>Start the battle</para>
    /// </summary>
    void Start()
    {
        character = SaveData.GetCharacter();
        inventoryManagerUI = SaveData.GetInventoryUI();
        inventoryManagerUI.inBattle = true;
        Game.CursorVisible();
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }
    
    /// <summary>
    ///   <para>Setup the battle phase</para>
    /// </summary>
    IEnumerator SetupBattle()
    {
        playerGO = Inventory.GetInventory().GetCurrentPokemon().gameObject;
        playerChild = playerGO.transform.GetChild(0);
        playerChild.position = playerBattleStation.position;
        playerChild.rotation = playerBattleStation.rotation;
        playerUnit = playerGO.GetComponent<Unit>();
        
        enemyGO = SaveData.GetEnemyData();
        enemyChild = enemyGO.transform.GetChild(0);
        enemyChild.position = enemyBattleStation.position;
        enemyChild.rotation = enemyBattleStation.rotation;
        enemyUnit = enemyGO.GetComponent<Unit>();
        pokemonOverworld = enemyChild.GetComponent<PokemonOverworld>();
        
        dialogueText.text = "A wild " + enemyUnit.pokeName + " approaches...";
        playerHUD.SetHud(playerUnit);
        enemyHUD.SetHud(enemyUnit);
        

        yield return new WaitForSeconds(2f);
        
        if (SpeedCheck())
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.P)) return;
    }

    /// <summary>
    ///   <para>Handle the what happens at this end of the battle</para>
    /// </summary>
    IEnumerator EndBattle()
    {
        yield return new WaitForSeconds(1f);
        switch (state)
        {
            case BattleState.CAPTURED:
                dialogueText.text = "You captured " + enemyUnit.pokeName + " !";
                yield return new WaitForSeconds(1f);
                SaveData.SetPlayerWon(true);
                enemyUnit.currentHp = enemyUnit.maxHp;
                enemyGO.transform.position = playerGO.transform.position;
                enemyChild.position = playerGO.transform.position;
                Game.overworldMusicPlay();
                SceneManager.UnloadSceneAsync("Fight");
                break;
            case BattleState.WON:
                dialogueText.text = "You won the battle!";
                yield return new WaitForSeconds(1f);
                SaveData.SetPlayerWon(true);
                ItemSO lootedItem = enemyUnit.lootTable[Random.Range(0,enemyUnit.lootTable.Count)];
                Inventory.GetInventory().AddToInventory(lootedItem);
                dialogueText.text = "You got " + lootedItem.name + "!";
                yield return new WaitForSeconds(2f);
                playerUnit.XPGain(enemyUnit.lvl);
                Destroy(enemyGO);
                Game.overworldMusicPlay();
                SceneManager.UnloadSceneAsync("Fight");
                break;
            case BattleState.LOST:
                dialogueText.text = "You were defeated!";
                yield return new WaitForSeconds(1f);
                SaveData.SetPlayerWon(false);
                character.gameObject.transform.position = SaveData.GetCharacterPosition();
                enemyChild.position = enemyGO.transform.position;
                pokemonOverworld.enabled = true;
                pokemonOverworld.StartMovement();
                Game.overworldMusicPlay();
                SceneManager.UnloadSceneAsync("Fight");
                break;
            case BattleState.ESCAPED:
                dialogueText.text = "You ran away!";
                yield return new WaitForSeconds(1f);
                SaveData.SetPlayerWon(false);
                character.gameObject.transform.position = SaveData.GetCharacterPosition();
                enemyChild.position = enemyGO.transform.position;
                pokemonOverworld.enabled = true;
                pokemonOverworld.StartMovement();
                Game.overworldMusicPlay();
                SceneManager.UnloadSceneAsync("Fight");
                break;
        }
        playerChild.position = playerGO.transform.position;
        playerUnit.ResetBuff();
        enemyUnit.ResetBuff();
        inventoryManagerUI.UpdatePokemonList();
        inventoryManagerUI.UpdateItemList();
        inventoryManagerUI.UpdateDropdown();
        Game.CursorInvisible();
        inventoryManagerUI.inBattle = false;
    }
    
    public Unit CapturePokemon(PokeballSO ball)
    {
        float catchRate = (1 - (enemyUnit.currentHp / enemyUnit.maxHp)) * ball.bonusBall * enemyUnit.catchRate;
        float random = Random.Range(0f, 1f);
        if (random < catchRate)
        {
            state = BattleState.CAPTURED;
            enemyUnit.captured = true;
            return enemyUnit;
        }
        else
        {
            return null;
        }
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
        playerUnit.Heal(playerUnit.def / 2 + Random.Range(-3,4));
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
        bool isDead = enemyUnit.TakeDamage( (playerUnit.atk + Random.Range(-3,3)) * EffectiveTypeCheck(playerUnit,enemyUnit) - 0.5f*enemyUnit.def);
        enemyHUD.SetHP(enemyUnit.currentHp);
        dialogueText.text = "The attack was successful!";
        
        state = BattleState.ENEMYTURN;
        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.WON;
            StartCoroutine(EndBattle());
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
        if (enemyUnit.buffCounter > -6)
        {
            enemyUnit.Distracted();
            dialogueText.text = enemyUnit.pokeName + " feel weaker!";
        }
        else
        {
            dialogueText.text = enemyUnit.pokeName + " don't feel any weaker!";
        }
        
        state = BattleState.ENEMYTURN;
        yield return new WaitForSeconds(2f);

        StartCoroutine(EnemyTurn());
    }

    public IEnumerator PlayerItem(ItemSO itemUsed)
    {
        if (itemUsed != null)
        {
            //close ui
            inventoryBattle.SetActive(false);
            buttons.SetActive(true);
            dialogueText.text = itemUsed.name + " was used !";
            state = BattleState.ENEMYTURN;
            yield return new WaitForSeconds(2f);
            if (enemyUnit.captured)
            {
                state = BattleState.CAPTURED;
                StartCoroutine(EndBattle());
            }
            else
            {
                StartCoroutine(EnemyTurn());
            }
        }
        else
        {
            dialogueText.text = "You already have 6 creatures !";
            yield return new WaitForSeconds(2f);
            dialogueText.text = "Choose an action:";
        }
        
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
        attackSound.Play();
        bool isDead = playerUnit.TakeDamage( (enemyUnit.atk + Random.Range(-3,4)) * EffectiveTypeCheck(enemyUnit,playerUnit) - 0.5f*playerUnit.def);
        playerHUD.SetHP(playerUnit.currentHp);
        
        state = BattleState.PLAYERTURN;
        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.LOST;
            StartCoroutine(EndBattle());
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
        enemyUnit.Heal(enemyUnit.def / 2 + Random.Range(-3,4));
        enemyHUD.SetHP(enemyUnit.currentHp);
        
        dialogueText.text = enemyUnit.pokeName + " heals!";
        healSound.Play();
        
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
            dialogueText.text = enemyUnit.pokeName + " feels stronger!";
            powerUpSound.Play();
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
        if (playerUnit.buffCounter > -6)
        {
            playerUnit.Distracted();
            dialogueText.text = playerUnit.pokeName + " feel weaker!";
            distractSound.Play();
        }
        else
        {
            dialogueText.text = playerUnit.pokeName + " don't feel any weaker!";
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
        
        
        int fail = Random.Range(0, 5);
        switch (fail)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                state = BattleState.ESCAPED;
                StartCoroutine(EndBattle());
                break;
            default:
                state = BattleState.ENEMYTURN;
                dialogueText.text = "You failed to escape!";
                StartCoroutine(EnemyTurn());
                break;
        }
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
                return 1.2f;
            case Type.Time when defender.type == Type.Arcane:
            case Type.Cosmic when defender.type == Type.Time:
            case Type.Arcane when defender.type == Type.Cosmic:
                return 0.8f;
            case Type.Universe:
                return 1.2f;
            default:
                return 1f;
        }
    }
    
    #endregion
    
}


