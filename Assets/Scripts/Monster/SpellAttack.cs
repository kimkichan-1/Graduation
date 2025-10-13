using System.Collections;
using UnityEngine;

/// <summary>
/// Spell 공격 이펙트 스크립트 (통합 애니메이션 버전)
/// 마법진과 마법손이 하나의 애니메이션으로 구성된 경우 사용
/// </summary>
public class SpellAttack : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int baseDamage = 20; // 기본 데미지
    [SerializeField] private BoxCollider2D damageHitbox; // 데미지를 주는 히트박스

    [Header("Animation & Timing")]
    [SerializeField] private float warningDuration = 0.5f; // 경고 시간 (데미지 없음)
    [SerializeField] private float attackStartTime = 0.5f; // 공격 시작 시간
    [SerializeField] private float attackEndTime = 1.5f; // 공격 종료 시간
    [SerializeField] private float totalLifetime = 2.5f; // 전체 이펙트 생존 시간

    [Header("Visual Effects")]
    [SerializeField] private Animator spellAnimator; // Spell 애니메이터
    [SerializeField] private SpriteRenderer spellRenderer; // Spell 스프라이트 렌더러
    [SerializeField] private Color warningColor = new Color(1f, 1f, 1f, 0.5f); // 경고 단계 색상 (반투명)

    [Header("Sound Effects")]
    [SerializeField] private AudioClip spellAppearSound; // Spell 등장 사운드
    [SerializeField] private AudioClip spellAttackSound; // Spell 공격 사운드

    private AudioSource audioSource;
    private bool hasDealtDamage = false; // 데미지를 이미 줬는지 체크

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 히트박스 비활성화
        if (damageHitbox != null)
        {
            damageHitbox.enabled = false;
        }

        // 애니메이터가 설정되어 있으면 애니메이션 재생
        if (spellAnimator != null)
        {
            spellAnimator.SetTrigger("Play"); // Spell 애니메이션 트리거
        }

        // 공격 시퀀스 시작
        StartCoroutine(SpellAttackSequence());

        // 전체 생존 시간 후 자동 파괴
        Destroy(gameObject, totalLifetime);
    }

    /// <summary>
    /// 외부에서 데미지 초기화 (보스 AI에서 호출)
    /// </summary>
    public void Initialize(int damage)
    {
        baseDamage = damage;
    }

    private IEnumerator SpellAttackSequence()
    {
        // 1단계: 경고 (스프라이트 반투명, 데미지 없음)
        if (spellRenderer != null)
        {
            spellRenderer.color = warningColor;
        }

        if (spellAppearSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(spellAppearSound);
        }

        yield return new WaitForSeconds(warningDuration);

        // 2단계: 공격 시작 (스프라이트 불투명, 데미지 활성화)
        if (spellRenderer != null)
        {
            spellRenderer.color = Color.white; // 완전 불투명
        }

        if (spellAttackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(spellAttackSound);
        }

        // 히트박스 활성화
        if (damageHitbox != null)
        {
            damageHitbox.enabled = true;
        }

        // 공격 지속 시간
        float attackDuration = attackEndTime - attackStartTime;
        yield return new WaitForSeconds(attackDuration);

        // 3단계: 공격 종료 (히트박스 비활성화)
        if (damageHitbox != null)
        {
            damageHitbox.enabled = false;
        }

        // 나머지 시간 동안 애니메이션 재생 (자동 파괴는 totalLifetime에서)
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어에게만 데미지 (한 번만)
        if (!hasDealtDamage && other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(baseDamage);
                hasDealtDamage = true; // 중복 데미지 방지
                Debug.Log($"Spell이 플레이어에게 {baseDamage} 데미지를 입혔습니다!");
            }
        }
    }

    // 애니메이션 이벤트용 함수 (선택사항 - 애니메이션에서 직접 타이밍 제어 시 사용)
    public void EnableHitbox()
    {
        if (damageHitbox != null)
        {
            damageHitbox.enabled = true;
        }
    }

    public void DisableHitbox()
    {
        if (damageHitbox != null)
        {
            damageHitbox.enabled = false;
        }
    }

    public void PlayAttackSound()
    {
        if (spellAttackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(spellAttackSound);
        }
    }
}
