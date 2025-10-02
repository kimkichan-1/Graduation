using UnityEngine;

public class BossAreaTrigger : MonoBehaviour
{
    private bool bossFightStarted = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 태그를 가진 오브젝트가 트리거에 진입했을 때
        if (other.CompareTag("Player") && !bossFightStarted)
        {
            // 보스전이 이미 시작되지 않았는지 확인
            bossFightStarted = true;

            Debug.Log("플레이어가 보스 구역에 진입했습니다. 턴 기반 보스전 시작!");

            // 턴 매니저의 인스턴스를 찾아 보스전 시작 메서드 호출
            TurnManager turnManager = TurnManager.instance;
            if (turnManager != null)
            {
                turnManager.StartBossFight();
            }
            else
            {
                Debug.LogError("씬에 TurnManager가 없습니다!");
            }
        }
    }
}