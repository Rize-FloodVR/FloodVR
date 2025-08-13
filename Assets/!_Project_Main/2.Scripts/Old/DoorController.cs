using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorController : MonoBehaviour
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float animationSpeed = 0.8f;
    private bool isOpen = false;
    private bool isAnimating = false;
    [SerializeField] private AudioClip doorOpenSound;

    private float targetAngle = 0f;
    private float startAngle = 0f;
    private Quaternion initialRotation;
    
    void Start()
    {
        initialRotation = transform.rotation;
        startAngle = transform.eulerAngles.y;
    }
    
    void Update()
    {
        if (isAnimating)
        {
            AnimateDoor();
        }
    }
    
    public void ToggleDoor()
    {
        if (isAnimating) return;
        if (doorOpenSound != null)
        {
            AudioSource.PlayClipAtPoint(doorOpenSound, transform.position);
        }

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