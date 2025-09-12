
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonBackEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image buttonImage;
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(0.8f, 0.9f, 1f); // Inspector에서 수정 가능

    void Start()
    {
        // 이 스크립트가 붙어있는 게임 오브젝트의 Image 컴포넌트를 가져옵니다.
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            // 시작할 때 기본 색상으로 설정합니다.
            buttonImage.color = normalColor;
        }
    }

    // 마우스 포인터가 버튼 영역으로 들어왔을 때 호출됩니다.
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonImage != null)
        {
            buttonImage.color = hoverColor;
        }
    }

    // 마우스 포인터가 버튼 영역에서 나갔을 때 호출됩니다.
    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }
}
