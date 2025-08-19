using UnityEngine;

public class MaceAttack : MonoBehaviour
{
    public BoxCollider2D maceHitbox; // 메이스스의 충돌 감지 영역 (인스펙터에서 할당)
    private PlayerController playerController; // 플레이어 상태 및 무기 스탯 참조
    private PlayerStats playerStats;           // 플레이어 영구 스탯 참조
    private Animator animator;                // 애니메이션 상태 확인을 위한 Animator

    // 초기화
    private void Start()
    {
        // 필요한 컴포넌트 가져오기
        playerController = GetComponent<PlayerController>();
        playerStats = GetComponent<PlayerStats>();
        animator = GetComponent<Animator>();
        if (maceHitbox != null)
            maceHitbox.enabled = false; // 초기에는 히트박스 비활성화
    }

    // 공격 애니메이션 이벤트: 히트박스 활성화
    public void EnableMaceHitbox()
    {
        if (maceHitbox != null)
            maceHitbox.enabled = true; // 히트박스 활성화
    }

    // 공격 애니메이션 이벤트: 히트박스 비활성화
    public void DisableMaceHitbox()
    {
        if (maceHitbox != null)
            maceHitbox.enabled = false; // 히트박스 비활성화
    }

    // 히트박스가 적과 충돌했을 때 호출
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (maceHitbox != null && maceHitbox.enabled && other.CompareTag("Enemy"))
        {
            var enemyHealth = other.GetComponent<MonsterHealth>();
            if (enemyHealth != null && playerController != null && playerController.currentWeaponStats != null && playerStats != null)
            {
                // 기본 공격력(무기) + 영구 보너스 공격력(레벨업)
                int totalAttackPower = playerController.currentWeaponStats.attackPower + playerStats.bonusAttackPower;

                int attackType = IsPlayingAttack2Animation() ? 2 : 1;

                enemyHealth.TakeDamageMace(totalAttackPower, attackType);
            }
        }
    }

    private bool IsPlayingAttack2Animation()
    {
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("Attack2_Mace");
        }
        return false;
    }
}
