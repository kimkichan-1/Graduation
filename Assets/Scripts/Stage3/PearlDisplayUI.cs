using UnityEngine;
using TMPro;

public class PearlDisplayUI : MonoBehaviour
{
    // '싱글턴(Singleton)' 패턴: 이 스크립트를 어디서든 쉽게 접근할 수 있게 해줍니다.
    public static PearlDisplayUI instance;

    [Header("UI 설정")]
    [SerializeField] private TextMeshProUGUI pearlCounterText;

    [Header("데이터")]
    [SerializeField] private int maxPearls = 3;
    private int currentPearls = 0;
    public int GetCurrentPearls() { return currentPearls; }

    void Awake()
    {
        // 씬에 이 스크립트의 인스턴스가 단 하나만 존재하도록 보장합니다.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 게임 시작 시 UI 초기화
        UpdatePearlUI();
    }

    // [Public 함수] 진주를 획득했을 때 외부에서 호출합니다.
    public void AddPearl(int amount = 1)
    {
        currentPearls += amount;
        if (currentPearls > maxPearls)
        {
            currentPearls = maxPearls;
        }
        Debug.Log("진주 획득! 현재: " + currentPearls + "/" + maxPearls);

        UpdatePearlUI();
    }

    // [Public 함수] 진주를 사용하려고 할 때 외부에서 호출합니다.
    public bool UsePearls(int amount)
    {
        if (currentPearls >= amount)
        {
            currentPearls -= amount;
            Debug.Log(amount + "개 진주 사용. 현재: " + currentPearls + "/" + maxPearls);
            UpdatePearlUI();
            return true; // 사용 성공
        }
        else
        {
            Debug.Log("진주가 부족합니다.");
            return false; // 사용 실패
        }
    }

    // [Private 함수] UI 텍스트를 업데이트하는 내부 기능입니다.
    private void UpdatePearlUI()
    {
        if (pearlCounterText != null)
        {
            pearlCounterText.text = $"{currentPearls} / {maxPearls}";
        }
    }
}