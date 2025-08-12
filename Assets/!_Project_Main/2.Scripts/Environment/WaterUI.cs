using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaterUI : MonoBehaviour
{
    public Image waterBarFillImage;
    public TextMeshProUGUI depthText; // (선택) 수심 텍스트도 표시할 경우
    public GameObject gameOverPanel; // 게임오버 UI 패널

    // 수심 비율(0~1) 받아서 UI에 반영
    public void SetWaterLevel(float ratio, float height = -1f)
    {
        if (waterBarFillImage != null)
            waterBarFillImage.fillAmount = Mathf.Clamp01(ratio);

        if (depthText != null && height >= 0f)
        {
            depthText.text = $"{height * 100:F0} cm";
            if (height == 0.5f)
            {
                depthText.color = Color.red;  // 빨간색으로 변경
                if (gameOverPanel != null && !gameOverPanel.activeSelf)
                {
                    gameOverPanel.SetActive(true); // 게임오버 패널 표시
                    //Time.timeScale = 0f; // 게임 멈춤(선택)
                }
            }
            
            else
            {
                depthText.color = Color.white;  // 기본 흰색
            }
        }
           

    }
}