using UnityEngine;
using UnityEngine.UI;

public class WaterUI : MonoBehaviour
{
    public Image waterBarFillImage;
    public Text depthText; // (선택) 수심 텍스트도 표시할 경우

    // 수심 비율(0~1) 받아서 UI에 반영
    public void SetWaterLevel(float ratio, float height = -1f)
    {
        if (waterBarFillImage != null)
            waterBarFillImage.fillAmount = Mathf.Clamp01(ratio);

        if (depthText != null && height >= 0f)
            depthText.text = $"{height:F2}cm";
    }
}