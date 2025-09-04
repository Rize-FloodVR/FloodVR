using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WaterUI : MonoBehaviour
{
    public Image waterBarFillImage;
    public TextMeshProUGUI depthText;
    public GameObject gameOverPanel;

    [Header("Screen Dim")]
    public Image screenDimmer;          
    public float dimTargetAlpha = 0.75f;
    public float dimFadeTime = 0.5f;  

    bool gameOverShown;

    public void SetWaterLevel(float ratio, float height = -1f)
    {
        if (waterBarFillImage) waterBarFillImage.fillAmount = Mathf.Clamp01(ratio);

        if (depthText && height >= 0f)
        {
            depthText.text = $"{height * 100f:F0} cm";

  
            if (!gameOverShown && height == 0.5f)
            {
                depthText.color = Color.red;
                TriggerGameOver();
            }
            else if (!gameOverShown)
            {
                depthText.color = Color.white;
            }
        }
    }

    void TriggerGameOver()
    {
        if (gameOverShown) return;
        gameOverShown = true;

        // 화면 디밍
        if (screenDimmer) StartCoroutine(FadeDimmer(screenDimmer, dimTargetAlpha, dimFadeTime));

        if (gameOverPanel && !gameOverPanel.activeSelf)
            gameOverPanel.SetActive(true);

        // 게임 종료
        Time.timeScale = 0f;

        // 오디오 중지
        AudioListener.pause = true;
    }

    IEnumerator FadeDimmer(Image img, float target, float time)
    {
        img.raycastTarget = true;
        Color c = img.color;
        float start = c.a;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / time;
            c.a = Mathf.Lerp(start, target, t);
            img.color = c;
            yield return null;
        }
    }
}