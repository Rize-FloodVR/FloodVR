using UnityEngine;

public class SafetyStepManager : MonoBehaviour
{
    // ✅ 싱글톤 인스턴스
    public static SafetyStepManager Instance { get; private set; }

    // ✅ 전기 차단 여부 저장
    public bool IsElectricOff { get; private set; }

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }

    // ✅ 전기 차단 완료 시 호출할 함수
    public void SetElectricOff()
    {
        IsElectricOff = true;
        Debug.Log("✅ 전기 차단 상태 true로 설정됨");
    }
}
