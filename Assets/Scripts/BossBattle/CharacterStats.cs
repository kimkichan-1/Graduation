// CharacterStats.cs

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterStats : MonoBehaviour
{
    [Header("캐릭터 기본 스탯")]
    public string characterName = "사서";
    public int maxHp = 50;
    public int currentHp;

    [Header("빛(Light) 시스템")]
    public int maxLight = 3;
    public int currentLight;

    [Header("전투 책장 시스템")]
    public List<CombatPage> deck = new List<CombatPage>(9);
    public Dictionary<CombatPage, int> cardCooldowns = new Dictionary<CombatPage, int>();
    public List<CombatPage> revealedCards = new List<CombatPage>(); // 사용한 적 있는 카드를 기록

    void Awake()
    {
        currentHp = maxHp;
        currentLight = maxLight;
        foreach (var page in deck)
        {
            if (!cardCooldowns.ContainsKey(page))
            {
                cardCooldowns.Add(page, 0);
            }
        }
    }

    public void SortDeckByCost()
    {
        if (deck.Count > 0)
        {
            deck = deck.OrderBy(page => page.lightCost).ToList();
        }
    }

    public void OnNewTurnStart()
    {
        currentLight = maxLight;
        List<CombatPage> keys = new List<CombatPage>(cardCooldowns.Keys);
        foreach (var page in keys)
        {
            if (cardCooldowns[page] > 0)
            {
                cardCooldowns[page]--;
            }
        }
    }

    public void SetCardCooldown(CombatPage page)
    {
        if (cardCooldowns.ContainsKey(page))
        {
            cardCooldowns[page] = page.lightCost;
        }
        if (!revealedCards.Contains(page))
        {
            revealedCards.Add(page);
        }
    }

    public bool IsCardUsable(CombatPage page)
    {
        return cardCooldowns.ContainsKey(page) && cardCooldowns[page] == 0;
    }
    
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        Debug.Log($"{characterName}이(가) {damage}의 피해를 입었습니다! 남은 체력: {currentHp}");
        if (currentHp <= 0)
        {
            Debug.Log($"!!! {characterName}이(가) 쓰러졌습니다. !!!");
            gameObject.SetActive(false);
        }
    }
}