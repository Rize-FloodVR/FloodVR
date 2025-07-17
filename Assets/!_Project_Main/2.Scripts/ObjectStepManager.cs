using UnityEngine;

public enum ObjectStep
{
    Phone,          //폰
    Flashlight,     // 손전등
    Wallet,          // 지갑
    Laptop,      // 노트북
    Complete

}



public class ObjectStepManager : MonoBehaviour
{
    public static ObjectStepManager Instance { get; private set; }


    public ObjectStep CurrentStep { get; set; } = ObjectStep.Phone;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    // 다음 미션 단계로 이동
    public void Step()
    {
        if (CurrentStep < ObjectStep.Complete)
        {
            OnStepChanged();
        }
    }

    // 미션 단계가 바뀔 때 호출
    private void OnStepChanged()
    {
        // 각 단계마다 팝업, 미션 활성화, 안내 등 연출 가능
        switch (CurrentStep)
        {
            case ObjectStep.Phone:
                MissionManager.Instance.ShowPopup("핸드폰 수집", 5f);
                break;
            case ObjectStep.Flashlight:
                MissionManager.Instance.ShowPopup("손전등 수집", 5f);
                break;
            case ObjectStep.Wallet:
                MissionManager.Instance.ShowPopup("지갑 수집", 5f);
                break;
            case ObjectStep.Laptop:
                MissionManager.Instance.ShowPopup("노트북 수집", 5f);
                break;
            case ObjectStep.Complete:
                MissionManager.Instance.ShowPopup("비상 물품 수집 완료!\n문이 열렸습니다.", 5f);
                break;
        }
    }
}
