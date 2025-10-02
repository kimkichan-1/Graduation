using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 1000;
    public int defense = 20;
    private int currentHealth;

    [Header("UI")]
    public Slider healthBar;
    public GameObject damageTextPrefab;
    public Vector3 damageTextOffset = new Vector3(0, 2f, 0);

    [Header("Audio")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private float deathAnimationDuration = 1.5f;

    [Header("Status")]
    private bool isDead = false;
    public bool IsDead => isDead;

    private Animator animator;
    private AudioSource audioSource;
    
    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
            healthBar.gameObject.SetActive(true);
        }
    }

    // 플레이어의 공격에 대한 공통 데미지 처리 메서드
    public void TakeDamage(int finalDamage)
    {
        if (isDead) return;

        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (damageTextPrefab != null)
        {
            Vector3 spawnPosition = transform.position + damageTextOffset;
            GameObject damageTextObj = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity);
            DamageText damageText = damageTextObj.GetComponent<DamageText>();
            if (damageText != null)
                damageText.SetDamage(finalDamage);
        }

        if (animator != null)
            animator.SetTrigger("isHurt");

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        
        if (animator != null)
            animator.SetTrigger("isDead");

        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        // 보스전 종료 로직
        Debug.Log("보스가 쓰러졌습니다! 게임 클리어!");
        
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        Destroy(gameObject);
    }
}