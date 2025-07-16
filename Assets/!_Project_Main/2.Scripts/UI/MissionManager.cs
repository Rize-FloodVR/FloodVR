using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    public GameObject popupPanel; // 패널 전체
    public TextMeshProUGUI messageText;      // 메시지 텍스트

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        HidePopup();
    }

    public void ShowPopup(string message, float autoHideTime = 0)
    {
        messageText.text = message;
        popupPanel.SetActive(true);

        CancelInvoke();
        if (autoHideTime > 0)
            Invoke(nameof(HidePopup), autoHideTime);
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }
}