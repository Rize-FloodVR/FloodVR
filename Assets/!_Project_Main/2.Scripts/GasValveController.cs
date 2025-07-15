using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GasValveController : MonoBehaviour
{
    [SerializeField] private Transform valveTransform;
    [SerializeField] private float rotationAngle = 90f;
    [SerializeField] private float animationSpeed = 1f;
    private Vector3 rotationAxis = Vector3.left;
    
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable simpleInteractable;
    
    private bool isValveOpen = true;
    private bool isAnimating = false;
    private bool isDisabled = false;
    private Quaternion openRotation;
    private Quaternion closedRotation;
    
    public bool IsValveOpen => isValveOpen;
    public bool IsAnimating => isAnimating;
    public bool IsDisabled => isDisabled;
    
    void Start()
    {
        InitializeInteraction();
        InitializeRotations();
    }
    
    void Update()
    {
        if (isAnimating)
        {
            AnimateValve();
        }
    }
    
    private void InitializeInteraction()
    {
        simpleInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        
        if (simpleInteractable == null)
        {
            simpleInteractable = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        }
        
        simpleInteractable.selectEntered.AddListener(OnValveActivated);
        
        if (valveTransform == null)
        {
            valveTransform = transform;
        }
    }
    
    private void InitializeRotations()
    {
        openRotation = valveTransform.localRotation;
        closedRotation = openRotation * Quaternion.AngleAxis(rotationAngle, rotationAxis);
    }
    
    private void OnValveActivated(SelectEnterEventArgs args)
    {
        if (!SafetyStepManager.Instance.IsElectricOff)
        {
            Debug.Log("⚠ 전기를 먼저 차단해야 가스밸브를 잠글 수 있습니다!");
            return;
        }
        ToggleValve();
    }
    
    public void ToggleValve()
    {
        if (isAnimating || isDisabled) return;
        
        if (isValveOpen)
        {
            isValveOpen = false;
            isAnimating = true;
            isDisabled = true;
            
            Debug.Log("가스밸브 닫힘 - 더 이상 조작할 수 없습니다");
            
            if (simpleInteractable != null)
            {
                simpleInteractable.enabled = false;
            }
            
            SafetyInteractionController safetyController = FindAnyObjectByType<SafetyInteractionController>();
            if (safetyController != null)
            {
                safetyController.OnGasValveToggled(isValveOpen);
            }
        }
    }
    
    private void AnimateValve()
    {
        Quaternion targetRotation = isValveOpen ? openRotation : closedRotation;
        
        valveTransform.localRotation = Quaternion.RotateTowards(
            valveTransform.localRotation,
            targetRotation,
            animationSpeed * rotationAngle * Time.deltaTime
        );
        
        if (Quaternion.Angle(valveTransform.localRotation, targetRotation) < 0.001f)
        {
            valveTransform.localRotation = targetRotation;
            isAnimating = false;
        }
    }
    public void SetValveState(bool state)
    {
        if (!isDisabled && !isAnimating && state == false)
        {
            isValveOpen = false;
            isAnimating = true;
            isDisabled = true;
        }
    }
}