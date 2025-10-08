using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerOxygen : MonoBehaviour
{
    [Header("산소 설정")]
    public float maxOxygen = 100f;
    public float currentOxygen;
    public float oxygenDepletionRate = 2f;
    public float oxygenChargeRate = 20f;

    [Header("UI 설정")]
    public Slider oxygenSlider;

    // --- 연동을 위해 추가된 변수들 ---
    private PlayerHealth playerHealth; // PlayerHealth 스크립트를 참조할 변수
    private bool isPlayerDead = false; // 플레이어의 사망 상태를 추적할 변수
    // ---

    private bool isChargingOxygen = false;

    void Awake()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name != "Stage3")
        {
            if (oxygenSlider != null)
            {
                oxygenSlider.gameObject.SetActive(false);
            }
            this.enabled = false;
        }
    }

    void Start()
    {
        currentOxygen = maxOxygen;
        // 게임이 시작될 때 PlayerHealth 컴포넌트를 미리 찾아 저장해둡니다.
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        // 플레이어가 이미 죽었다면 더 이상 산소 로직을 실행하지 않습니다.
        if (isPlayerDead) return;

        if (isChargingOxygen)
        {
            currentOxygen += oxygenChargeRate * Time.deltaTime;
        }
        else
        {
            currentOxygen -= oxygenDepletionRate * Time.deltaTime;
        }

        currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);

        if (oxygenSlider != null)
        {
            oxygenSlider.value = currentOxygen / maxOxygen;
        }

        // ▼▼▼▼▼ 핵심 수정 부분 ▼▼▼▼▼
        // 산소가 0 이하로 떨어졌는지 확인
        if (currentOxygen <= 0)
        {
            // PlayerHealth 컴포넌트가 있고, 아직 죽지 않았다면 Die() 함수를 호출
            if (playerHealth != null)
            {
                playerHealth.Die(); // PlayerHealth의 Die() 함수를 호출
                isPlayerDead = true; // 사망 신호를 보냈으므로, 다시 호출하지 않도록 상태 변경
            }
        }
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲
    }

    public void StartCharging()
    {
        isChargingOxygen = true;
    }

    public void StopCharging()
    {
        isChargingOxygen = false;
    }
}
