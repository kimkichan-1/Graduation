using UnityEngine;

/// <summary>
/// Animation Event를 부모의 Stage1MidBossAI로 전달하는 브릿지 스크립트
/// Sprite 자식 오브젝트에 부착합니다.
/// </summary>
public class MidBossAnimationEvents : MonoBehaviour
{
    private Stage1MidBossAI parentAI;

    void Awake()
    {
        // 부모의 AI 스크립트 찾기
        parentAI = GetComponentInParent<Stage1MidBossAI>();

        if (parentAI == null)
        {
            Debug.LogError("[MidBossAnimationEvents] 부모에서 Stage1MidBossAI를 찾을 수 없습니다!");
        }
    }

    // Animation Event 함수들 - 부모로 전달
    public void EnableAttackHitbox()
    {
        if (parentAI != null) parentAI.EnableAttackHitbox();
    }

    public void DisableAttackHitbox()
    {
        if (parentAI != null) parentAI.DisableAttackHitbox();
    }

    public void PlayAttackSound()
    {
        if (parentAI != null) parentAI.PlayAttackSound();
    }

    public void PlayWalkSound()
    {
        if (parentAI != null) parentAI.PlayWalkSound();
    }
}
