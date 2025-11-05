// 파일명: LifeGameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LifeGameManager : MonoBehaviour
{
    public static LifeGameManager Instance { get; private set; }

    public int finalTapCount = 0;
    public float regenBuffValue = 0f;
    public Vector3 playerPositionBeforePortal;
    public string sceneNameBeforePortal;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!string.IsNullOrEmpty(sceneNameBeforePortal) && scene.name == sceneNameBeforePortal)
        {
            StartCoroutine(RestoreSceneState());
        }
    }

    IEnumerator RestoreSceneState()
    {
        yield return null;

        // ★★★ 이 부분을 추가하세요 ★★★
        // 혹시라도 시간이 멈춰있을 경우를 대비해, 씬 복구 시 시간을 무조건 1로 되돌립니다.
        Time.timeScale = 1f;
        // --- 여기까지 ---

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = playerPositionBeforePortal;
        }

        if (Camera.main != null)
        {
            Camera.main.cullingMask = -1;
        }

        sceneNameBeforePortal = null;
    }

    public void ProcessMinigameResult(int tapCount)
    {
        finalTapCount = tapCount;
        if (tapCount < 30) regenBuffValue = 0f;
        else if (tapCount < 50) regenBuffValue = 0.5f;
        else if (tapCount < 100) regenBuffValue = 1.0f;
        else regenBuffValue = 2.0f;
        Debug.Log($"미니게임 종료! 최종 점수: {finalTapCount}");
    }

    public void ApplyImmediateHeal()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("체력을 회복시킬 플레이어를 찾을 수 없습니다!");
            return;
        }

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("플레이어에게 PlayerHealth 스크립트가 없습니다!");
            return;
        }

        // --- ▼▼▼▼▼ 수정된 부분 (회복량 계산 로직) ▼▼▼▼▼ ---
        int healAmount = 0;
        if (finalTapCount < 30)
        {
            healAmount = 10;
        }
        else if (finalTapCount < 50) // 30 이상, 50 미만
        {
            healAmount = 20;
        }
        else if (finalTapCount < 100) // 50 이상, 100 미만
        {
            healAmount = 30;
        }
        else // 100 이상
        {
            healAmount = 50;
        }
        // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---

        // 회복량이 0보다 클 때만 회복 실행 및 메시지 표시
        if (healAmount > 0)
        {
            playerHealth.Heal(healAmount);
            Debug.Log($"미니게임 보상으로 체력을 {healAmount}만큼 즉시 회복했습니다!");
        }
        else
        {
            Debug.Log("미니게임 점수가 낮아 체력 회복 보상이 없습니다.");
        }
    }
}