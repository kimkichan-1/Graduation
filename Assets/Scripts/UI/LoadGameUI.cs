using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// LoadGame 씬의 UI 관리
/// 3개의 세이브 슬롯 표시 및 선택 처리
/// </summary>
public class LoadGameUI : MonoBehaviour
{
    [Header("세이브 슬롯 버튼")]
    [SerializeField] private SaveSlotButton[] slotButtons = new SaveSlotButton[3];

    [Header("기타 버튼")]
    [SerializeField] private GameObject backButton;

    [Header("덮어쓰기 확인 UI")]
    [SerializeField] private GameObject overwriteConfirmPanel; // 덮어쓰기 확인 패널
    [SerializeField] private UnityEngine.UI.Button confirmOverwriteButton; // 확인 버튼
    [SerializeField] private UnityEngine.UI.Button cancelOverwriteButton;  // 취소 버튼

    private int pendingSlotNumber = -1; // 덮어쓰기 대기 중인 슬롯 번호

    void Start()
    {
        // 세이브 슬롯 정보 로드 및 표시
        DisplaySaveSlots();

        // 덮어쓰기 확인 패널 이벤트 연결
        if (confirmOverwriteButton != null)
        {
            confirmOverwriteButton.onClick.AddListener(OnConfirmOverwrite);
        }

        if (cancelOverwriteButton != null)
        {
            cancelOverwriteButton.onClick.AddListener(OnCancelOverwrite);
        }

        // 덮어쓰기 확인 패널 숨김
        if (overwriteConfirmPanel != null)
        {
            overwriteConfirmPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 모든 세이브 슬롯 정보 표시
    /// </summary>
    private void DisplaySaveSlots()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager가 없습니다!");
            return;
        }

        SaveData[] slots = SaveManager.Instance.GetAllSaveSlots();

        for (int i = 0; i < slots.Length && i < slotButtons.Length; i++)
        {
            if (slotButtons[i] != null && slots[i] != null)
            {
                slotButtons[i].SetSlotData(slots[i]);
            }
        }
    }

    /// <summary>
    /// 슬롯 선택 처리
    /// </summary>
    public void OnSlotSelected(int slotNumber)
    {
        if (SaveManager.Instance == null || GameManager.Instance == null)
        {
            Debug.LogError("SaveManager 또는 GameManager가 없습니다!");
            return;
        }

        SaveData data = SaveManager.Instance.LoadGame(slotNumber);

        if (data == null || data.isEmpty)
        {
            // 빈 슬롯
            if (GameManager.Instance.isNewGame)
            {
                // NewGame 플로우 - 신규 게임 시작 가능
                StartNewGameInSlot(slotNumber);
            }
            else
            {
                // LoadGame 플로우 - 빈 슬롯은 클릭 불가
                Debug.Log("빈 슬롯입니다. NewGame을 통해 게임을 시작하세요.");
                return;
            }
        }
        else
        {
            // 슬롯에 데이터가 있음
            if (GameManager.Instance.isNewGame)
            {
                // NewGame 플로우에서 기존 슬롯 선택 - 덮어쓰기 확인
                ShowOverwriteConfirmation(slotNumber);
            }
            else
            {
                // LoadGame 플로우 - 기존 세이브 로드
                LoadExistingGame(slotNumber, data);
            }
        }
    }

    /// <summary>
    /// 신규 게임 시작 (빈 슬롯에)
    /// </summary>
    private void StartNewGameInSlot(int slotNumber)
    {
        GameManager.Instance.SelectSaveSlot(slotNumber);

        // Weapon 씬에서 선택한 무기가 있다면 초기 세이브 생성
        if (GameManager.Instance.isNewGame && !string.IsNullOrEmpty(GameManager.Instance.selectedWeapon) && GameManager.Instance.selectedWeapon != "None")
        {
            // 초기 세이브 데이터 생성
            SaveData newSave = new SaveData(slotNumber);
            newSave.equippedWeapon = GameManager.Instance.selectedWeapon;
            newSave.currentScene = "Stage1";

            // 무기에 따른 초기 설정 (WeaponSpawner가 스폰하므로 hasSword는 false로 유지)
            // 무기는 플레이어가 주워서 장착함

            // 초기 저장
            SaveManager.Instance.SaveGame(slotNumber, newSave);
            Debug.Log($"슬롯 {slotNumber} 신규 게임 시작: {newSave.equippedWeapon}");
        }

        // Stage1으로 이동
        SceneManager.LoadScene("Stage1");
    }

    /// <summary>
    /// 기존 게임 로드
    /// </summary>
    private void LoadExistingGame(int slotNumber, SaveData data)
    {
        Debug.Log($"슬롯 {slotNumber} 로드: Lv.{data.playerLevel}, {data.currentScene}");

        GameManager.Instance.ApplySaveData(data);
        // ApplySaveData 내부에서 씬 로드 처리
    }

    /// <summary>
    /// 슬롯 삭제 (옵션)
    /// </summary>
    public void OnDeleteSlot(int slotNumber)
    {
        if (SaveManager.Instance == null)
            return;

        // 확인 다이얼로그 표시 (간단 버전)
        bool confirm = true; // 실제로는 확인 UI 필요

        if (confirm)
        {
            SaveManager.Instance.DeleteSave(slotNumber);
            DisplaySaveSlots(); // 갱신
            Debug.Log($"슬롯 {slotNumber} 삭제됨");
        }
    }

    /// <summary>
    /// 덮어쓰기 확인 대화상자 표시
    /// </summary>
    private void ShowOverwriteConfirmation(int slotNumber)
    {
        pendingSlotNumber = slotNumber;

        if (overwriteConfirmPanel != null)
        {
            overwriteConfirmPanel.SetActive(true);
            Debug.Log($"슬롯 {slotNumber} 덮어쓰기 확인 대화상자 표시");
        }
        else
        {
            // 확인 패널이 없으면 경고 후 바로 덮어쓰기
            Debug.LogWarning("덮어쓰기 확인 패널이 설정되지 않았습니다. 바로 덮어쓰기합니다.");
            StartNewGameInSlot(slotNumber);
        }
    }

    /// <summary>
    /// 덮어쓰기 확인 버튼 클릭
    /// </summary>
    private void OnConfirmOverwrite()
    {
        if (overwriteConfirmPanel != null)
        {
            overwriteConfirmPanel.SetActive(false);
        }

        if (pendingSlotNumber > 0)
        {
            Debug.Log($"슬롯 {pendingSlotNumber} 덮어쓰기 확인됨");
            StartNewGameInSlot(pendingSlotNumber);
            pendingSlotNumber = -1;
        }
    }

    /// <summary>
    /// 덮어쓰기 취소 버튼 클릭
    /// </summary>
    private void OnCancelOverwrite()
    {
        if (overwriteConfirmPanel != null)
        {
            overwriteConfirmPanel.SetActive(false);
        }

        Debug.Log($"슬롯 {pendingSlotNumber} 덮어쓰기 취소됨");
        pendingSlotNumber = -1;
    }

    /// <summary>
    /// 뒤로 가기 (Main 씬으로)
    /// </summary>
    public void OnBackButton()
    {
        SceneManager.LoadScene("Main");
    }
}
