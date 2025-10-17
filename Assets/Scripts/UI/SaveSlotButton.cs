using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 개별 세이브 슬롯 버튼 UI
/// 슬롯 정보 표시 및 클릭 처리
/// </summary>
public class SaveSlotButton : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private TextMeshProUGUI slotNumberText;    // "Slot 1"
    [SerializeField] private TextMeshProUGUI saveInfoText;      // 세이브 정보 (레벨, 씬, 시간 등)
    [SerializeField] private TextMeshProUGUI emptyText;         // "Empty Slot" 텍스트
    [SerializeField] private Button selectButton;               // 선택 버튼
    [SerializeField] private Button deleteButton;               // 삭제 버튼 (옵션)

    [Header("슬롯 정보")]
    private int slotNumber;
    private bool isEmpty;
    private LoadGameUI parentUI;

    void Awake()
    {
        // 버튼 이벤트 연결
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnSelectButtonClicked);
        }
        else
        {
            Debug.LogError($"SaveSlotButton: selectButton이 null입니다!", this);
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(OnDeleteButtonClicked);
            deleteButton.gameObject.SetActive(false); // 기본적으로 숨김
        }
    }

    void Start()
    {
        // LoadGameUI 찾기 (부모에서 찾거나 씬에서 찾기)
        parentUI = GetComponentInParent<LoadGameUI>();

        if (parentUI == null)
        {
            // 부모에 없으면 씬 전체에서 찾기
            parentUI = FindObjectOfType<LoadGameUI>();
        }

        if (parentUI == null)
        {
            Debug.LogError("LoadGameUI를 찾을 수 없습니다! LoadGame 씬에 LoadGameUI 컴포넌트가 있는지 확인하세요.");
        }
    }

    /// <summary>
    /// 슬롯 데이터 설정 및 UI 업데이트
    /// </summary>
    public void SetSlotData(SaveData data)
    {
        if (data == null) return;

        slotNumber = data.slotNumber;
        isEmpty = data.isEmpty;

        // 슬롯 번호 표시
        if (slotNumberText != null)
        {
            slotNumberText.text = $"Slot {slotNumber}";
        }

        // 세이브 정보 표시
        if (data.isEmpty)
        {
            // 빈 슬롯
            if (saveInfoText != null)
                saveInfoText.gameObject.SetActive(false);

            if (emptyText != null)
            {
                emptyText.gameObject.SetActive(true);
                emptyText.text = "Empty Slot\n(New Game)";
                emptyText.color = Color.gray;
            }

            if (deleteButton != null)
                deleteButton.gameObject.SetActive(false);
        }
        else
        {
            // 기존 세이브
            if (emptyText != null)
                emptyText.gameObject.SetActive(false);

            if (saveInfoText != null)
            {
                saveInfoText.gameObject.SetActive(true);
                saveInfoText.text = data.GetSummary();
                saveInfoText.color = Color.white;
            }

            if (deleteButton != null)
                deleteButton.gameObject.SetActive(true); // 삭제 버튼 표시
        }
    }

    /// <summary>
    /// 선택 버튼 클릭
    /// </summary>
    private void OnSelectButtonClicked()
    {
        if (parentUI != null)
        {
            parentUI.OnSlotSelected(slotNumber);
        }
        else
        {
            Debug.LogError("LoadGameUI를 찾을 수 없습니다!");
        }
    }

    /// <summary>
    /// 삭제 버튼 클릭
    /// </summary>
    private void OnDeleteButtonClicked()
    {
        if (parentUI != null)
        {
            parentUI.OnDeleteSlot(slotNumber);
        }
    }
}
