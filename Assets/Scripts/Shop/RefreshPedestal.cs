using UnityEngine;
using TMPro;

/// <summary>
/// 새로고침 받침대 - 플레이어가 W키로 상점 아이템을 새로고침할 수 있음
/// </summary>
public class RefreshPedestal : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private SpriteRenderer refreshIconRenderer;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject interactionPrompt; // "Press W" UI
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("시각 효과")]
    [SerializeField] private SpriteRenderer pedestalRenderer;
    [SerializeField] private Sprite refreshIcon; // 새로고침 아이콘 (화살표 순환 모양)
    [SerializeField] private float rotationSpeed = 50f; // 아이콘 회전 속도

    private bool isPlayerNearby = false;

    void OnEnable()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        if (refreshIconRenderer != null && refreshIcon != null)
        {
            refreshIconRenderer.sprite = refreshIcon;
        }

        UpdateCostDisplay();
    }

    void Update()
    {
        // 아이콘 회전 애니메이션
        if (refreshIconRenderer != null)
        {
            refreshIconRenderer.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }

        // 플레이어가 근처에 있고 W키를 누르면 새로고침
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.W))
        {
            TryRefresh();
        }
    }

    /// <summary>
    /// 새로고침 시도
    /// </summary>
    private void TryRefresh()
    {
        if (ShopManager.Instance != null)
        {
            bool success = ShopManager.Instance.TryRefreshShop();

            if (success)
            {
                // 새로고침 성공 시 비용 표시 업데이트
                UpdateCostDisplay();
            }
        }
    }

    /// <summary>
    /// 비용 표시 업데이트
    /// </summary>
    private void UpdateCostDisplay()
    {
        if (costText != null && ShopManager.Instance != null)
        {
            int currentCost = ShopManager.Instance.GetCurrentRefreshCost();
            costText.color = Color.gray;
            costText.text = $"Refresh\n{currentCost}G";
            
        }
    }

    /// <summary>
    /// 상호작용 UI 업데이트
    /// </summary>
    private void UpdateInteractionPrompt()
    {
        if (interactionPrompt == null || ShopManager.Instance == null)
            return;

        if (isPlayerNearby)
        {
            interactionPrompt.SetActive(true);

            // 플레이어의 현재 돈 확인
            if (PlayerController.Instance != null)
            {
                PlayerStats stats = PlayerController.Instance.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    int refreshCost = ShopManager.Instance.GetCurrentRefreshCost();
                    bool canAfford = stats.currentMoney >= refreshCost;

                    if (interactionText != null)
                    {
                        if (canAfford)
                        {
                            interactionText.text = "Press W to Refresh";
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
