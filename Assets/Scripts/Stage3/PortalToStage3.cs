using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리 기능을 사용하기 위해 필수

public class PortalToStage3 : MonoBehaviour
{
    // --- ▼▼▼ 수정된 부분 (플레이어 감지 변수 추가) ▼▼▼ ---
    // 플레이어가 포탈 범위 안에 있는지 확인하는 스위치
    private bool isPlayerNear = false;
    // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---

    // 이 함수는 플레이어가 포탈 범위 안으로 '들어왔을 때' 한 번 호출됩니다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어가 근처에 있다고 표시
            isPlayerNear = true;
            // (선택사항) 여기에 "[W]키를 누르세요" 같은 UI를 띄우면 더 좋습니다.
        }
    }

    // --- ▼▼▼ 수정된 부분 (Update 함수 추가) ▼▼▼ ---
    // Update 함수는 매 프레임마다 계속 실행됩니다.
    private void Update()
    {
        // 만약 플레이어가 근처에 있고(isPlayerNear == true), W키를 눌렀다면
        if (isPlayerNear && Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("W키를 눌렀습니다. Stage3으로 이동합니다.");

            // "Stage3"라는 이름의 씬을 불러옵니다.
            SceneManager.LoadScene("Stage3");
        }
    }
    // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---

    // 이 함수는 플레이어가 포탈 범위 밖으로 '나갔을 때' 한 번 호출됩니다.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어가 멀어졌다고 표시
            isPlayerNear = false;
        }
    }
}
