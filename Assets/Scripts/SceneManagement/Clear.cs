using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 'Stage3' 포탈에 붙여서, 플레이어가 닿은 상태에서 'W'키를 누르면
/// 게임 클리어 씬을 로드하는 스크립트입니다.
/// </summary>
public class Clear : MonoBehaviour
{
    private bool isCleared = false; // 중복 실행 방지
    private bool playerIsInside = false; // 플레이어가 트리거 안에 있는지 확인

    // ※ 'ClearScene'이 Build Settings에 추가되었는지 꼭 확인하세요! ※

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 플레이어가 트리거에 들어오면, 안에 있다고 표시
        if (other.CompareTag("Player"))
        {
            playerIsInside = true;
            // (선택) 여기에 "W키를 눌러 상호작용" UI를 띄워도 좋습니다.
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 2. 플레이어가 트리거에서 나가면, 밖에 있다고 표시
        if (other.CompareTag("Player"))
        {
            playerIsInside = false;
            // (선택) 상호작용 UI를 숨깁니다.
        }
    }

    private void Update()
    {
        // 3. 플레이어가 트리거 안에 있고, 'W' 키를 눌렀으며, 아직 클리어 안 했다면
        if (playerIsInside && Input.GetKeyDown(KeyCode.W) && !isCleared)
        {
            isCleared = true; // 중복 실행 방지

            Debug.Log("게임 클리어! (테스트용 Stage3 포탈 진입)");

            // 4. (★중요★) 현재 상태를 마지막으로 저장합니다.
            // 'GameManager'가 살아있다면 AutoSave()를 호출합니다.
            if (GameManager.Instance != null)
            {
                // null을 전달하면 현재 플레이어 위치로 저장됩니다.
                GameManager.Instance.AutoSave(null);
                Debug.Log("클리어 직전, 최종 상태를 저장했습니다.");
            }
            else
            {
                Debug.LogWarning("GameManager가 없어서 클리어 저장을 못했습니다.");
            }

            // 5. (선택) 게임 시간을 멈춰서 모든 움직임을 정지시킵니다.
            Time.timeScale = 0f;

            // 6. 'ClearScene'을 로드합니다.
            SceneManager.LoadScene("ClearScene");
        }
    }
}
