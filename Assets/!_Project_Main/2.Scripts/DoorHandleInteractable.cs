using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorHandleInteractable : MonoBehaviour
{
    [SerializeField] private DoorController doorController;
    
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable simpleInteractable;
    
    void Start()
    { 
        simpleInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        
        if (simpleInteractable == null)
        {
            simpleInteractable = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        }
        
        simpleInteractable.selectEntered.AddListener(OnHandleActivated);
        if (doorController == null)
        {
            doorController = GetComponentInParent<DoorController>();
        }

    }
    
    private void OnHandleActivated(SelectEnterEventArgs args)
    {
        // 문 토글
        if (doorController != null)
        {
            doorController.ToggleDoor();
        }
        
        Debug.Log("도어 핸들 활성화");
    }
}