using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
public class CollectibleItem : MonoBehaviour
{
    public ItemType itemType;
    
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isGrabbed = false;
    private bool inputProcessed = false;
    
    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }
    
    void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        inputProcessed = false;
        Debug.Log($"{itemType} 그랩됨. 트리거를 눌러 수집.");
    }
    
    void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
        inputProcessed = false;
    }
    
    void Update()
    {
        if (isGrabbed && !inputProcessed)
        {
            // 시뮬레이터용 임시 코드. G를 눌러 그랩된 상태에서 C를 누리면 아이템 수집됨
            if (Input.GetKeyDown(KeyCode.C))
            {
                CollectItem();
                return;
            }
            // 컨트롤러 입력 확인
            if (CheckControllerInput())
            {
                CollectItem();
            }
        }
    }
    
    bool CheckControllerInput()
    {
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        InputDevice leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        
        // 트리거 버튼만 확인
        bool rightTrigger = false;
        bool leftTrigger = false;
        
        if (rightController.isValid)
        {
            rightController.TryGetFeatureValue(CommonUsages.triggerButton, out rightTrigger);
        }
        
        if (leftController.isValid)
        {
            leftController.TryGetFeatureValue(CommonUsages.triggerButton, out leftTrigger);
        }

        return rightTrigger || leftTrigger;
    }
    
    void CollectItem()
    {
        inputProcessed = true;
         
        Debug.Log($"{itemType} 수집 완료!");
        CollectionManager.Instance.CollectItem(itemType);
        Destroy(gameObject);
    }
}