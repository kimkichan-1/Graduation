// CombatPage.cs

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCombatPage", menuName = "Library of Ruina/Combat Page")]
public class CombatPage : ScriptableObject
{
    [Header("책장 정보")]
    public string pageName;
    public int lightCost;

    [Header("주사위 목록")]
    public List<CombatDice> diceList = new List<CombatDice>();
}