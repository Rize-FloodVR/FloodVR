using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ElectricSwitchController : MonoBehaviour
{
    [SerializeField] private float moveDistance = 0.2f;
    [SerializeField] private float animationSpeed = 1f;
    
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable simpleInteractable;
    
    private bool isSwitchOn = true;
    private bool isAnimating = false;
    private bool isDisabled = false;
    private Vector3 onPosition;
    private Vector3 offPosition;

    [SerializeField] private AudioClip switchSound;


    public bool IsSwitchOn => isSwitchOn;
    public bool IsAnimating => isAnimating;
    public bool IsDisabled => isDisabled;
    
    void Start()
    {
        InitializeInteraction();
        InitializePositions();
    }
    
    void Update()
    {
        if (isAnimating)
        {
            AnimateSwitch();
        }
    }
    
    private void InitializeInteraction()
    {
        simpleInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        
        if (simpleInteractable == null)
        {
            simpleInteractable = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        }
        
        simpleInteractable.selectEntered.AddListener(OnSwitchActivated);
    }
    
    private void InitializePositions()
    {
        onPosition = transform.localPosition;
        offPosition = onPosition + Vector3.down * moveDistance;
    }
    
    private void OnSwitchActivated(SelectEnterEventArgs args)
    {
        ToggleSwitch();
    }
    
    public void ToggleSwitch()
    {
        if (isAnimating || isDisabled) return;
        
        if (isSwitchOn)
        {
            isSwitchOn = false;
            isAnimating = true;
            isDisabled = true;

            if (switchSound != null)
            {
                AudioSource.PlayClipAtPoint(switchSound, transform.position);
            }

            if (simpleInteractable != null)
            {
                simpleInteractable.enabled = false;
            }

            SafetyStepManager.Instance?.SetElectricOff();

            SafetyInteractionController safetyController = FindAnyObjectByType<SafetyInteractionController>();
            if (safetyController != null)
            {
                safetyController.OnElectricSwitchToggled(isSwitchOn);
            }
        }
    }
    
    private void AnimateSwitch()
    {
        Vector3 targetPosition = isSwitchOn ? onPosition : offPosition;
        
        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            targetPosition,
            animationSpeed * Time.deltaTime
        );
        
        if (Vector3.Distance(transform.localPosition, targetPosition) < 0.001f)
        {
            transform.localPosition = targetPosition;
            isAnimating = false;
        }
    }
    
    public void SetSwitchState(bool state)
    {
        if (!isDisabled && !isAnimating && state == false)
        {
            isSwitchOn = false;
            isAnimating = true;
            isDisabled = true;
        }
    }
}