using CoreEngine.UI;
using UnityEngine;


public abstract class BaseWindow_Windowed : BaseWindow
{
    private RectTransform rectTransform {  get; set; }
    private Vector2 AnchoredStartPos { get; set; }

    protected void Awake()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();


        AnchoredStartPos = rectTransform.anchoredPosition;
    }

    protected virtual void OnEnable()
    {
        // 창모드는 마지막 위치가 중심이 아닐 수 있음
        rectTransform.anchoredPosition = AnchoredStartPos;
    }
}
