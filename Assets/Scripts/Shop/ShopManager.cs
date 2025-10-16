using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상점 시스템 관리자 - 랜덤 아이템 선택, 구매 처리, 새로고침 관리
/// </summary>
public class ShopManager : MonoBehaviour
{
    #region Singleton
    public static ShopManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("ShopManager 인스턴스가 이미 존재합니다!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    [Header("아이템 풀")]
    [SerializeField] private List<ShopItemData> allAvailableItems = new List<ShopItemData>();

    [Header("받침대 참조")]
    [SerializeField] private ShopPedestal[] itemPedestals = new ShopPedestal[4];

    [Header("새로고침 설정")]
    [SerializeField] private int baseRefreshCost = 50;
    [SerializeField] private int refreshCostIncrement = 50;
    private int currentRefreshCost;
    private int refreshCount = 0;

    [Header("오디오")]
    [SerializeField] private AudioClip purchaseSound;
    [SerializeField] private AudioClip refreshSound;
    [SerializeField] private AudioClip errorSound;
    private AudioSource audioSource;

    // 현재 상점에 표시된 아이템들 (중복 방지용)
    private List<ShopItemData> currentShopItems = new List<ShopItemData>();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        currentRefreshCost = baseRefreshCost;
        InitializeShop();
    }

    /// <summary>
    /// 상점 초기화 - 랜덤 4개 아이템 선택
    /// </summary>
    private void InitializeShop()
    {
        if (allAvailableItems.Count < 4)
        {
            Debug.LogError("상점 아이템 풀에 최소 4개 이상의 아이템이 필요합니다!");
            return;
        }

        SelectRandomItems();
        AssignItemsToPedestals();
    }

    /// <summary>
    /// 아이템 풀에서 랜덤으로 4개 선택 (중복 없이)
    /// </summary>
    private void SelectRandomItems()
    {
        currentShopItems.Clear();
        List<ShopItemData> tempPool = new List<ShopItemData>(allAvailableItems);

        for (int i = 0; i < 4 && tempPool.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, tempPool.Count);
            currentShopItems.Add(tempPool[randomIndex]);
            tempPool.RemoveAt(randomIndex);
        }
    }

    /// <summary>
    /// 선택된 아이템들을 받침대에 할당
    /// </summary>
    private void AssignItemsToPedestals()
    {
        for (int i = 0; i < itemPedestals.Length && i < currentShopItems.Count; i++)
        {
            if (itemPedestals[i] != null)
            {
                itemPedestals[i].SetItem(currentShopItems[i]);
            }
        }
    }

    /// <summary>
    /// 아이템 구매 시도
    /// </summary>
    public bool TryPurchaseItem(ShopItemData item, ShopPedestal pedestal)
    {
        if (item == null)
        {
            Debug.LogWarning("구매할 아이템이 없습니다.");
            return false;
        }

        PlayerController player = PlayerController.Instance;
        if (player == null)
        {
            Debug.LogError("플레이어를 찾을 수 없습니다!");
            return false;
        }

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats를 찾을 수 없습니다!");
            return false;
        }

        // 돈이 충분한지 확인
        if (stats.currentMoney < item.price)
        {
            Debug.Log("골드가 부족합니다!");
            PlaySound(errorSound);
            return false;
        }

        // 돈 차감
        stats.SpendMoney(item.price);

        // 아이템 효과 적용
        item.ApplyEffect(player);

        // 구매 성공 처리
        pedestal.OnItemPurchased();
        PlaySound(purchaseSound);

        Debug.Log($"{item.itemName}을(를) {item.price} 골드에 구매했습니다!");
        return true;
    }

    /// <summary>
    /// 상점 새로고침 시도
    /// </summary>
    public bool TryRefreshShop()
    {
        PlayerController player = PlayerController.Instance;
        if (player == null)
        {
            Debug.LogError("플레이어를 찾을 수 없습니다!");
            return false;
        }

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats를 찾을 수 없습니다!");
            return false;
        }

        // 돈이 충분한지 확인
        if (stats.currentMoney < currentRefreshCost)
        {
            Debug.Log("새로고침에 필요한 골드가 부족합니다!");
            PlaySound(errorSound);
            return false;
        }

        // 돈 차감
        stats.SpendMoney(currentRefreshCost);

        // 새로고침 비용 증가
        refreshCount++;
        currentRefreshCost = baseRefreshCost + (refreshCostIncrement * refreshCount);

        // 아이템 재선택
        SelectRandomItems();
        AssignItemsToPedestals();

        PlaySound(refreshSound);
        Debug.Log($"상점을 새로고침했습니다! (다음 새로고침 비용: {currentRefreshCost} 골드)");
        return true;
    }

    /// <summary>
    /// 현재 새로고침 비용 반환
    /// </summary>
    public int GetCurrentRefreshCost()
    {
        return currentRefreshCost;
    }

    /// <summary>
    /// 사운드 재생
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
