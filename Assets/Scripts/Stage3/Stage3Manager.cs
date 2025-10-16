using UnityEngine;

public class Stage3Manager : MonoBehaviour
{
    // 씬을 떠날 때 참조하기 위해 변수로 저장
    private PlayerSwimming swimmingLogic;
    private PlayerOxygen oxygenLogic;

    void Start()
    {
        // "Player" 태그를 가진 플레이어 오브젝트를 찾습니다.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("씬에 'Player' 태그를 가진 오브젝트가 없습니다!");
            return;
        }

        // --- PlayerSwimming 활성화 ---
        swimmingLogic = player.GetComponent<PlayerSwimming>();
        if (swimmingLogic != null)
        {
            swimmingLogic.enabled = true;
            Debug.Log("PlayerSwimming 활성화 완료.");
        }

        // --- ▼▼▼▼▼ 수정된 부분 (PlayerOxygen 활성화 추가) ▼▼▼▼▼ ---
        // 플레이어에게서 PlayerOxygen 컴포넌트를 찾아 변수에 저장
        oxygenLogic = player.GetComponent<PlayerOxygen>();
        if (oxygenLogic != null)
        {
            // PlayerOxygen 스크립트를 활성화시킵니다.
            oxygenLogic.enabled = true;
            Debug.Log("PlayerOxygen 활성화 완료.");
        }
        // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---
    }

    // 이 Stage3Manager 오브젝트가 파괴될 때 (즉, Stage3 씬을 떠날 때) 호출됩니다.
    private void OnDestroy()
    {
        // 씬을 떠날 때 Stage3 전용 컴포넌트들을 비활성화하여
        // 다른 씬에 영향을 주지 않도록 합니다.
        if (swimmingLogic != null)
        {
            swimmingLogic.enabled = false;
        }

        // --- ▼▼▼▼▼ 수정된 부분 (PlayerOxygen 비활성화 추가) ▼▼▼▼▼ ---
        if (oxygenLogic != null)
        {
            oxygenLogic.enabled = false;
            // 산소 UI도 함께 비활성화합니다.
            if (oxygenLogic.oxygenSlider != null)
            {
                oxygenLogic.oxygenSlider.gameObject.SetActive(false);
            }
        }
        // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---
    }
}