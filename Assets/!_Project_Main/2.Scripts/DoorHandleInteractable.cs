using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DoorHandleInteractable : MonoBehaviour
{
    [SerializeField] private DoorController doorController;
    public CollectionManager collectionManager;
    public GameObject roomDoor;

    private XRSimpleInteractable simpleInteractable;


    void Start()
    {
        collectionManager = CollectionManager.Instance;
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        
        if (simpleInteractable == null)
        {
            simpleInteractable = gameObject.AddComponent<XRSimpleInteractable>();
        }
        
        simpleInteractable.selectEntered.AddListener(OnHandleActivated);
        if (doorController == null)
        {
            doorController = GetComponentInParent<DoorController>();
        }
    }
    private void OnHandleActivated(SelectEnterEventArgs args)
    {
        // Debug.Log(args.interactableObject.ToString());

        GameObject obj = args.interactableObject.transform.gameObject;
        GameObject objj = obj.transform.root.gameObject;
        Debug.Log(objj);
        // Debug.Log(objj.name == roomDoor.name);

        if (doorController != null)
        {
            if(collectionManager.AllItemsCollected || objj == roomDoor)
            {
                doorController.ToggleDoor();
            }
        }

        //if (doorController != null && collectionManager.AllItemsCollected)
        //{
        //    doorController.ToggleDoor();
        //}
    }
}