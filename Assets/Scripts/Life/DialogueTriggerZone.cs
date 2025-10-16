// 파일명: DialogueTrigger.cs
using UnityEngine;
public class DialogueTrigger : MonoBehaviour
{
    // Inspector 창에서 MinigameManager를 연결
    public MinigameManager minigameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // MinigameManager의 이벤트 시작 함수를 호출
            minigameManager.StartEventSequence();
            Destroy(gameObject); // 한 번만 실행되도록 자신을 파괴
        }
    }
}
