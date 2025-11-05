using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScarecrowHealth : MonoBehaviour
{
    public int maxHealth = 300; // 최대 체력
    public int defense = 0;     // 방어력
    private int currentHealth;  // 현재 체력
    private Animator animator;  // 애니메이터 컴포넌트
    private AudioSource audioSource; // 사운드 재생을 위한 컴포넌트
    private bool isDead = false; // 사망 상태 플래그

    public Slider healthBar;          // 체력바 UI
    public GameObject damageTextPrefab; // 데미지 텍스트 프리팹
    public Vector3 damageTextOffset = new Vector3(0, 1f, 0); // 데미지 텍스트 표시 오프셋

    [SerializeField] private AudioClip deathSound; // 사망 사운드 클립
    [SerializeField] private AudioClip hitSound;   // 피격 사운드 클립
    [SerializeField] private float deathAnimationDuration = 1f; // 사망 애니메이션 지속 시간 (초)

    private void Start()
    {
        currentHealth = maxHealth; // 초기 체력 설정
        animator = GetComponent<Animator>(); // 애니메이터 초기화
        audioSource = GetComponent<AudioSource>(); // AudioSource 초기화

        // AudioSource가 없으면 추가
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth; // 체력바 최대값 설정
            healthBar.value = currentHealth; // 초기 체력바 값 설정
            healthBar.gameObject.SetActive(false); // 처음에는 체력바 숨김
        }
    }

    // Sword 데미지 처리 메서드
    // amount: 기본 공격력, attackType: 공격 타입 (1 = Attack1, 2 = Attack2)
    public void TakeDamageSword(int amount, int attackType)
    {
        // 사망 상태면 데미지 처리하지 않음
        if (isDead) return;

        // 최종 데미지 계산
        float damageMultiplier = Random.Range(0.85f, 1.15f); // 0.85~1.15 사이의 랜덤 배율
        int finalDamage;

        if (attackType == 1) // Attack1 (단독 또는 콤보)
        {
            // (attackPower - defense) * Random.Range(0.85f, 1.15f) / 2
            finalDamage = Mathf.RoundToInt(((amount - defense) * damageMultiplier) / 2f);
        }
        else // Attack2
        {
            // (attackPower - defense) * Random.Range(0.85f, 1.15f)
            finalDamage = Mathf.RoundToInt((amount - defense) * damageMultiplier);
        }

        // 최소 데미지 1 보장
        finalDamage = Mathf.Max(finalDamage, 1);

        ApplyDamage(finalDamage);
    }

    // Mace 데미지 처리 메서드
    // amount: 기본 공격력, attackType: 공격 타입 (1 = Attack1, 2 = Attack2)
    public void TakeDamageMace(int amount, int attackType)
    {
        // 사망 상태면 데미지 처리하지 않음
        if (isDead) return;

        // 최종 데미지 계산
        float damageMultiplier = Random.Range(0.85f, 1.15f); // 0.85~1.15 사이의 랜덤 배율
        int finalDamage;

        if (attackType == 1) // Attack1 (단독 또는 콤보)
        {
            // (attackPower - defense) * Random.Range(0.85f, 1.15f)
            finalDamage = Mathf.RoundToInt((amount - defense) * damageMultiplier);
        }
        else // Attack2
        {
            // (attackPower - defense) * Random.Range(0.85f, 1.15f) * 1.1f
            finalDamage = Mathf.RoundToInt(((amount - defense) * damageMultiplier) * 1.1f);
        }

        // 최소 데미지 1 보장
        finalDamage = Mathf.Max(finalDamage, 1);

        ApplyDamage(finalDamage);
    }

    // Lance 데미지 처리 메서드
    // amount: 기본 공격력, attackType: 공격 타입 (1 = Attack1, 2 = Attack2)
    public void TakeDamageLance(int amount, int attackType)
    {
        // 사망 상태면 데미지 처리하지 않음
        if (isDead) return;

        // 최종 데미지 계산
        float damageMultiplier = Random.Range(0.85f, 1.15f); // 0.8~1.2 사이의 랜덤 배율
        int finalDamage;

        if (attackType == 1) // Attack1 (단독 또는 콤보)
        {
            // (attackPower - defense) * Random.Range(0.85f, 1.15f) / 2f
            finalDamage = Mathf.RoundToInt(((amount - defense) * damageMultiplier) / 2f);
        }
        else // Attack2
        {
            // (attackPower - defense) * Random.Range(0.85f, 1.15f) / 3f
            finalDamage = Mathf.RoundToInt(((amount - defense) * damageMultiplier) / 3f);
        }

        // 최소 데미지 1 보장
        finalDamage = Mathf.Max(finalDamage, 1);

        ApplyDamage(finalDamage);
    }

    // 공통 데미지 적용 로직
    private void ApplyDamage(int finalDamage)
    {
        // 체력 감소 및 클램핑
        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // 피격 사운드 재생
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // 데미지 텍스트 생성 (월드 공간)
        if (damageTextPrefab != null)
        {
            Vector3 spawnPosition = transform.position + damageTextOffset;
            GameObject damageTextObj = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, GameObject.Find("DamageTextCanvas").transform);
            DamageText damageText = damageTextObj.GetComponent<DamageText>();
            if (damageText != null)
                damageText.SetDamage(finalDamage);
        }

        // 피격 애니메이션 재생
        if (animator != null)
            animator.SetTrigger("isHurt");

        // 체력바 업데이트 및 표시
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
            healthBar.gameObject.SetActive(true); // 공격받으면 체력바 표시
        }

        // 체력이 0 이하일 경우 사망 처리
        if (currentHealth <= 0)
            Die();
    }

    // 사망 처리 메서드
    private void Die()
    {
        // 이미 사망 상태면 중복 처리 방지
        if (isDead) return;

        isDead = true; // 사망 상태로 전환

        if (animator != null)
            animator.SetTrigger("isDead");

        // 사망 사운드 재생
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // 사망 애니메이션 재생 후 파괴
        StartCoroutine(DestroyAfterAnimation());
    }

    // 사망 애니메이션 재생 후 오브젝트 파괴
    private IEnumerator DestroyAfterAnimation()
    {
        // 사망 애니메이션 지속 시간 대기
        yield return new WaitForSeconds(deathAnimationDuration);

        Debug.Log("허수아비가 쓰러졌습니다.");
        Destroy(gameObject); // 오브젝트 파괴
    }
}