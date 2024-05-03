using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    START,
    PLAYERTURN,
    ENEMYTURN,
    WON,
    LOST,
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField]private GameObject playerPrefab;
    [SerializeField]private GameObject enemyPrefab;
    [SerializeField]private Transform playerBattleStation;
    [SerializeField] private Transform enemyBattleStation;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private BattleHUD enemyHUD;
    
    public BattleState state;
    
    Unit playerUnit;
    Unit enemyUnit;
    
    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation.position, playerBattleStation.rotation);
        playerUnit = playerGO.GetComponent<Unit>();
        
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation.position, enemyBattleStation.rotation);
        enemyUnit = enemyGO.GetComponent<Unit>();
        
        dialogueText.text = "A wild " + enemyUnit.pokeName + " approaches...";
        playerHUD.SetHud(playerUnit);
        enemyHUD.SetHud(enemyUnit);

        yield return new WaitForSeconds(2f);
        
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }
    
    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
    }
}
