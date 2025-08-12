using UnityEngine;

public enum MissionStep
{
    Start,          //����
    Electric,     // ���� ����
    Gas,          // ���� ����
    Collect,      // ������Ʈ ����
    Escape,       // �� Ż��
    Complete      // �Ϸ�
}

public class MissionStepManager : MonoBehaviour
{
    public static MissionStepManager Instance { get; private set; }
    public MissionStep CurrentStep { get; private set; } = MissionStep.Electric;

    [Header("Voice (2D AudioSource ����)")]
    public AudioSource voice;
    public AudioClip electricClip, gasClip, collectClip, escapeClip;

    void Awake() { if (Instance == null) Instance = this; else Destroy(gameObject); }
    void Start() { OnStepChanged(); }

    public void NextStep()
    {
        if (CurrentStep < MissionStep.Complete) { CurrentStep++; OnStepChanged(); }
    }

    void OnStepChanged()
    {
        switch (CurrentStep)
        {
            case MissionStep.Electric:
                MissionManager.Instance.ShowPopup("침수 경보가 발생했습니다.\n감전 위험 감지! 전력 차단이 필요합니다.", 5f);
                Play(electricClip);
                break;

            case MissionStep.Gas:
                MissionManager.Instance.ShowPopup("전기 차단 완료!\n가스 누출 위험! 가스밸브를 차단하세요.", 5f);
                Play(gasClip);
                break;

            case MissionStep.Collect:
                MissionManager.Instance.ShowPopup("가스 차단 완료!\n비상 물품을 수집하세요.", 5f);
                Play(collectClip);
                break;

            case MissionStep.Escape:
                MissionManager.Instance.ShowPopup("비상 물품 수집 완료!\n현관문으로 탈출하세요.", 5f);
                Play(escapeClip);
                break;
        }
    }

    void Play(AudioClip clip)
    {
        if (voice && clip) voice.PlayOneShot(clip);
    }
}
/* ��: ���� ���� ���� ��
if (MissionStepManager.Instance.CurrentStep == MissionStep.Electric)
    MissionStepManager.Instance.NextStep();

���� ������Ʈ 4�� ��� ����� ���� ���� �������
if (MissionStepManager.Instance.CurrentStep == MissionStep.Collect && collectedCount == 4)
    MissionStepManager.Instance.NextStep();*/