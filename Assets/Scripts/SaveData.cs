using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveData
{
    
    private static GameObject enemyGO;
    private static bool playerWon;
    private static InventoryManagerUI inventoryManagerUI;
    
    public static void SaveEnemyData(GameObject enemy)
    {
        enemyGO = enemy;
    }
    
    public static GameObject GetEnemyData()
    {
        return enemyGO;
    }

    public static void SetPlayerWon(bool state)
    {
        playerWon = state;
    }

    public static bool GetPlayerWon()
    {
        return playerWon;
    }

    public static void SetInventoryUI(InventoryManagerUI i)
    {
        inventoryManagerUI = i;
    }

    public static InventoryManagerUI GetInventoryUI()
    {
        return inventoryManagerUI;
    }
}
