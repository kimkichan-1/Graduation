using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathSceneManager : MonoBehaviour
{
    // ��ư Ŭ�� �� ȣ��� �Լ�
    public void GoToMainMenu()
    {
        // 1. (�ڼ����ʡ�) ����� �ð��� 1f (���� �ӵ�)�� �ǵ����ϴ�.
        Time.timeScale = 1f;

        PlayerStats.ResetStaticVariables();
        BossGameManager.ResetStaticVariables();
        MidBossController.ResetStaticVariables();
        PortalController.ResetStaticVariables();
        StatueInteraction.ResetStaticVariables();
        BlacksmithMinigameManager.ResetStaticVariables();

        if (GameManager.Instance != null && SaveManager.Instance != null)
        {
            // 'GameManager'�κ��� ���� �÷��� ���� ���� ��ȣ�� �����ɴϴ�.
            int slotToDelete = GameManager.Instance.currentSaveSlot;

            // ���� ��ȣ�� ��ȿ�� ��� (1, 2, 3 ��)
            if (slotToDelete > 0)
            {
                Debug.Log($"�÷��̾� ���: ���̺� ���� {slotToDelete}�� �����͸� �����մϴ�.");

                // 'SaveManager'���� �ش� ������ ���� ������ �����մϴ�.
                SaveManager.Instance.DeleteSave(slotToDelete);
            }
            else
            {
                // currentSaveSlot�� -1�̰ų� 0�� ��� (�� ���� ���� ���� �� �� ��)
                Debug.Log("���� ���̺� ������ �������� �ʾ�, ������ ������ �����ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("GameManager �Ǵ� SaveManager�� ��� ���̺� ������ ������ �� �����ϴ�.");
        }
        // --- ������������������������������� ---

        // 3. (�ڼ����ʡ�) DontDestroyOnLoadManager�� �÷��׸� �����մϴ�.
        //    ���� GameManager ���� ���� Destroy() �� �ʿ䰡 �����ϴ�.
        DontDestroyOnLoadManager.isReturningToMainMenu = true;

        // 4. "Main" ���� �ε��մϴ�.
        SceneManager.LoadScene("Main");
    }
}
