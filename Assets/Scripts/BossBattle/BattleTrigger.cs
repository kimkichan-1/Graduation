// BattleTrigger.cs

using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 이 트리거 영역에 들어왔는지 확인
        if (other.CompareTag("Player"))
        {
            Debug.Log("전투 지역 진입! 전투를 시작합니다.");
            // 게임 매니저에게 전투 상태로 전환하라고 명령
            GameManager.Instance.ChangeState(GameState.Battle);
            
            // 한 번만 실행되도록 트리거 오브젝트를 비활성화
            gameObject.SetActive(false);
        }
    }
}