using UnityEngine;
using UnityEngine.UI;

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
    private bool isPlayerDead = false;
    private bool isChargingOxygen = false;

    // 스크립트가 활성화될 때 호출됩니다.
    void OnEnable()
    {
        // Stage3에 진입하여 활성화될 때마다 산소를 가득 채우고 UI를 켭니다.
        currentOxygen = maxOxygen;
        isPlayerDead = false;
        if (oxygenSlider != null)
        {
            oxygenSlider.gameObject.SetActive(true);
        }
    }

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
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

        if (currentOxygen <= 0)
        {
            if (playerHealth != null && !isPlayerDead)
            {
                playerHealth.Die();
                isPlayerDead = true;
            }
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
