// 파일명: ItemCollector.cs (수정된 최종 버전)
using UnityEngine;
using System.Collections.Generic;

public class ItemCollector : MonoBehaviour
{
    private List<GameObject> collectibleItemsInRange = new List<GameObject>();

    void Update()
    {
        // 리스트에 이미 파괴된 아이템이 있다면 먼저 정리
        collectibleItemsInRange.RemoveAll(item => item == null);

        // Z 키를 눌렀고, 수집 가능한 아이템이 범위 안에 있다면
        if (Input.GetKeyDown(KeyCode.M) && collectibleItemsInRange.Count > 0)
        {
            GameObject itemToCollect = collectibleItemsInRange[0];

            // 중요: 파괴하기 전에 리스트에서 먼저 제거하여 경쟁 상태를 방지
            collectibleItemsInRange.RemoveAt(0);

            // 관리자에게 아이템 수집 및 파괴를 요청
            if (BlacksmithMinigameManager.Instance != null)
            {
                BlacksmithMinigameManager.Instance.CollectFlame(itemToCollect);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
        {
            if (!collectibleItemsInRange.Contains(other.gameObject))
            {
                collectibleItemsInRange.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
        {
            // 리스트에서 제거하기 전에, 해당 오브젝트가 여전히 리스트에 있는지 확인
            if (collectibleItemsInRange.Contains(other.gameObject))
            {
                collectibleItemsInRange.Remove(other.gameObject);
            }
        }
    }
}