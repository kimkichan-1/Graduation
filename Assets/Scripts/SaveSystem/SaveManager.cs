using System.IO;
using UnityEngine;

/// <summary>
/// 세이브 파일 저장/로드 관리자
/// JSON 파일을 로컬 디스크에 저장
/// </summary>
public class SaveManager : MonoBehaviour
{
    #region Singleton
    public static SaveManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("SaveManager 인스턴스가 이미 존재합니다!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 세이브 폴더 생성
        if (!Directory.Exists(SaveFolderPath))
        {
            Directory.CreateDirectory(SaveFolderPath);
            Debug.Log($"세이브 폴더 생성: {SaveFolderPath}");
        }
    }
    #endregion

    // 세이브 파일 경로 설정
    private string SaveFolderPath => Path.Combine(Application.persistentDataPath, "Saves");

    /// <summary>
    /// 특정 슬롯의 파일 경로 반환
    /// </summary>
    private string GetSaveFilePath(int slotNumber)
    {
        return Path.Combine(SaveFolderPath, $"SaveSlot{slotNumber}.json");
    }

    /// <summary>
    /// 게임 저장
    /// </summary>
    public bool SaveGame(int slotNumber, SaveData data)
    {
        if (slotNumber < 1 || slotNumber > 3)
        {
            Debug.LogError($"잘못된 슬롯 번호: {slotNumber}. 1-3 사이여야 합니다.");
            return false;
        }

        try
        {
            data.slotNumber = slotNumber;
            data.isEmpty = false;
            data.UpdateSaveTime();

            string json = JsonUtility.ToJson(data, true);  // true = pretty print
            string filePath = GetSaveFilePath(slotNumber);

            File.WriteAllText(filePath, json);
            Debug.Log($"게임 저장 완료: {filePath}");
            Debug.Log($"저장 내용: Lv.{data.playerLevel}, {data.currentScene}, {data.currentMoney}G");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"게임 저장 실패: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 게임 로드
    /// </summary>
    public SaveData LoadGame(int slotNumber)
    {
        if (slotNumber < 1 || slotNumber > 3)
        {
            Debug.LogError($"잘못된 슬롯 번호: {slotNumber}");
            return null;
        }

        string filePath = GetSaveFilePath(slotNumber);

        if (!File.Exists(filePath))
        {
            Debug.Log($"슬롯 {slotNumber}에 세이브 파일이 없습니다.");
            return SaveData.CreateEmptySlot(slotNumber);
        }

        try
        {
            string json = File.ReadAllText(filePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            Debug.Log($"게임 로드 완료: 슬롯 {slotNumber}");
            Debug.Log($"로드 내용: Lv.{data.playerLevel}, {data.currentScene}, {data.currentMoney}G");
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"게임 로드 실패: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// 세이브 삭제
    /// </summary>
    public bool DeleteSave(int slotNumber)
    {
        if (slotNumber < 1 || slotNumber > 3)
        {
            Debug.LogError($"잘못된 슬롯 번호: {slotNumber}");
            return false;
        }

        string filePath = GetSaveFilePath(slotNumber);

        if (!File.Exists(filePath))
        {
            Debug.Log($"슬롯 {slotNumber}에 삭제할 파일이 없습니다.");
            return false;
        }

        try
        {
            File.Delete(filePath);
            Debug.Log($"세이브 삭제 완료: 슬롯 {slotNumber}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"세이브 삭제 실패: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 슬롯 정보 확인 (파일 존재 여부)
    /// </summary>
    public SaveData GetSaveSlotInfo(int slotNumber)
    {
        return LoadGame(slotNumber);
    }

    /// <summary>
    /// 모든 슬롯 정보 가져오기
    /// </summary>
    public SaveData[] GetAllSaveSlots()
    {
        SaveData[] slots = new SaveData[3];
        for (int i = 0; i < 3; i++)
        {
            slots[i] = GetSaveSlotInfo(i + 1);
        }
        return slots;
    }

    /// <summary>
    /// 슬롯이 비어있는지 확인
    /// </summary>
    public bool IsSlotEmpty(int slotNumber)
    {
        SaveData data = LoadGame(slotNumber);
        return data == null || data.isEmpty;
    }

    /// <summary>
    /// 디버그: 세이브 폴더 경로 출력
    /// </summary>
    [ContextMenu("Show Save Folder Path")]
    public void ShowSaveFolderPath()
    {
        Debug.Log($"세이브 폴더 경로: {SaveFolderPath}");

        // 폴더 열기 (Windows만)
        #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        System.Diagnostics.Process.Start("explorer.exe", SaveFolderPath.Replace("/", "\\"));
        #endif
    }

    /// <summary>
    /// 디버그: 모든 세이브 슬롯 정보 출력
    /// </summary>
    [ContextMenu("Show All Save Slots")]
    public void ShowAllSaveSlots()
    {
        SaveData[] slots = GetAllSaveSlots();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && !slots[i].isEmpty)
            {
                Debug.Log($"슬롯 {i + 1}: {slots[i].GetSummary()}");
            }
            else
            {
                Debug.Log($"슬롯 {i + 1}: Empty");
            }
        }
    }
}
