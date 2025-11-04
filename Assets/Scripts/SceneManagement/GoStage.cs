using UnityEngine;
using UnityEngine.SceneManagement;

public class GoStage : MonoBehaviour
{
    [Header("다음 스테이지 설정")]
    [SerializeField] private string nextStageName;

    private bool isPlayerInRange = false;

    private void Update()
    {
        // 플레이어가 범위 안에 있고 'W' 키를 눌렀을 때
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.W))
        {
            // 지정된 다음 스테이지 씬 로드
            SceneManager.LoadScene(nextStageName);
        }
    }

    // 플레이어가 트리거 범위에 들어왔을 때 호출
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        { 
            isPlayerInRange = true;
        }
    }

    // 플레이어가 트리거 범위에서 나갔을 때 호출
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
