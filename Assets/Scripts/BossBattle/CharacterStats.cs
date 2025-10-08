using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterStats : MonoBehaviour
{
    private PlayerController playerController; // PlayerController 참조

    [Header("전투용 스탯 (PlayerController에서 복사됨)")]
    public string characterName = "VOID";
    public int maxHp;
    public int currentHp;
    public int attackPower;
    public int defensePower;
    public float moveSpeed;

    [Header("빛(Light) 시스템")]
    public int maxLight = 3;
    public int currentLight;

    [Header("전투 책장 시스템")]
    public List<CombatPage> deck = new List<CombatPage>(9);
    public Dictionary<CombatPage, int> cardCooldowns = new Dictionary<CombatPage, int>();
    public List<CombatPage> revealedCards = new List<CombatPage>();

    void Awake()
    {
        // PlayerController가 있는 오브젝트일 경우에만 참조를 가져옴 (보스는 해당 없음)
        playerController = GetComponent<PlayerController>();

        // 덱 초기화
        foreach (var page in deck)
        {
            if (!cardCooldowns.ContainsKey(page))
            {
                cardCooldowns.Add(page, 0);
            }
        }
    }

    // PlayerController로부터 스탯을 복사해오는 함수
    public void InitializeFromController()
    {
        if (playerController == null)
        {
            // 이 오브젝트가 보스처럼 PlayerController가 없는 경우, 이 함수는 아무것도 하지 않음
            return;
        }

        Debug.Log("PlayerController로부터 스탯을 복사합니다...");
        this.maxHp = playerController.maxHp;
        this.currentHp = playerController.maxHp;
        this.attackPower = playerController.attackPower;
        this.defensePower = playerController.defensePower;
        this.moveSpeed = playerController.baseMoveSpeed;
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