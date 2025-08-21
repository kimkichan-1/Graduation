using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    #region Singleton
    public static TooltipManager instance;
    void Awake()
    {
        if (instance != null) { Destroy(gameObject); }
        else { instance = this; }
    }
    #endregion

    public Tooltip tooltip;
    public RectTransform canvasRectTransform;

    void Start()
    {
        if (tooltip != null)
        {
            tooltip.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (tooltip != null && tooltip.gameObject.activeSelf)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, null, out localPoint);
            tooltip.transform.localPosition = localPoint;
        }
    }

    public void ShowTooltip(string name, string description)
    {
        if (tooltip != null)
        {
            tooltip.nameText.text = name;
            tooltip.descriptionText.text = description;
            tooltip.gameObject.SetActive(true);
        }
    }

    public void HideTooltip()
    {
        if (tooltip != null)
        {
            tooltip.gameObject.SetActive(false);
        }
    }
}
