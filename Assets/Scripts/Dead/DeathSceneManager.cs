using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathSceneManager : MonoBehaviour
{
    // 버튼 클릭 시 호출될 함수
    public void GoToMainMenu()
    {
        // 1. (★수정됨★) 멈췄던 시간을 1f (정상 속도)로 되돌립니다.
        Time.timeScale = 1f;

        PlayerStats.ResetStaticVariables();
        BossGameManager.ResetStaticVariables();

        if (GameManager.Instance != null && SaveManager.Instance != null)
        {
            // 'GameManager'로부터 현재 플레이 중인 슬롯 번호를 가져옵니다.
            int slotToDelete = GameManager.Instance.currentSaveSlot;

            // 슬롯 번호가 유효한 경우 (1, 2, 3 등)
            if (slotToDelete > 0)
            {
                Debug.Log($"플레이어 사망: 세이브 슬롯 {slotToDelete}의 데이터를 삭제합니다.");

                // 'SaveManager'에게 해당 슬롯의 파일 삭제를 명령합니다.
                SaveManager.Instance.DeleteSave(slotToDelete);
            }
            else
            {
                // currentSaveSlot이 -1이거나 0인 경우 (새 게임 직후 저장 안 함 등)
                Debug.Log("현재 세이브 슬롯이 설정되지 않아, 삭제할 파일이 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("GameManager 또는 SaveManager가 없어서 세이브 파일을 삭제할 수 없습니다.");
        }
        // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---

        // 3. (★수정됨★) DontDestroyOnLoadManager의 플래그를 설정합니다.
        //    이제 GameManager 등을 직접 Destroy() 할 필요가 없습니다.
        DontDestroyOnLoadManager.isReturningToMainMenu = true;

        // 4. "Main" 씬을 로드합니다.
        SceneManager.LoadScene("Main");
    }
}
