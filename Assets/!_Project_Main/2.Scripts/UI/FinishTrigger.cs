using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool pauseOnShow = true;

    bool done;

    private void OnTriggerEnter(Collider other)
    {
        if (done) return;
        if (!other.CompareTag(playerTag)) return;

        done = true;
        if (gameOverPanel) gameOverPanel.SetActive(true);

        // 선택: 게임 멈추기
        if (pauseOnShow) Time.timeScale = 0f;

        // 선택: 플레이어 조작 잠그기 (있으면 비활성)
        // var move = FindObjectOfType<PlayerMove>(); if (move) move.enabled = false;
    }
}