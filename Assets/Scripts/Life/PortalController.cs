using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Button ������Ʈ�� ����ϱ� ���� �ʼ�!

public class PortalController : MonoBehaviour
{
    public static List<string> usedPortalIDs = new List<string>();

    [Header("��Ż ����")]
    [Tooltip("�� ��Ż�� �ĺ��� ������ ID�� �����ϼ���. (��: HiddenPortal_Stage1)")]
    public string portalID = "HiddenPortal";

    [Header("�÷��̾� ���� ����")]
    [Tooltip("�÷��̾ ���ƿ� ��ġ�� �����ϴ� �� ������Ʈ")]
    [SerializeField] private Transform returnSpawnPoint;

    [Header("ã�ƿ� UI ����")]
    [Tooltip("Hierarchy â�� �ִ� Ȯ��â UI �г��� ��Ȯ�� �̸��� �Է��ϼ���.")]
    [SerializeField] private string confirmationPanelName = "ConfirmationPanel";

    // --- ��û�Ͻ� �������� ���⿡ ���������ϴ� ---
    [Tooltip("Ȯ��â �г� �ȿ� �ִ� 'Ȯ��' ��ư�� �̸��� �Է��ϼ���.")]
    [SerializeField] private string confirmButtonName = "ConfirmButton";
    [Tooltip("Ȯ��â �г� �ȿ� �ִ� '���' ��ư�� �̸��� �Է��ϼ���.")]
    [SerializeField] private string cancelButtonName = "CancelButton";
    // --- ������� ---

    [Header("�̵��� �� ����")]
    [SerializeField] private string sceneToLoad;

    private GameObject confirmationPanelObject;

    private void Start()
    {
        // --- �ڡڡ� �� �κ��� �߰��ϼ��� �ڡڡ� ---
        // ���� ���۵� ��, �� ��Ż�� ID�� �̹� ���� ��Ͽ� �ִ��� Ȯ���մϴ�.
        if (usedPortalIDs.Contains(portalID))
        {
            // ���� �̹� ���Ǿ��ٸ�, ��� �����θ� �ı��ϰ� �ƹ��͵� ���� �ʽ��ϴ�.
            Destroy(gameObject);
            return;
        }
        // --- ������� ---

        SetupUIAndButtons();
    }

    // UI�� ã�� ��ư �̺�Ʈ�� �����ϴ� �Լ�
    private void SetupUIAndButtons()
    {
        // ���� �ִ� ��� ĵ������ ������ Ȯ��â �г��� ã���ϴ� (��Ȱ��ȭ�� �͵� ����).
        Canvas[] canvases = FindObjectsOfType<Canvas>(true);
        foreach (Canvas canvas in canvases)
        {
            Transform panelTransform = canvas.transform.Find(confirmationPanelName);
            if (panelTransform != null)
            {
                confirmationPanelObject = panelTransform.gameObject;

                // --- �ڡڡ� ��ư�� ã�� �̺�Ʈ�� �ڵ�� �����ϴ� �κ� �ڡڡ� ---

                // 1. Ȯ�� ��ư ã�� �� OnConfirm �Լ� ����
                Transform confirmButtonT = panelTransform.Find(confirmButtonName);
                if (confirmButtonT != null)
                {
                    Button confirmButton = confirmButtonT.GetComponent<Button>();
                    if (confirmButton != null)
                    {
                        confirmButton.onClick.RemoveAllListeners(); // ���� ���� ���� (�ߺ� ����)
                        confirmButton.onClick.AddListener(OnConfirm); // OnConfirm �Լ��� �ڵ�� ����
                    }
                }

                // 2. ��� ��ư ã�� �� OnCancel �Լ� ����
                Transform cancelButtonT = panelTransform.Find(cancelButtonName);
                if (cancelButtonT != null)
                {
                    Button cancelButton = cancelButtonT.GetComponent<Button>();
                    if (cancelButton != null)
                    {
                        cancelButton.onClick.RemoveAllListeners(); // ���� ���� ����
                        cancelButton.onClick.AddListener(OnCancel);   // OnCancel �Լ��� �ڵ�� ����
                    }
                }

                break; // ã������ ���� ����
            }
        }

        if (confirmationPanelObject == null)
        {
            Debug.LogError($"'{confirmationPanelName}' �̸��� UI�� ������ ã�� �� �����ϴ�! Hierarchy â���� UI ������Ʈ�� �̸��� Ȯ�����ּ���.", this.gameObject);
        }
        else
        {
            confirmationPanelObject.SetActive(false); // ã�� UI�� ��Ȱ��ȭ
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (confirmationPanelObject != null)
            {
                confirmationPanelObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (confirmationPanelObject != null)
            {
                confirmationPanelObject.SetActive(false);
            }
        }
    }

    public void OnConfirm()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            if (!usedPortalIDs.Contains(portalID))
            {
                usedPortalIDs.Add(portalID);
            }

            // --- 수정된 부분 ---
            // 1. PlayerReturnData 대신 LifeGameManager에 정보를 저장합니다.
            if (returnSpawnPoint != null && LifeGameManager.Instance != null)
            {
                LifeGameManager.Instance.playerPositionBeforePortal = returnSpawnPoint.position;
                LifeGameManager.Instance.sceneNameBeforePortal = SceneManager.GetActiveScene().name;
            }
            else
            {
                Debug.LogError("Return Spawn Point가 연결되지 않았거나 LifeGameManager가 씬에 없습니다!", this.gameObject);
                return;
            }

            

            SceneManager.LoadScene(sceneToLoad);
        }
        Destroy(gameObject);
    }

    public void OnCancel()
    {
        if (!usedPortalIDs.Contains(portalID))
        {
            usedPortalIDs.Add(portalID);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// static 변수 초기화 (게임 오버 또는 메인 메뉴 복귀 시 호출)
    /// </summary>
    public static void ResetStaticVariables()
    {
        usedPortalIDs.Clear();
        Debug.Log("PortalController: usedPortalIDs 초기화됨");
    }
}
