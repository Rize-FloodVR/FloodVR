using System.Collections;
using UnityEngine;

public class UIFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public CanvasGroup tvCanvasGroup;
    public float fadeDuration = 1.5f;
    public AudioSource tvSound;

    void Start()
    {
        StartCoroutine(FadeInOut());
    }


    private IEnumerator FadeInOut()
    {
        // 1. Fade In (인트로 UI 등장)
        yield return StartCoroutine(Fade(canvasGroup, 0f, 1.5f));

        // 2. 대기 시간 (1.5초)
        yield return new WaitForSeconds(2.0f);

        // 3. Fade Out (인트로 UI 사라짐)
        yield return StartCoroutine(Fade(canvasGroup, 1f, 0f));

        // 4. 4초 대기
        yield return new WaitForSeconds(2f);

        // 5. 뉴스 소리 재생 및 화면 등장
        tvSound.Play();
        yield return StartCoroutine(Fade(tvCanvasGroup, 0f, 1f));
        

        Debug.Log("TV 등장 완료. 다음 연출 실행 가능!");
    }

    private IEnumerator Fade(CanvasGroup targetGroup, float from, float to)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            targetGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        targetGroup.alpha = to;
    }
}