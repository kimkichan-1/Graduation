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

    private PlayerHealth playerHealth;
    private bool isPlayerDead = false; // 이 변수를 사용하여 사망 상태를 추적합니다.
    private bool isChargingOxygen = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Stage3")
        {
            this.enabled = true;
            if (oxygenSlider != null)
            {
                oxygenSlider.gameObject.SetActive(true);
            }
            currentOxygen = maxOxygen;
            isPlayerDead = false; // Stage3에 진입할 때마다 사망 상태 초기화
        }
        else
        {
            this.enabled = false;
            if (oxygenSlider != null)
            {
                oxygenSlider.gameObject.SetActive(false);
            }
        }
    }

    void Start()
    {
        currentOxygen = maxOxygen;
        playerHealth = GetComponent<PlayerHealth>();
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void Update()
    {
        // --- ▼▼▼▼▼ 1번 수정 부분 ▼▼▼▼▼ ---
        // playerHealth.isDead 대신, 이 스크립트 내부의 isPlayerDead 변수를 확인합니다.
        if (isPlayerDead) return;
        // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---

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

        // 산소가 0 이하로 떨어졌는지 확인
        if (currentOxygen <= 0)
        {
            // --- ▼▼▼▼▼ 2번 수정 부분 ▼▼▼▼▼ ---
            // playerHealth.isDead 대신, isPlayerDead 변수로 중복 호출을 방지합니다.
            if (playerHealth != null && !isPlayerDead)
            {
                playerHealth.Die(); // PlayerHealth의 Die() 함수를 호출
                isPlayerDead = true; // 사망 신호를 보냈으므로, 이 스크립트의 상태를 '사망'으로 변경
            }
            // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---
        }
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
