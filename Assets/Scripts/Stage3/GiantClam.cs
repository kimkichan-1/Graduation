using UnityEngine;

public class GiantClam : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private int requiredPearls = 3;
    [SerializeField] private GameObject portalToActivate;

    [Header("오브젝트 설정")]
    [SerializeField] private GameObject closedClamObject;
    [SerializeField] private GameObject openClamObject;

    private bool isOpen = false; // 조개가 열렸는지 확인하는 스위치

    void Start()
    {
        // 시작할 때의 초기 상태 설정
        if (closedClamObject != null) closedClamObject.SetActive(true);
        if (openClamObject != null) openClamObject.SetActive(false);
        if (portalToActivate != null) portalToActivate.SetActive(false);
    }

    void Update()
    {
        // 아직 조개가 열리지 않았을 때만 확인
        if (!isOpen)
        {
            // PearlDisplayUI에 현재 진주 개수를 물어봄
            if (PearlDisplayUI.instance.GetCurrentPearls() >= requiredPearls)
            {
                // 진주 개수가 충분하면 즉시 Open() 함수를 호출
                Open();
            }
        }
    }

    private void Open()
    {
        isOpen = true; // 스위치를 켜서 Update 함수가 더 이상 실행되지 않도록 함
        Debug.Log("대왕조개가 자동으로 열립니다!");

        // 닫힌 조개 오브젝트를 끄고, 열린 조개 오브젝트를 켭니다.
        if (closedClamObject != null) closedClamObject.SetActive(false);
        if (openClamObject != null) openClamObject.SetActive(true);

        // 포탈을 활성화합니다.
        if (portalToActivate != null)
        {
            portalToActivate.SetActive(true);
            Debug.Log("포탈이 활성화되었습니다!");
        }
    }
}
