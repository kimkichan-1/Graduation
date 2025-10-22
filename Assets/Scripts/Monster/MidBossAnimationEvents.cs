using UnityEngine;

/// <summary>
/// Animation Event를 부모의 MidBoss AI로 전달하는 브릿지 스크립트
/// Sprite 자식 오브젝트에 부착합니다.
/// Stage1MidBossAI와 Stage2MidBossAI 모두 지원합니다.
/// </summary>
public class MidBossAnimationEvents : MonoBehaviour
{
    private Stage1MidBossAI stage1AI;
    private Stage2MidBossAI stage2AI;

    void Awake()
    {
        // Stage1 AI 찾기
        stage1AI = GetComponentInParent<Stage1MidBossAI>();

        // Stage2 AI 찾기
        stage2AI = GetComponentInParent<Stage2MidBossAI>();

        if (stage1AI == null && stage2AI == null)
        {
            Debug.LogError("[MidBossAnimationEvents] 부모에서 Stage1MidBossAI 또는 Stage2MidBossAI를 찾을 수 없습니다!");
        }
        else if (stage1AI != null)
        {
            Debug.Log("[MidBossAnimationEvents] Stage1MidBossAI 연결 성공!");
        }
        else if (stage2AI != null)
        {
            Debug.Log("[MidBossAnimationEvents] Stage2MidBossAI 연결 성공!");
        }
    }

    // Animation Event 함수들 - 부모로 전달
    public void EnableAttackHitbox()
    {
        if (stage1AI != null) stage1AI.EnableAttackHitbox();
        if (stage2AI != null) stage2AI.EnableAttackHitbox();
    }

    public void DisableAttackHitbox()
    {
        if (stage1AI != null) stage1AI.DisableAttackHitbox();
        if (stage2AI != null) stage2AI.DisableAttackHitbox();
    }

    public void PlayAttackSound()
    {
        if (stage1AI != null) stage1AI.PlayAttackSound();
        if (stage2AI != null) stage2AI.PlayAttackSound();
    }

    public void PlayWalkSound()
    {
        if (stage1AI != null) stage1AI.PlayWalkSound();
        if (stage2AI != null) stage2AI.PlayWalkSound();
    }
}
