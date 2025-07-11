using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class CollectibleItem : MonoBehaviour
{
    public ItemType itemType;
    
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isGrabbed = false;
    
    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }
    
    void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        Debug.Log($"{itemType} 그랩됨. 트리거를 눌러 수집.");
    }
    
    void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }
    
    private bool wasTriggeredLastFrame = false;
    
    void Update()
    {
        if (isGrabbed)
        {
            // 키보드 입력 (테스트용)
            if (Input.GetKeyDown(KeyCode.C))
            {
                CollectItem();
                return;
            }
            
            // 컨트롤러 입력 (VR용)
            InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            InputDevice leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            
            bool rightTrigger = false;
            bool leftTrigger = false;
            bool rightGrip = false;
            bool leftGrip = false;
            
            // 트리거와 그립 버튼 모두 확인
            if (rightController.isValid)
            {
                rightController.TryGetFeatureValue(CommonUsages.triggerButton, out rightTrigger);
                rightController.TryGetFeatureValue(CommonUsages.gripButton, out rightGrip);
            }
            
            if (leftController.isValid)
            {
                leftController.TryGetFeatureValue(CommonUsages.triggerButton, out leftTrigger);
                leftController.TryGetFeatureValue(CommonUsages.gripButton, out leftGrip);
            }
            
            bool isInputThisFrame = rightTrigger || leftTrigger || rightGrip || leftGrip;
            
            // 입력이 감지되면 수집
            if (isInputThisFrame && !wasTriggeredLastFrame)
            {
                CollectItem();
            }
            
            wasTriggeredLastFrame = isInputThisFrame;
        }
    }
    
    void CollectItem()
    {
        Debug.Log($"{itemType} 수집 완료!");
        CollectionManager.Instance.CollectItem(itemType);
        Destroy(gameObject);
    }
}