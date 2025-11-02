using UnityEngine;
using UnityEngine.UI; // Slider 사용에 필수

public class SettingsUI : MonoBehaviour
{
    // 1. 인스펙터에서 씬에 있는 슬라이더를 연결
    [Header("UI 슬라이더 연결")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        // AudioManager가 없으면 Stage1부터 시작 안 한 것임
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager가 없습니다! Main부터 시작해야 합니다.");
            return;
        }

        // 2. 씬이 열릴 때, PlayerPrefs에 저장된 현재 볼륨 값으로 슬라이더를 초기화
        bgmSlider.value = PlayerPrefs.GetFloat("BgmVolume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume", 0.5f);

        // 3. 슬라이더를 움직일 때마다 AudioManager의 SetVolume 함수를 호출하도록 연결
        bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
    }

    // BGM 슬라이더가 바뀔 때
    private void OnBgmSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVolume("BgmVolume", value, true);
        }
    }

    // SFX 슬라이더가 바뀔 때
    private void OnSfxSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVolume("SfxVolume", value, true);
        }
    }

    // 4. (중요) 리스너 제거: 이 씬이 파괴될 때(예: Back 버튼) 연결을 끊어줘야 함
    void OnDestroy()
    {
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.RemoveListener(OnBgmSliderChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);
        }
    }
}
