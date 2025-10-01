using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Button 컴포넌트를 사용하기 위해 필수!

public class PortalController : MonoBehaviour
{
    private static List<string> usedPortalIDs = new List<string>();

    [Header("포탈 설정")]
    [Tooltip("이 포탈을 식별할 고유한 ID를 지정하세요. (예: HiddenPortal_Stage1)")]
    public string portalID = "HiddenPortal";

    [Header("플레이어 복귀 지점")]
    [Tooltip("플레이어가 돌아올 위치를 지정하는 빈 오브젝트")]
    [SerializeField] private Transform returnSpawnPoint;

    [Header("찾아올 UI 정보")]
    [Tooltip("Hierarchy 창에 있는 확인창 UI 패널의 정확한 이름을 입력하세요.")]
    [SerializeField] private string confirmationPanelName = "ConfirmationPanel";

    // --- 요청하신 변수들이 여기에 합쳐졌습니다 ---
    [Tooltip("확인창 패널 안에 있는 '확인' 버튼의 이름을 입력하세요.")]
    [SerializeField] private string confirmButtonName = "ConfirmButton";
    [Tooltip("확인창 패널 안에 있는 '취소' 버튼의 이름을 입력하세요.")]
    [SerializeField] private string cancelButtonName = "CancelButton";
    // --- 여기까지 ---

    [Header("이동할 씬 설정")]
    [SerializeField] private string sceneToLoad;

    private GameObject confirmationPanelObject;

    private void Start()
    {
        // --- ★★★ 이 부분을 추가하세요 ★★★ ---
        // 씬이 시작될 때, 이 포탈의 ID가 이미 사용된 목록에 있는지 확인합니다.
        if (usedPortalIDs.Contains(portalID))
        {
            // 만약 이미 사용되었다면, 즉시 스스로를 파괴하고 아무것도 하지 않습니다.
            Destroy(gameObject);
            return;
        }
        // --- 여기까지 ---

        SetupUIAndButtons();
    }

    // UI를 찾고 버튼 이벤트를 설정하는 함수
    private void SetupUIAndButtons()
    {
        // 씬에 있는 모든 캔버스를 뒤져서 확인창 패널을 찾습니다 (비활성화된 것도 포함).
        Canvas[] canvases = FindObjectsOfType<Canvas>(true);
        foreach (Canvas canvas in canvases)
        {
            Transform panelTransform = canvas.transform.Find(confirmationPanelName);
            if (panelTransform != null)
            {
                confirmationPanelObject = panelTransform.gameObject;

                // --- ★★★ 버튼을 찾고 이벤트를 코드로 연결하는 부분 ★★★ ---

                // 1. 확인 버튼 찾기 및 OnConfirm 함수 연결
                Transform confirmButtonT = panelTransform.Find(confirmButtonName);
                if (confirmButtonT != null)
                {
                    Button confirmButton = confirmButtonT.GetComponent<Button>();
                    if (confirmButton != null)
                    {
                        confirmButton.onClick.RemoveAllListeners(); // 기존 연결 제거 (중복 방지)
                        confirmButton.onClick.AddListener(OnConfirm); // OnConfirm 함수를 코드로 연결
                    }
                }

                // 2. 취소 버튼 찾기 및 OnCancel 함수 연결
                Transform cancelButtonT = panelTransform.Find(cancelButtonName);
                if (cancelButtonT != null)
                {
                    Button cancelButton = cancelButtonT.GetComponent<Button>();
                    if (cancelButton != null)
                    {
                        cancelButton.onClick.RemoveAllListeners(); // 기존 연결 제거
                        cancelButton.onClick.AddListener(OnCancel);   // OnCancel 함수를 코드로 연결
                    }
                }

                break; // 찾았으면 루프 종료
            }
        }

        if (confirmationPanelObject == null)
        {
            Debug.LogError($"'{confirmationPanelName}' 이름의 UI를 씬에서 찾을 수 없습니다! Hierarchy 창에서 UI 오브젝트의 이름을 확인해주세요.", this.gameObject);
        }
        else
        {
            confirmationPanelObject.SetActive(false); // 찾은 UI를 비활성화
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

            // --- PortalReturnManager가 사용할 수 있도록 PortalReturnData에 정보를 저장합니다 ---
            if (returnSpawnPoint != null)
            {
                PortalReturnData.hasReturnInfo = true;
                PortalReturnData.returnPosition = returnSpawnPoint.position;
                PortalReturnData.previousSceneName = SceneManager.GetActiveScene().name;
            }
            else
            {
                Debug.LogError("Return Spawn Point가 PortalController에 연결되지 않았습니다! Inspector를 확인해주세요.", this.gameObject);
                return;
            }

            // 2. GameManager에 복귀 정보를 저장합니다 (기존 기능 유지).
            if (GameManager.Instance != null)
            {
                // ★★★ 핵심 변경점 ★★★
                // 플레이어의 현재 위치 대신, 지정된 ReturnSpawnPoint의 위치를 저장합니다.
                if (returnSpawnPoint != null)
                {
                    GameManager.Instance.playerPositionBeforePortal = returnSpawnPoint.position;
                }
                else
                {
                    // 안전장치: 만약 returnSpawnPoint가 연결 안됐으면 그냥 현재 플레이어 위치 저장
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                    {
                        GameManager.Instance.playerPositionBeforePortal = player.transform.position;
                    }
                }

                GameManager.Instance.sceneNameBeforePortal = SceneManager.GetActiveScene().name;
            }

            // 3. 씬을 이동합니다 (기존 기능 유지).
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
}
