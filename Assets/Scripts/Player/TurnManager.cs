using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    public PlayerController player;
    public BossAI boss;

    private int turnCount = 0;
    private bool isPlayerActionComplete = false;
    private bool isBossActionComplete = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // 플레이어가 트리거에 진입하면 호출될 메서드
    public void StartBossFight()
    {
        // 보스전 시작 알림 UI 표시 (선택사항)
        Debug.Log("보스전 시작!");
        
        // 플레이어의 자유로운 이동을 비활성화
        if (player != null)
        {
            player.enabled = false;
        }

        // 보스 AI 활성화
        if (boss != null)
        {
            boss.enabled = true;
        }
        
        // 첫 번째 턴 시작
        StartCoroutine(InitialTurnDelay());
    }

    private IEnumerator InitialTurnDelay()
    {
        // 보스전 시작 효과를 위해 잠시 대기
        yield return new WaitForSeconds(2f);
        StartNewTurn();
    }

    private void Start()
    {
        StartNewTurn();
    }

    private void StartNewTurn()
    {
        turnCount++;
        Debug.Log($"<color=yellow>--- 턴 {turnCount} 시작 ---</color>");

        isPlayerActionComplete = false;
        isBossActionComplete = false;

        player.StartPlanningPhase();
        boss.StartPlanningPhase();
    }
    
    // 플레이어가 Space 키를 눌렀을 때 호출
    public void StartTurnExecution()
    {
        player.ExecuteTurn();
        boss.ExecuteTurn();
    }
    
    // 플레이어의 모든 행동이 끝났을 때 호출
    public void OnPlayerActionsComplete()
    {
        isPlayerActionComplete = true;
        CheckForNextTurn();
    }

    // 보스의 모든 행동이 끝났을 때 호출
    public void OnBossActionsComplete()
    {
        isBossActionComplete = true;
        CheckForNextTurn();
    }
    
    private void CheckForNextTurn()
    {
        if (isPlayerActionComplete && isBossActionComplete)
        {
            StartNewTurn();
        }
    }
}