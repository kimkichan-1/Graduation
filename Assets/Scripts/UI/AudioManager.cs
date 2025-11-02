using UnityEngine;
using UnityEngine.Audio; // 믹서 사용에 필수
using UnityEngine.SceneManagement; // 씬 관리
using System.Collections; // 코루틴(페이드) 사용에 필수

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("오디오 믹서")]
    [SerializeField] private AudioMixer gameMixer;

    // ★★★ 1. BGM 전용 스피커(AudioSource) 참조
    [Header("BGM 설정")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private float fadeDuration = 1.0f; // BGM이 바뀔 때 페이드 시간

    private AudioClip currentBgmClip; // 현재 재생 중인 BGM
    private Coroutine fadeCoroutine; // 현재 진행 중인 페이드 코루틴

    public AudioMixer GameMixer => gameMixer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // <-- DontDestroyOnLoadManager가 처리
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 2. 스피커(AudioSource)가 없으면 직접 추가
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            // 믹서 그룹 연결 (4단계에서 했던 작업)
            bgmSource.outputAudioMixerGroup = gameMixer.FindMatchingGroups("BGM")[0];
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        LoadVolumeSettings();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadVolumeSettings();
    }

    private void LoadVolumeSettings()
    {
        float bgmVolume = PlayerPrefs.GetFloat("BgmVolume", 0.5f);
        float sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.5f);
        SetVolume("BgmVolume", bgmVolume, false);
        SetVolume("SfxVolume", sfxVolume, false);
    }

    // ★★★ 3. (새 함수) 씬의 '신청서'가 호출할 BGM 재생 함수 ★★★
    public void PlayBGM(AudioClip clipToPlay)
    {
        // 1. 신청서에 노래가 없거나, 이미 틀고 있는 노래면 무시
        if (clipToPlay == null || clipToPlay == currentBgmClip)
        {
            return;
        }

        // 2. 이전에 실행 중이던 페이드가 있다면 중지
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // 3. 새 노래로 부드럽게 교체하는 코루틴 시작
        fadeCoroutine = StartCoroutine(FadeMusic(clipToPlay));
    }

    // ★★★ 4. (새 함수) BGM 페이드 인/아웃 코루틴 ★★★
    private IEnumerator FadeMusic(AudioClip newClip)
    {
        float startVolume = 1f; // (믹서 볼륨 말고 AudioSource 자체 볼륨)
        float timer = 0f;

        // --- 페이드 아웃 ---
        if (bgmSource.isPlaying) // 노래가 나오고 있었다면
        {
            startVolume = bgmSource.volume;
            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime; // (Time.timeScale 0일 때도 작동)
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
                yield return null;
            }
            bgmSource.Stop();
        }

        // --- 새 노래 설정 및 페이드 인 ---
        currentBgmClip = newClip;
        bgmSource.clip = newClip;
        bgmSource.Play();

        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(0f, 1f, timer / fadeDuration); // (최종 볼륨은 믹서가 제어하므로 1f로 올림)
            yield return null;
        }

        bgmSource.volume = 1f;
        fadeCoroutine = null;
    }

    // (SetVolume 함수는 PlayerPrefs.Save()가 포함된 채로 그대로)
    public void SetVolume(string parameterName, float volume, bool saveToPrefs = true)
    {
        float dbVolume = Mathf.Log10(volume) * 20;
        if (volume == 0) dbVolume = -80f;
        gameMixer.SetFloat(parameterName, dbVolume);

        if (saveToPrefs)
        {
            PlayerPrefs.SetFloat(parameterName, volume);
            PlayerPrefs.Save();
        }
    }
}
