using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int baseMaxHealth = 100; // 기본 최대 체력
    public int maxHealth = 100;     // 실제 최대 체력 (무기 효과 반영)
    public int defense = 0;         // 방어력
    private int currentHealth;      // 현재 체력 (private)

    [Header("UI")]
    public Slider healthSlider;     // 체력바 UI (Slider)
    public Text healthText;         // 체력 숫자 UI 텍스트

    [Header("Animator")]
    public Animator animator;       // 플레이어의 애니메이터 (Hurt, Die 등 애니메이션 제어)

    [Header("Events")]
    public UnityEvent onPlayerDeath; // 사망 시 실행될 이벤트 (예: 게임 오버 UI 호출 등)

    [Header("Invincibility")]
    public float invincibilityDuration = 1.0f; // 데미지 후 기본 무적 지속 시간
    private bool isInvincible = false;         // 현재 무적 상태인지 여부

    [Header("Audio")]
    [SerializeField] private AudioClip damageSound; // 데미지 사운드 클립
    private AudioSource audioSource;                // 사운드 재생을 위한 컴포넌트

    private bool isDead = false;                   // 사망 상태인지 여부

    private void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        UpdateHealthUI();
    }

    public void ApplyBonusHealth(int bonusHealth)
    {
        int previousMaxHealth = maxHealth;
        maxHealth = baseMaxHealth + bonusHealth;
        float healthPercent = (float)currentHealth / previousMaxHealth;
        currentHealth = Mathf.RoundToInt(maxHealth * healthPercent);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        if (isDead || isInvincible) return;

        int finalDamage = Mathf.Max(1, amount - defense);
        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("isHurt");
        }

        StartCoroutine(StartInvincibility(blink: true));
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            // 항상 maxValue를 먼저 업데이트하여 슬라이더의 비율이 정확하게 계산되도록 함
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    public void Die()
    {
        isDead = true;
        Debug.Log("Player has died.");
        animator.SetTrigger("isDead");
        onPlayerDeath?.Invoke();
    }

    public void AddPermanentHealth(int amount)
    {
        baseMaxHealth += amount;
        maxHealth += amount;
        Heal(amount); // Heal이 UpdateHealthUI를 호출하여 모든 UI를 갱신
    }

    public IEnumerator StartInvincibility(float duration = -1f, bool blink = true)
    {
        isInvincible = true;
        float invincibilityTime = duration < 0f ? invincibilityDuration : duration;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float blinkInterval = 0.1f;
        float elapsedTime = 0f;

        if (blink && spriteRenderer != null)
        {
            while (elapsedTime < invincibilityTime)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                elapsedTime += blinkInterval;
                yield return new WaitForSeconds(blinkInterval);
            }
            spriteRenderer.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(invincibilityTime);
        }
        isInvincible = false;
    }

    public int GetCurrentHealth() => currentHealth;

    public void ResetHealth()
    {
        isDead = false;
        currentHealth = maxHealth;
        UpdateHealthUI();
    }
}
