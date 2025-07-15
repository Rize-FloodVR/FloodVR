using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorHandleInteractable : MonoBehaviour
{
    [SerializeField] private DoorController doorController;
    public CollectionManager collectionManager;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable simpleInteractable;
    
    void Start()
    {
        collectionManager = CollectionManager.Instance;
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
        if (doorController != null && collectionManager.AllItemsCollected)
        {
            doorController.ToggleDoor();
        }
    }
}