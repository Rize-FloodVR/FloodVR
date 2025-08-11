/*using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using UnityEngine;
using UnityEngine.UI;

public class HVRHoverOutline : HVRGrabbable
{
    public QOutline outline;
    public Color hoverColor = Color.yellow;
    private Color originalColor;

    protected override void Awake()
    {
        base.Awake();
        if (outline != null)
            originalColor = outline.OutlineColor;
    }

    // Hover 시작
    public override void OnHandGrabberEnter(HVRHandGrabber hand)
    {
        base.OnHandGrabberEnter(hand);
        if (outline != null)
            outline.OutlineColor = hoverColor;
    }

    // Hover 종료
    public override void OnHandGrabberHoverExit(HVRHandGrabber hand)
    {
        base.OnHandGrabberHoverExit(hand);
        if (outline != null)
            outline.OutlineColor = originalColor;
    }
}*/