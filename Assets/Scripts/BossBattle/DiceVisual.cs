using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// 주사위 비주얼 연출을 담당하는 컴포넌트
/// Library of Ruina 스타일의 주사위 애니메이션을 구현합니다
/// </summary>
public class DiceVisual : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private Image diceBackground;
    [SerializeField] private TextMeshProUGUI diceValueText;
    [SerializeField] private Image diceTypeIcon;

    [Header("색상 설정")]
    [SerializeField] private Color attackColor = new Color(1f, 0.3f, 0.3f); // 빨간색
    [SerializeField] private Color defenseColor = new Color(0.3f, 0.6f, 1f); // 파란색

    [Header("애니메이션 설정")]
    [SerializeField] private float rollDuration = 0.8f; // 주사위 굴리는 시간
    [SerializeField] private float scaleUpAmount = 1.3f; // 확대 크기
    [SerializeField] private float rotationSpeed = 720f; // 회전 속도 (도/초)

    [Header("사운드")]
    [SerializeField] private AudioClip diceRollSound;
    [SerializeField] private AudioClip diceResultSound;

    private AudioSource audioSource;
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private int currentValue;
    private DiceType currentType;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        originalScale = transform.localScale;
    }

    /// <summary>
    /// 주사위 초기 설정
    /// </summary>
    public void Setup(DiceType type, int minValue, int maxValue)
    {
        currentType = type;

        // 타입에 따른 색상 설정
        Color typeColor = (type == DiceType.Attack) ? attackColor : defenseColor;
        if (diceBackground != null) diceBackground.color = typeColor;

        // 초기 텍스트 설정
        if (diceValueText != null)
        {
            diceValueText.text = $"{minValue}-{maxValue}";
            diceValueText.color = Color.white;
        }
    }

    /// <summary>
    /// 주사위 굴리기 애니메이션
    /// </summary>
    public IEnumerator AnimateRoll(int finalValue)
    {
        currentValue = finalValue;

        // 롤링 사운드 재생
        if (diceRollSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(diceRollSound);
        }

        float elapsed = 0f;

        while (elapsed < rollDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / rollDuration;

            // 회전 애니메이션
            float rotation = rotationSpeed * elapsed;
            rectTransform.localRotation = Quaternion.Euler(0, 0, rotation);

            // 스케일 애니메이션 (확대 -> 원래 크기)
            float scale;
            if (progress < 0.5f)
            {
                // 전반부: 확대
                scale = Mathf.Lerp(1f, scaleUpAmount, progress * 2f);
            }
            else
            {
                // 후반부: 축소
                scale = Mathf.Lerp(scaleUpAmount, 1f, (progress - 0.5f) * 2f);
            }
            transform.localScale = originalScale * scale;

            // 랜덤 숫자 표시 (롤링 효과)
            if (diceValueText != null && Random.value > 0.5f)
            {
                int randomValue = Random.Range(1, 20);
                diceValueText.text = randomValue.ToString();
            }

            yield return null;
        }

        // 최종 결과 표시
        rectTransform.localRotation = Quaternion.identity;
        transform.localScale = originalScale;

        if (diceValueText != null)
        {
            diceValueText.text = finalValue.ToString();
        }

        // 결과 사운드 재생
        if (diceResultSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(diceResultSound);
        }

        // 결과 강조 애니메이션
        yield return StartCoroutine(PulseAnimation());
    }

    /// <summary>
    /// 결과 강조 펄스 애니메이션
    /// </summary>
    private IEnumerator PulseAnimation()
    {
        float pulseDuration = 0.3f;
        float elapsed = 0f;

        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / pulseDuration;

            // 크기 변화 (1.0 -> 1.2 -> 1.0)
            float scale = 1f + Mathf.Sin(progress * Mathf.PI) * 0.2f;
            transform.localScale = originalScale * scale;

            yield return null;
        }

        transform.localScale = originalScale;
    }

    /// <summary>
    /// 승리 애니메이션
    /// </summary>
    public IEnumerator AnimateWin()
    {
        if (diceBackground != null)
        {
            Color originalColor = diceBackground.color;
            diceBackground.color = Color.yellow;

            yield return new WaitForSeconds(0.3f);

            diceBackground.color = originalColor;
        }

        // 확대 효과
        yield return StartCoroutine(ScaleAnimation(1.5f, 0.3f));
    }

    /// <summary>
    /// 패배 애니메이션
    /// </summary>
    public IEnumerator AnimateLose()
    {
        if (diceBackground != null)
        {
            Color originalColor = diceBackground.color;
            diceBackground.color = Color.gray;

            yield return new WaitForSeconds(0.3f);

            diceBackground.color = originalColor;
        }

        // 축소 효과
        yield return StartCoroutine(ScaleAnimation(0.7f, 0.3f));
    }

    /// <summary>
    /// 무승부 애니메이션
    /// </summary>
    public IEnumerator AnimateDraw()
    {
        // 좌우 흔들림
        float shakeDuration = 0.3f;
        float elapsed = 0f;
        Vector3 originalPosition = transform.localPosition;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float x = Mathf.Sin(elapsed * 30f) * 10f;
            transform.localPosition = originalPosition + new Vector3(x, 0, 0);
            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    /// <summary>
    /// 스케일 애니메이션 헬퍼
    /// </summary>
    private IEnumerator ScaleAnimation(float targetScale, float duration)
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = originalScale * targetScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, progress);
            yield return null;
        }

        transform.localScale = endScale;
    }

    /// <summary>
    /// 사라지는 애니메이션
    /// </summary>
    public IEnumerator AnimateFadeOut()
    {
        float fadeDuration = 0.5f;
        float elapsed = 0f;

        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - (elapsed / fadeDuration);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    public int GetCurrentValue() => currentValue;
    public DiceType GetDiceType() => currentType;
}
