using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MonsterHealth : MonoBehaviour
{
    [Header("UI 우선순위 설정")]
    [Tooltip("다른 UI(레벨업 등)가 먼저 나올 수 있도록 대화창을 띄우기 전 대기하는 시간(초)")]
    [SerializeField] private float dialogueDelay = 0.5f;
    // --- ★★★ 이 부분을 추가하세요 ★★★ ---
    [Header("이벤트 연동")]
    [Tooltip("이 몬스터가 죽었을 때 활성화시킬 석상 (선택사항)")]
    [TextArea(3, 10)] // Inspector에서 여러 줄로 편하게 입력
    public string[] deathDialogue;
    public StatueInteraction targetStatue;
    // --- 여기까지 ---

    public int maxHealth = 300; // 최대 체력
    public int defense = 0;     // 방어력
    public int xpValue = 10;    // 사망 시 제공할 경험치
    private int currentHealth;  // 현재 체력
    private Animator animator;  // 애니메이터 컴포넌트
    private AudioSource audioSource; // 사운드 재생을 위한 컴포넌트
    private bool isDead = false; // 사망 상태 플래그
    public bool IsDead => isDead;   // 외부에서 사망 상태 확인용 프로퍼티
    private SpriteRenderer spriteRenderer; // 스프라이트 렌더러 (색상 변경용)

    public Slider healthBar;          // 체력바 UI
    public GameObject damageTextPrefab; // 데미지 텍스트 프리팹
    public Vector3 damageTextOffset = new Vector3(0, 1f, 0); // 데미지 텍스트 표시 오프셋

    [Header("타격 이펙트")]
    public GameObject hitEffectPrefab; // 타격 이펙트 프리팹
    public float hitEffectDuration = 0.5f; // 타격 이펙트 지속 시간
    [Tooltip("몬스터 스프라이트 범위 (X축 최소/최대, Y축 최소/최대)")]
    public Vector2 randomRangeX = new Vector2(-0.5f, 0.5f); // X축 랜덤 범위
    public Vector2 randomRangeY = new Vector2(-0.5f, 0.5f); // Y축 랜덤 범위

    [Header("피격 반응 설정")]
    [Range(0f, 1f)]
    [Tooltip("Hurt 애니메이션이 재생될 확률 (0.0 = 0%, 1.0 = 100%)")]
    public float hurtAnimationProbability = 0.3f; // Hurt 애니메이션 확률 (기본: 30%)
    public float flashDuration = 0.1f; // 스프라이트 깜빡임 지속 시간

    [SerializeField] private AudioClip deathSound; // 사망 사운드 클립
    [SerializeField] private AudioClip hitSound;   // 피격 사운드 클립
    [SerializeField] private float deathAnimationDuration = 1f; // 사망 애니메이션 지속 시간 (초)

    private void Start()
    {
        currentHealth = maxHealth; // 초기 체력 설정
        animator = GetComponent<Animator>(); // 애니메이터 초기화
        audioSource = GetComponent<AudioSource>(); // AudioSource 초기화
        spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer 초기화

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

        // 피격 사운드 재생 (사망 시에도 재생)
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // 타격 이펙트 생성 (사망 시에도 생성)
        if (hitEffectPrefab != null)
        {
            // 랜덤 오프셋 계산
            float randomX = Random.Range(randomRangeX.x, randomRangeX.y);
            float randomY = Random.Range(randomRangeY.x, randomRangeY.y);
            Vector3 randomOffset = new Vector3(randomX, randomY, 0);

            Vector3 effectPosition = transform.position + randomOffset;
            GameObject effect = Instantiate(hitEffectPrefab, effectPosition, Quaternion.identity);

            // 몬스터의 방향에 따라 이펙트도 flip
            Vector3 effectScale = effect.transform.localScale;
            effectScale.x = transform.localScale.x > 0 ? Mathf.Abs(effectScale.x) : -Mathf.Abs(effectScale.x);
            effect.transform.localScale = effectScale;

            Destroy(effect, hitEffectDuration); // 지정된 시간 후 자동 삭제
        }

        // 데미지 텍스트 생성 (사망 시에도 생성)
        if (damageTextPrefab != null)
        {
            Vector3 spawnPosition = transform.position + damageTextOffset;
            GameObject damageTextObj = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, GameObject.Find("DamageTextCanvas").transform);
            DamageText damageText = damageTextObj.GetComponent<DamageText>();
            if (damageText != null)
                damageText.SetDamage(finalDamage);
        }

        // 체력바 업데이트 및 표시
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
            healthBar.gameObject.SetActive(true); // 공격받으면 체력바 표시
        }

        // 체력이 0 이하일 경우 즉시 사망 플래그 설정 및 사망 처리
        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
            return; // Hurt 애니메이션 실행 방지
        }

        // 확률 기반으로 Hurt 애니메이션 재생 여부 결정
        float randomValue = Random.Range(0f, 1f);

        if (randomValue < hurtAnimationProbability)
        {
            // 확률에 따라 Hurt 애니메이션 재생
            if (animator != null)
                animator.SetTrigger("isHurt");
        }
        else
        {
            // 확률에 해당하지 않으면 스프라이트만 하얗게 깜빡임
            if (spriteRenderer != null)
                StartCoroutine(FlashWhite());
        }
    }

    // 스프라이트를 하얗게 깜빡이게 하는 코루틴
    private IEnumerator FlashWhite()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    // 사망 처리 메서드
    private void Die()
    {
        // isDead는 이미 ApplyDamage에서 true로 설정됨
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        // 1. 경험치 우선 지급 (레벨업 이벤트 발생)
        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats != null) playerStats.AddXp(xpValue);

        // 2. 사망 애니메이션, 사운드 등 즉시 처리
        if (animator != null) animator.SetBool("isDead", true);
        if (deathSound != null) audioSource.PlayOneShot(deathSound);
        if (healthBar != null) healthBar.gameObject.SetActive(false);

        // 3. 레벨업 창이 먼저 뜰 수 있도록 잠시 대기
        yield return new WaitForSeconds(dialogueDelay);

        // 4. 보스 처치 대화창 이벤트 실행
        MidBossController bossEventController = GetComponent<MidBossController>();
        if (bossEventController != null)
        {
            bossEventController.TriggerDeathEvent();
        }

        // 5. 아이템 드랍
        ItemDrop itemDropper = GetComponent<ItemDrop>();
        if (itemDropper != null)
        {
            itemDropper.GenerateDrops();
        }

        // --- ★★★ 수정된 부분: 직접 파괴하는 대신, DestroyAfterAnimation 코루틴을 호출합니다. ★★★ ---
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