using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LockedDoorController : MonoBehaviour
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float animationSpeed = 0.8f;
    [SerializeField] private XRSimpleInteractable interactable; // XR 버튼

    private bool isOpen = false;
    private bool isAnimating = false;

    private float targetAngle = 0f;
    private float startAngle = 0f;
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
        startAngle = transform.eulerAngles.y;

        // XR 상호작용 연결
        if (interactable == null)
            interactable = GetComponent<XRSimpleInteractable>();

        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnDoorInteract);
        }
        else
        {
            Debug.LogWarning("XR Simple Interactable이 문에 없습니다.");
        }
    }

    void Update()
    {
        if (isAnimating)
        {
            AnimateDoor();
        }
    }

    private void OnDoorInteract(SelectEnterEventArgs args)
    {
        // 아이템 수집 확인
        if (!CollectionManager.Instance.AllItemsCollected)
        {
            Debug.Log("아이템을 모두 수집하지 않았습니다. 문을 열 수 없습니다.");
            return;
        }

        ToggleDoor();
    }

    public void ToggleDoor()
    {
        if (isAnimating) return;

        isOpen = !isOpen;
        targetAngle = isOpen ? startAngle - openAngle : startAngle;
        isAnimating = true;
    }

    private void AnimateDoor()
    {
        float currentY = transform.eulerAngles.y;
        float newY = Mathf.MoveTowardsAngle(currentY, targetAngle, animationSpeed * 90f * Time.deltaTime);

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, newY, transform.eulerAngles.z);

        if (Mathf.Abs(Mathf.DeltaAngle(newY, targetAngle)) < 0.1f)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, targetAngle, transform.eulerAngles.z);
            isAnimating = false;
        }
    }

    public bool IsOpen => isOpen;
    public bool IsAnimating => isAnimating;
}
