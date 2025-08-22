using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [Header("골드 드롭 설정")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int baseGoldDrop = 50;
    [SerializeField] [Range(0, 1)] private float goldDropRange = 0.2f;
    [SerializeField] private int minCoinCount = 5; // 최소 코인 개수
    [SerializeField] private int maxCoinCount = 10; // 최대 코인 개수
    [SerializeField] private float dropForce = 3f; // 코인이 튀어나가는 힘

    public void GenerateDrops()
    {
        DropGold();
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
}
