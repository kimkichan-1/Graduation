using UnityEngine;
using TMPro;

/// <summary>
/// 상점 아이템 받침대 - 플레이어가 W키로 구매할 수 있음
/// </summary>
public class ShopPedestal : MonoBehaviour
{
    [Header("아이템 정보")]
    [SerializeField] private ShopItemData currentItem;

    [Header("UI 참조")]
    [SerializeField] private SpriteRenderer itemIconRenderer;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject interactionPrompt; // "Press W" UI
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("시각 효과")]
    [SerializeField] private SpriteRenderer pedestalRenderer;
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color soldOutColor = Color.gray;

    private bool isPlayerNearby = false;
    private bool isSoldOut = false;

    void Start()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        UpdateVisuals();
    }

    void Update()
    {
        // 플레이어가 근처에 있고, 아직 팔리지 않았으며, W키를 누르면 구매
        if (isPlayerNearby && !isSoldOut && Input.GetKeyDown(KeyCode.W))
        {
            TryPurchase();
        }
    }

    /// <summary>
    /// 받침대에 아이템 설정
    /// </summary>
    public void SetItem(ShopItemData item)
    {
        currentItem = item;
        isSoldOut = false;
        UpdateVisuals();
    }

    /// <summary>
    /// 구매 시도
    /// </summary>
    private void TryPurchase()
    {
        if (currentItem == null || isSoldOut)
        {
            return;
        }

        if (ShopManager.Instance != null)
        {
            bool success = ShopManager.Instance.TryPurchaseItem(currentItem, this);
            // 구매 성공 여부는 ShopManager가 처리하고 OnItemPurchased()를 호출함
        }
    }

    /// <summary>
    /// 구매 완료 처리 (ShopManager에서 호출)
    /// </summary>
    public void OnItemPurchased()
    {
        isSoldOut = true;
        UpdateVisuals();

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    /// <summary>
    /// 시각적 요소 업데이트
    /// </summary>
    private void UpdateVisuals()
    {
        if (currentItem == null)
        {
            // 아이템이 없으면 모두 숨김
            if (itemIconRenderer != null)
                itemIconRenderer.sprite = null;
            if (priceText != null)
                priceText.text = "";
            return;
        }

        // 아이콘 설정
        if (itemIconRenderer != null && currentItem.icon != null)
        {
            itemIconRenderer.sprite = currentItem.icon;
            // 아이콘 크기 조절
            itemIconRenderer.transform.localScale = Vector3.one * currentItem.iconScale;
        }

        // 가격 텍스트 설정
        if (priceText != null)
        {
            if (isSoldOut)
            {
                priceText.text = "SOLD OUT";
                priceText.color = Color.red;
            }
            else
            {
                priceText.text = $"{currentItem.price}G";
                priceText.color = Color.yellow;
            }
        }

        // 받침대 색상 변경
        if (pedestalRenderer != null)
        {
            pedestalRenderer.color = isSoldOut ? soldOutColor : availableColor;
        }

        // 아이콘 투명도 조절
        if (itemIconRenderer != null)
        {
            Color iconColor = itemIconRenderer.color;
            iconColor.a = isSoldOut ? 0.3f : 1f;
            itemIconRenderer.color = iconColor;
        }
    }

    /// <summary>
    /// 상호작용 UI 업데이트
    /// </summary>
    private void UpdateInteractionPrompt()
    {
        if (interactionPrompt == null)
            return;

        if (isPlayerNearby && !isSoldOut && currentItem != null)
        {
            interactionPrompt.SetActive(true);

            // 플레이어의 현재 돈 확인
            if (PlayerController.Instance != null)
            {
                PlayerStats stats = PlayerController.Instance.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    bool canAfford = stats.currentMoney >= currentItem.price;

                    if (interactionText != null)
                    {
                        if (canAfford)
                        {
                            interactionText.text = "Press W";
                            interactionText.color = Color.white;
                        }
                        else
                        {
                            interactionText.text = "Not enough gold";
                            interactionText.color = Color.red;
                        }
                    }
                }
            }
        }
        else
        {
            interactionPrompt.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            UpdateInteractionPrompt();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UpdateInteractionPrompt();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }
}
