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
    public Image screenDimmer;          // 풀스크린 검은 이미지
    public float dimTargetAlpha = 0.75f;
    public float dimFadeTime = 0.5f;    // 언스케일드

    bool gameOverShown;

    public void SetWaterLevel(float ratio, float height = -1f)
    {
        if (waterBarFillImage) waterBarFillImage.fillAmount = Mathf.Clamp01(ratio);

        if (depthText && height >= 0f)
        {
            depthText.text = $"{height * 100f:F0} cm";

            // 정확히 0.5f == 비교는 위험 → 이상(>=)으로 체크
            if (!gameOverShown && height >= 0.5f)
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

        // 게임 멈춤 (UI 페이드는 언스케일드로 돌려서 정상 동작)
        Time.timeScale = 0f;

        // (선택) 효과음도 멈추고 싶으면:
        AudioListener.pause = true;
    }

    IEnumerator FadeDimmer(Image img, float target, float time)
    {
        img.raycastTarget = true; // 입력 막기 원치 않으면 false
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