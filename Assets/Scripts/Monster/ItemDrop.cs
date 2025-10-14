using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemDropData
{
    [Tooltip("드롭할 아이템 프리팹")]
    public GameObject itemPrefab;

    [Tooltip("드롭 확률 (0~100%)")]
    [Range(0, 100)]
    public float dropChance = 10f;

    [Tooltip("최소 드롭 개수")]
    [Min(1)]
    public int minDropCount = 1;

    [Tooltip("최대 드롭 개수")]
    [Min(1)]
    public int maxDropCount = 1;
}

public class ItemDrop : MonoBehaviour
{
    [Header("골드 드롭 설정")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int baseGoldDrop = 50;
    [SerializeField] [Range(0, 1)] private float goldDropRange = 0.2f;
    [SerializeField] private int minCoinCount = 5; // 최소 코인 개수
    [SerializeField] private int maxCoinCount = 10; // 최대 코인 개수

    [Header("아이템 드롭 설정")]
    [SerializeField] private List<ItemDropData> itemDropList = new List<ItemDropData>();

    [Header("공통 설정")]
    [SerializeField] private float dropForce = 3f; // 아이템/코인이 튀어나가는 힘

    public void GenerateDrops()
    {
        DropGold();
        DropItems();
    }

    private void DropGold()
    {
        if (baseGoldDrop <= 0 || coinPrefab == null) return;

        int totalGold = Random.Range(Mathf.RoundToInt(baseGoldDrop * (1 - goldDropRange)), Mathf.RoundToInt(baseGoldDrop * (1 + goldDropRange)) + 1);
        int coinCount = Random.Range(minCoinCount, maxCoinCount + 1);

        if (totalGold <= 0) return;

        int goldPerCoin = totalGold / coinCount;
        int remainder = totalGold % coinCount;

        for (int i = 0; i < coinCount; i++)
        {
            int amount = goldPerCoin + (i < remainder ? 1 : 0);
            if (amount <= 0) continue;

            GameObject coinObject = Instantiate(coinPrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb = coinObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1.5f)).normalized;
                rb.AddForce(randomDirection * dropForce, ForceMode2D.Impulse);
            }

            Coin coin = coinObject.GetComponent<Coin>();
            if (coin != null)
            {
                coin.goldAmount = amount;
            }
        }
    }

    private void DropItems()
    {
        if (itemDropList == null || itemDropList.Count == 0) return;

        foreach (ItemDropData itemData in itemDropList)
        {
            // 아이템 프리팹이 없으면 스킵
            if (itemData.itemPrefab == null) continue;

            // 드롭 확률 체크 (0~100)
            float randomValue = Random.Range(0f, 100f);
            if (randomValue > itemData.dropChance) continue;

            // 드롭 개수 결정
            int dropCount = Random.Range(itemData.minDropCount, itemData.maxDropCount + 1);

            // 아이템 생성
            for (int i = 0; i < dropCount; i++)
            {
                GameObject droppedItem = Instantiate(itemData.itemPrefab, transform.position, Quaternion.identity);

                // Rigidbody2D가 있으면 힘을 가해서 튀어나가게 함
                Rigidbody2D rb = droppedItem.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1.5f)).normalized;
                    rb.AddForce(randomDirection * dropForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}
