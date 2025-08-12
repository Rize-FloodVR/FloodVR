using UnityEngine;
using TMPro;

public class UIToast : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;               // 패널 오브젝트 (활성/비활성)
    public TextMeshProUGUI label;           // 알림 텍스트

    [Header("Time Settings")]
    public float showTime = 1.2f;           // 표시 유지 시간

    public void Show(string msg)
    {
        StopAllCoroutines();
        StartCoroutine(CoShow(msg));
    }

    System.Collections.IEnumerator CoShow(string msg)
    {
        if (label) label.text = msg;
        if (panel) panel.SetActive(true);   // 패널 켜기

        yield return new WaitForSeconds(showTime);

        if (panel) panel.SetActive(false);  // 패널 끄기
    }
}