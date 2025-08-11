using UnityEngine;
using UnityEngine.UI;

public class ItemOutlineController : MonoBehaviour
{
    public QOutline outline;                 // 아웃라인 컴포넌트
    public Color normalColor = Color.white; // 기본 색
    public Color hoverColor = Color.yellow; // 호버 시 색

    void Awake()
    {
        if (!outline) outline = GetComponent<QOutline>();
        if (outline) outline.OutlineColor = normalColor;
    }

    // 호버 시작 시 호출
    public void SetHoverColor()
    {
        if (outline) outline.OutlineColor = hoverColor;
    }

    // 호버 종료 시 호출
    public void SetNormalColor()
    {
        if (outline) outline.OutlineColor = normalColor;
    }
}