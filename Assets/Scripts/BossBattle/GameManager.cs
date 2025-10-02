// GameManager.cs (업그레이드 버전)

using UnityEngine;

// 게임의 상태를 명확히 정의하는 열거형(enum)
public enum GameState { Exploration, Battle }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 어디서든 접근할 수 있는 싱글톤

    [Header("관리 대상 컨트롤러")]
    public PlayerController playerController;
    public BattleController battleController;
    
    public GameState currentState; // 현재 게임 상태

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 게임 시작 시에는 항상 필드 상태로 시작
        ChangeState(GameState.Exploration);
    }

    // 상태를 전환하는 핵심 함수
    public void ChangeState(GameState newState)
{
    currentState = newState;
    
    switch (currentState)
    {
        case GameState.Exploration:
            // 필드 상태: 플레이어가 움직일 수 있도록 허용하고, 전투 컨트롤러는 비활성화
            if (playerController != null)
            {
                playerController.enabled = true; // 스크립트는 항상 켜 둡니다.
                playerController.canMove = true;
            }
            battleController.gameObject.SetActive(false);
            break;
            
        case GameState.Battle:
            // 전투 상태: 플레이어의 움직임만 막고, 전투 컨트롤러는 활성화
            if (playerController != null)
            {
                playerController.enabled = true; // 스크립트는 항상 켜 둡니다.
                playerController.canMove = false;
            }
            battleController.gameObject.SetActive(true);
            break;
    }
}
}