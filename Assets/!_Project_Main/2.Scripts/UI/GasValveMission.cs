using UnityEngine;
using HurricaneVR.Framework.Components; // HVRPhysicsDial

public class GasValveMission : MonoBehaviour
{
    [Header("Dial")]
    public HVRPhysicsDial dial;          // 밸브(HVRPhysicsDial) drag&drop
    [Tooltip("목표 각도(예: 80~90)")]
    public float completeAngle = 80f;    // 몇 도 이상 돌리면 성공으로 볼지
    [Tooltip("디버그 로그 출력")]
    public bool debugLog = false;

    /*[Header("Popup")]
    public string popupText = "가스 차단 완료! 다음 단계로 이동하세요.";
    public float popupTime = 2f;*/

    // 내부 상태
    private bool done;
    private float startX;                 // 시작 X각도(축이 X라고 가정)

    void Start()
    {
        if (!dial) dial = GetComponent<HVRPhysicsDial>();
        // 시작 각도 저장 (HVR Physics Dial의 Save Joint Start Rotation 포즈와 맞추면 정확)
        startX = transform.localEulerAngles.x;
    }

    void Update()
    {
        if (done) return;

        // 현재 X축 각도와 시작 각도의 차( -180 ~ 180 )
        float deltaX = Mathf.DeltaAngle(startX, transform.localEulerAngles.x);
        float angle = Mathf.Abs(deltaX);

        if (debugLog) Debug.Log($"Valve angle: {angle:0.0}");

        if (angle >= completeAngle)
        {
            done = true;  // 한 번만 실행

           /* // 팝업
            MissionManager.Instance.ShowPopup(popupText, popupTime);*/

            // 미션 단계 전환(가스 단계일 때만)
            if (MissionStepManager.Instance.CurrentStep == MissionStep.Gas)
                MissionStepManager.Instance.NextStep();
        }
    }
}