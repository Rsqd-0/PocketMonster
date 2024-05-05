using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pokemonName;
    [SerializeField] private TextMeshProUGUI pokemonLevel;
    [SerializeField] private Slider pokemonHP;

    /// <summary>
    ///   <para>Set pokemon UI</para>
    /// </summary>
    public void SetHud(Unit unit)
    {
        pokemonName.text = unit.pokeName;
        pokemonLevel.text = "Lvl : " + unit.lvl;
        pokemonHP.maxValue = unit.maxHp;
        pokemonHP.value = unit.currentHp;
    }
    
    /// <summary>
    ///   <para>Set pokemon HP bar</para>
    /// </summary>
    public void SetHP(float hp)
    {
        pokemonHP.value = hp;
    }
}
