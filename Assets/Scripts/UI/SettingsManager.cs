using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bgmSource;
    public Slider volumeSlider;
    public Slider sfxSlider;

    [Header("Panels")]
    public GameObject settingsPanel;         // 설정 종류 창
    public GameObject audioSettingsPanel;    // 오디오 세부 설정 창

    void Start()
    {
        if (audioSettingsPanel == null)
        {
            Debug.LogWarning("⚠ audioSettingsPanel이 할당되지 않았습니다. Inspector에서 확인하세요.");
        }
        else
        {
            audioSettingsPanel.SetActive(false);
        }
        // 오디오 불러오기
        float savedBGM = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        volumeSlider.value = savedBGM;
        sfxSlider.value = savedSFX;
        bgmSource.volume = savedBGM;

        // 이벤트 연결
        volumeSlider.onValueChanged.AddListener(SetVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // 처음엔 세부 설정창 꺼두기
        audioSettingsPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void SetVolume(float value)
    {
        bgmSource.volume = value;
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    // === 패널 전환 함수들 ===
    public void OpenAudioSettings()
    {
        settingsPanel.SetActive(false);
        audioSettingsPanel.SetActive(true);
    }

    public void BackToSettingsMenu()
    {
        audioSettingsPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
}


