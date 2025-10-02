// CombatDice.cs

using UnityEngine;

public enum DiceType
{
    Attack,
    Defense
}

[System.Serializable]
public class CombatDice
{
    public DiceType type;
    public int minValue;
    public int maxValue;

    public int Roll()
    {
        return Random.Range(minValue, maxValue + 1);
    }
}