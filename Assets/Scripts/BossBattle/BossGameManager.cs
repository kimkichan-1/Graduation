using UnityEngine;

public enum GameState { Exploration, Battle }

public class BossGameManager : MonoBehaviour
{
    public static BossGameManager Instance;

    [Header("관리 대상 컨트롤러")]
    public PlayerController playerController;
    public BattleController battleController;

    public GameState currentState = GameState.Exploration;

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
        // 씬에 있는 PlayerController를 자동으로 찾아서 연결합니다.
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (GameManager.Instance == null || !GameManager.Instance.isLoadingGame)
        {
            ChangeState(GameState.Exploration);
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        
        switch (currentState)
        {
            case GameState.Exploration:
                if (playerController != null)
                {
                    playerController.enabled = true;
                    playerController.canMove = true;
                }
                if (battleController != null)
                {
                    battleController.gameObject.SetActive(false);
                }
                break;
                
            case GameState.Battle:
                if (playerController != null)
                {
                    playerController.enabled = true; // 스크립트는 켜두되
                    playerController.canMove = false; // 움직임만 막음
                }
                if (battleController != null)
                {
                    battleController.gameObject.SetActive(true);
                }
                break;
        }
    }
    /// <summary>
    /// 메인 메뉴로 돌아갈 때 모든 static 변수와 상태를 초기화합니다.
    /// </summary>
    public static void ResetStaticVariables()
    {
        if (Instance != null)
        {
            // Instance 자체는 DontDestroyOnLoadManager 로직에 의해
            // 파괴될 수도 있고, 수동으로 파괴해야 할 수도 있습니다.
            // 가장 안전한 방법은 Instance를 null로 만들고,
            // 혹시 모를 상황에 대비해 상태도 리셋하는 것입니다.
            Instance.currentState = GameState.Exploration;
            Instance = null;
            Debug.Log("BossGameManager static 변수 리셋 완료.");
        }
    }
}