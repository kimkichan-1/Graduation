// OxygenZone.cs
using UnityEngine;

public class OxygenZone : MonoBehaviour
{
    // 다른 Collider가 이 영역에 들어왔을 때 호출됨
    // 다른 Collider가 이 영역에 들어왔을 때 호출됨
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 들어온 대상이 "Player" 태그를 가지고 있는지 먼저 확인
        if (other.CompareTag("Player"))
        {
            // --- ▼▼▼ 수정된 부분 ▼▼▼ ---
            // Debug.Log를 if문 안으로 옮겼습니다.
            // 이제 "Player" 태그를 가진 오브젝트가 들어왔을 때만 이 메시지가 출력됩니다.
            Debug.Log(other.gameObject.name + " 가(이) 충전 영역에 들어왔습니다!");
            // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲ ---

            // 플레이어에게서 PlayerOxygen 스크립트를 가져옴
            PlayerOxygen playerOxygen = other.GetComponent<PlayerOxygen>();
            if (playerOxygen != null)
            {
                // 플레이어의 산소 충전 시작 함수를 호출
                playerOxygen.StartCharging();
            }
        }
    }

    // 다른 Collider가 이 영역에서 나갔을 때 호출됨
    private void OnTriggerExit2D(Collider2D other)
    {
        // 나간 대상이 "Player" 태그를 가지고 있는지 확인
        if (other.CompareTag("Player"))
        {
            // 플레이어에게서 PlayerOxygen 스크립트를 가져옴
            PlayerOxygen playerOxygen = other.GetComponent<PlayerOxygen>();
            if (playerOxygen != null)
            {
                // 플레이어의 산소 충전 중지 함수를 호출
                playerOxygen.StopCharging();
            }
        }
    }
}
