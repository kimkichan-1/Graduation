using UnityEngine;
using TMPro;

/// <summary>
/// 경험치 교환 받침대 - 플레이어가 E키로 현재 경험치를 골드로 교환할 수 있음
/// 레벨 다운은 되지 않으며, 현재 보유한 경험치만 교환 가능
/// </summary>
public class ExchangePedestal : MonoBehaviour
{
    [Header("교환 비율 설정")]
    [SerializeField] private int xpPerExchange = 10; // 한번에 교환할 경험치량
    [SerializeField] private int goldPerExchange = 10; // 한번 교환시 받을 골드량

    [Header("UI 참조")]
    [SerializeField] private SpriteRenderer exchangeIconRenderer;
    [SerializeField] private TextMeshProUGUI rateText; // 교환 비율 표시
    [SerializeField] private GameObject interactionPrompt; // "Press E" UI
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("시각 효과")]
    [SerializeField] private Sprite exchangeIcon; // 교환 아이콘
    [SerializeField] private float pulseSpeed = 2f; // 아이콘 맥박 속도
    [SerializeField] private float pulseAmount = 0.2f; // 맥박 크기

    [Header("오디오")]
    [SerializeField] private AudioClip exchangeSound;
    [SerializeField] private AudioClip errorSound;
    private AudioSource audioSource;

    private bool isPlayerNearby = false;
    private Vector3 originalIconScale;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        if (exchangeIconRenderer != null)
        {
            if (exchangeIcon != null)
            {
                exchangeIconRenderer.sprite = exchangeIcon;
            }
            originalIconScale = exchangeIconRenderer.transform.localScale;
        }

        UpdateRateDisplay();
    }

    void Update()
    {
        // 아이콘 맥박 애니메이션
        if (exchangeIconRenderer != null)
        {
            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            exchangeIconRenderer.transform.localScale = originalIconScale * scale;
        }

        // 플레이어가 근처에 있고 W키를 누르면 교환
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.W))
        {
            TryExchange();
        }
    }

    /// <summary>
    /// 경험치 → 골드 교환 시도 (한번에 10XP씩)
    /// </summary>
    private void TryExchange()
    {
        PlayerController player = PlayerController.Instance;
        if (player == null)
        {
            Debug.LogError("플레이어를 찾을 수 없습니다!");
            return;
        }

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats를 찾을 수 없습니다!");
            return;
        }

        // 현재 보유 경험치 확인 (레벨업에 사용되지 않은 경험치)
        int currentXp = stats.currentXp;

        if (currentXp < xpPerExchange)
        {
            Debug.Log($"교환 가능한 경험치가 부족합니다! (필요: {xpPerExchange} XP)");
            PlaySound(errorSound);
            return;
        }

        // 한번에 고정된 양만 교환
        int xpToSpend = xpPerExchange;
        int goldToReceive = goldPerExchange;

        // 경험치 차감
        stats.currentXp -= xpToSpend;

        // UI 업데이트
        if (stats.uiManager != null)
        {
            stats.uiManager.UpdateXpBar(stats.currentXp, stats.xpToNextLevel);
        }

        // 골드 추가
        stats.AddMoney(goldToReceive);

        PlaySound(exchangeSound);
        Debug.Log($"{xpToSpend} 경험치를 {goldToReceive} 골드로 교환했습니다!");
    }

    /// <summary>
    /// 교환 비율 표시 업데이트
    /// </summary>
    private void UpdateRateDisplay()
    {
        if (rateText != null)
        {
            rateText.text = $"Exchange\n{xpPerExchange}XP={goldPerExchange}G";
            rateText.color = Color.black;
        }
    }

    /// <summary>
    /// 상호작용 UI 업데이트
    /// </summary>
    private void UpdateInteractionPrompt()
    {
        if (interactionPrompt == null)
            return;

        if (isPlayerNearby)
        {
            interactionPrompt.SetActive(true);

            // 플레이어의 현재 경험치 확인
            if (PlayerController.Instance != null)
            {
                PlayerStats stats = PlayerController.Instance.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    int currentXp = stats.currentXp;
                    bool canExchange = currentXp >= xpPerExchange;

                    if (interactionText != null)
                    {
                        if (canExchange)
                        {
                            interactionText.text = "Press W to Exchange";
                            interactionText.color = Color.white;
                        }
                        else
                        {
                            interactionText.text = $"Need at least {xpPerExchange} XP";
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
