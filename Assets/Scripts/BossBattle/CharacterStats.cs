using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterStats : MonoBehaviour
{
    // BattleController 참조 변수
    public BattleController battleController;

    private PlayerController playerController;
    private PlayerHealth playerHealth;
    private PlayerStats playerStats;

    [Header("전투용 스탯")]
    public string characterName = "사서";
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
        // 씬에 있는 BattleController를 찾아서 연결 (보스에게만 필요)
        if (battleController == null)
        {
            battleController = FindObjectOfType<BattleController>();
        }
        
        playerController = GetComponent<PlayerController>();
        playerHealth = GetComponent<PlayerHealth>();
        playerStats = GetComponent<PlayerStats>();
        
        foreach (var page in deck)
        {
            if (!cardCooldowns.ContainsKey(page))
            {
                cardCooldowns.Add(page, 0);
            }
        }
    }

    public void InitializeFromPlayerScripts()
    {
        if (playerController == null || playerHealth == null || playerStats == null)
        {
            this.currentHp = this.maxHp;
            Debug.Log($"{characterName}의 스탯을 인스펙터 기본값으로 초기화합니다.");
            return;
        }

        Debug.Log("Player 스크립트들로부터 전투 스탯을 복사합니다...");
        
        this.maxHp = playerHealth.maxHealth;
        this.currentHp = playerHealth.GetCurrentHealth();
        this.attackPower = playerController.attackPower + playerStats.bonusAttackPower;
        this.defensePower = playerController.defensePower + playerHealth.defense;
        this.moveSpeed = playerController.baseMoveSpeed + playerStats.bonusMoveSpeed;
    }
    
    public void TakeDamage(int damage)
    {
        if (playerHealth != null) // 플레이어일 경우
        {
            playerHealth.TakeDamage(damage);
            this.currentHp = playerHealth.GetCurrentHealth();
            
            if(this.currentHp <= 0)
            {
                if (battleController != null)
                {
                    battleController.OnCharacterDefeated(this);
                }
            }
        }
        else // 보스일 경우
        {
            currentHp -= Mathf.Max(1, damage - defensePower);
            Debug.Log($"{characterName}이(가) {damage}의 피해를 입었습니다! 남은 체력: {currentHp}");
            if (currentHp <= 0)
            {
                if (battleController != null)
                {
                    battleController.OnCharacterDefeated(this);
                }
                gameObject.SetActive(false);
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
}