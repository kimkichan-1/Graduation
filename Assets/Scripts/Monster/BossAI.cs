using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossAI : MonoBehaviour
{
    [Header("Turn-Based System")]
    public ActionData bossAttackAction; // 공격 행동 데이터

    private List<ActionData> actionBuffer = new List<ActionData>();
    
    public void StartPlanningPhase()
    {
        actionBuffer.Clear();
        // 턴 시작 시 보스의 행동을 결정합니다.
        // 여기서는 간단하게 항상 공격 행동을 선택합니다.
        actionBuffer.Add(bossAttackAction);
        
        Debug.Log("보스 턴 계획 시작.");
    }
    
    public void ExecuteTurn()
    {
        // 버퍼에 담긴 행동을 실행합니다.
        StartCoroutine(ExecuteActions());
    }
    
    private IEnumerator ExecuteActions()
    {
        foreach (var action in actionBuffer)
        {
            if (action.type == ActionData.ActionType.Attack)
            {
                Debug.Log($"보스 공격 행동 실행: {action.actionName}");
                // 보스 공격 애니메이션 코루틴 시작
                // 예시: StartCoroutine(BossAttack());
                yield return new WaitForSeconds(action.duration);
            }
        }
        
        // 모든 행동이 끝나면 턴 종료를 알림
        TurnManager.instance.OnBossActionsComplete();
    }
}