using UnityEngine;

public class MissionStepRelay : MonoBehaviour
{
    [Tooltip("이 오브젝트가 완료시켜야 하는 단계")]
    public MissionStep requiredStep;

    [Tooltip("중복 트리거 방지")]
    public bool once = true;

    private bool _fired;

    // 인스펙터 이벤트에 연결해서 호출 (파라미터 없는 이벤트용)
    public void AdvanceIfStepMatches()
    {
        if (_fired && once) return;
        if (MissionStepManager.Instance == null) return;

        if (MissionStepManager.Instance.CurrentStep == requiredStep)
        {
            _fired = true;
            MissionStepManager.Instance.NextStep();
        }
    }

    // 값이 넘어오는 이벤트(HVR 다이얼의 OnValueChanged(float))에 연결
    public void AdvanceIfValueInRange(float value)
    {
        if (_fired && once) return;
        if (MissionStepManager.Instance == null) return;

        if (MissionStepManager.Instance.CurrentStep != requiredStep) return;

        // 아래 임계치는 다이얼용으로 사용
        if (IsInRange(value))
        {
            _fired = true;
            MissionStepManager.Instance.NextStep();
        }
    }

    [Header("Tracker")]
    public bool trackerUsesNormalized = true; // true면 0~1 정규화값 사용, false면 각도 사용

    // HVRRotationTracker.Angle Changed(float angle, float normalized) 에 연결
    public void AdvanceFromTracker(float angle, float normalized)
    {
        // 기존 게이트/허용오차 로직을 그대로 쓰기 위해 내부 메서드로 전달
        AdvanceIfValueInRange(trackerUsesNormalized ? normalized : angle);
    }

    [Header("다이얼용 설정")]
    [Tooltip("목표 값(예: 0~1 정규화 또는 각도 변환 값)")]
    public float targetValue = 1f;

    [Tooltip("허용 오차")]
    public float tolerance = 0.05f;

    private bool IsInRange(float v) => Mathf.Abs(v - targetValue) <= tolerance;
}