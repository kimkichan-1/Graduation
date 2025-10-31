using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class CharacterStats : MonoBehaviour
{
    public BattleController battleController;
    private PlayerController playerController;
    private PlayerHealth playerHealth;
    private PlayerStats playerStats;
    private CharacterVisuals characterVisuals;

    [Header("전투용 스탯")]
    public string characterName = "사서";
    public int maxHp;
    public int currentHp;
    public int attackPower;
    public int defensePower;
    public float moveSpeed;

    [Header("체력바 UI")]
    public GameObject healthBarUIParent;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    [Header("빛(Light) 시스템")]
    public int maxLight = 3;
    public int currentLight;

    [Header("전투 책장 시스템")]
    public List<CombatPage> deck = new List<CombatPage>(9);
    public List<CombatPage> cardCollection = new List<CombatPage>();
    public Dictionary<CombatPage, int> cardCooldowns = new Dictionary<CombatPage, int>();
    public List<CombatPage> revealedCards = new List<CombatPage>();

    void Awake()
    {
        characterVisuals = GetComponent<CharacterVisuals>();
        
        if (battleController == null)
        {
            battleController = FindObjectOfType<BattleController>();
        }
        
        playerController = GetComponent<PlayerController>();
        playerHealth = GetComponent<PlayerHealth>();
        playerStats = GetComponent<PlayerStats>();
        
        // ▼▼▼ [수정] 이 루프를 삭제하거나 주석 처리합니다. ▼▼▼
        /*
        foreach (var page in deck)
        {
            if (!cardCooldowns.ContainsKey(page))
            {
                cardCooldowns.Add(page, 0);
            }
        }
        */
        // ▲▲▲
    }

    public void InitializeFromPlayerScripts()
    {
        if (playerController == null || playerHealth == null || playerStats == null)
        {
            this.currentHp = this.maxHp;
            UpdateHealthUI();
            return;
        }
        
        this.maxHp = playerHealth.maxHealth;
        this.currentHp = playerHealth.GetCurrentHealth();
        this.attackPower = playerController.attackPower + playerStats.bonusAttackPower;
        this.defensePower = playerController.defensePower + playerHealth.defense;
        this.moveSpeed = playerController.baseMoveSpeed + playerStats.bonusMoveSpeed;
        UpdateHealthUI();
    }
    
    public void TakeDamage(int damage)
    {
        if (battleController != null && characterVisuals != null && damage > 0)
        {
            float knockbackDistance = damage * battleController.knockbackPower;
            StartCoroutine(characterVisuals.Knockback(knockbackDistance, 0.2f));
            battleController.ApplyClashPointKnockback(this, knockbackDistance);
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            this.currentHp = playerHealth.GetCurrentHealth();
            UpdateHealthUI();
            
            if(this.currentHp <= 0 && battleController != null)
            {
                battleController.OnCharacterDefeated(this);
            }
        }
        else
        {
            currentHp -= Mathf.Max(1, damage - defensePower);
            if (currentHp < 0) currentHp = 0;
            
            UpdateHealthUI();

            if (currentHp <= 0 && battleController != null)
            {
                battleController.OnCharacterDefeated(this);
            }
        }
    }

    public void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHp;
            healthSlider.value = currentHp;
        }
        if (healthText != null)
        {
            healthText.text = $"{currentHp} / {maxHp}";
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
            if (cardCooldowns.ContainsKey(page) && cardCooldowns[page] > 0)
            {
                cardCooldowns[page]--;
            }
        }
    }

    public void SetCardCooldown(CombatPage page)
    {
        // ▼▼▼ [수정] 딕셔너리에 키가 있는지 먼저 확인합니다. ▼▼▼
        if (page == null) return;
        if (!cardCooldowns.ContainsKey(page))
        {
            cardCooldowns.Add(page, 0);
        }
        // ▲▲▲
        
        cardCooldowns[page] = page.lightCost;
        
        if (!revealedCards.Contains(page))
        {
            revealedCards.Add(page);
        }
    }

    public bool IsCardUsable(CombatPage page)
    {
        // ▼▼▼ [수정] 딕셔너리에 키가 있는지 먼저 확인합니다. ▼▼▼
        if (page == null) return false;
        if (!cardCooldowns.ContainsKey(page))
        {
            // 이 카드는 덱에 있지만 아직 딕셔너리에 없음 -> 지금 추가
            cardCooldowns.Add(page, 0); 
        }
        // ▲▲▲
        
        return cardCooldowns[page] == 0;
    }

    public void AddCardToCollection(CombatPage newCard)
    {
        if (newCard == null) return;
        cardCollection.Add(newCard);
        Debug.Log($"[CharacterStats] 카드 획득: {newCard.pageName}");
    }
}