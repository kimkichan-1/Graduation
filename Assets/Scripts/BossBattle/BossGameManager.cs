using UnityEngine;

public enum GameState { Exploration, Battle }

public class BossGameManager : MonoBehaviour
{
    public static BossGameManager Instance;

    [Header("관리 대상 컨트롤러")]
    public PlayerController playerController;
    public BattleController battleController;
    
    public GameState currentState;

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
        
        ChangeState(GameState.Exploration);
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
}