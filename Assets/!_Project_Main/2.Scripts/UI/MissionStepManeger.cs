using UnityEngine;

public enum MissionStep
{
    Electric,     // 전기 차단
    Gas,          // 가스 차단
    Collect,      // 오브젝트 수집
    Escape,       // 문 탈출
    Complete      // 완료
}

public class MissionStepManager : MonoBehaviour
{
    public static MissionStepManager Instance { get; private set; }

    public MissionStep CurrentStep { get; private set; } = MissionStep.Electric;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 다음 미션 단계로 이동
    public void NextStep()
    {
        if (CurrentStep < MissionStep.Complete)
        {
            CurrentStep++;
            OnStepChanged();
        }
    }



    // 미션 단계가 바뀔 때 호출
    private void OnStepChanged()
    {
        // 각 단계마다 팝업, 미션 활성화, 안내 등 연출 가능
        switch (CurrentStep)
        {
            case MissionStep.Electric:
                MissionManager.Instance.ShowPopup("감전 위험 감지됨! 전력 차단이 필요합니다.", 5f);
                break;
            case MissionStep.Gas:
                MissionManager.Instance.ShowPopup("전기 차단 완료!\n가스 누출 위험! 가스밸브를 차단하세요.", 5f);
                break;
            case MissionStep.Collect: 
                MissionManager.Instance.ShowPopup("가스 차단 완료!\n비상 물품을 수집하세요.", 5f);
                break;
            case MissionStep.Escape:
                MissionManager.Instance.ShowPopup("비상 물품 수집 완료!\n문이 열렸습니다.", 5f);
                break;
        }
    }
}

/* 예: 전기 차단 성공 시
if (MissionStepManager.Instance.CurrentStep == MissionStep.Electric)
    MissionStepManager.Instance.NextStep();

수집 오브젝트 4개 모두 모았을 때도 같은 방식으로
if (MissionStepManager.Instance.CurrentStep == MissionStep.Collect && collectedCount == 4)
    MissionStepManager.Instance.NextStep();*/