using UnityEngine;
using System.Collections.Generic;

public class SafetyInteractionController : MonoBehaviour
{
    [SerializeField] private bool isElectricSwitchOff = false;
    [SerializeField] private bool isGasValveClosed = false;
    
    [SerializeField] private CompletionMode completionMode = CompletionMode.RequireAll;
    [SerializeField] private int minimumRequiredActions = 1; // RequireMinimum 전용
    
    private Dictionary<string, bool> safetyStates = new Dictionary<string, bool>();
    
    public enum CompletionMode
    {
        RequireAll,      // 모든 안전 조치 필요
        RequireAny,      // 하나 이상의 안전 조치 필요
        RequireMinimum,  // 지정된 개수 이상의 안전 조치 필요
    }
    
    public System.Action<bool> OnElectricSwitchChanged;
    public System.Action<bool> OnGasValveChanged;
    public System.Action OnAllSafetyActionsCompleted;
    
    void Start()
    {
        InitializeSafetyStates();
        CheckSafetyCompletion();
    }
    
    private void InitializeSafetyStates()
    {
        safetyStates["ElectricSwitch"] = isElectricSwitchOff;
        safetyStates["GasValve"] = isGasValveClosed;
    }
    
    public void OnElectricSwitchToggled(bool isSwitchOn)
    {
        isElectricSwitchOff = !isSwitchOn;
        safetyStates["ElectricSwitch"] = isElectricSwitchOff;
        if (MissionStepManager.Instance.CurrentStep == MissionStep.Electric)
            MissionStepManager.Instance.NextStep();

        Debug.Log($"전기차단기 내려감");
        
        OnElectricSwitchChanged?.Invoke(isElectricSwitchOff);
        CheckSafetyCompletion();
    }
    
    public void OnGasValveToggled(bool isValveOpen)
    {
        isGasValveClosed = !isValveOpen;
        safetyStates["GasValve"] = isGasValveClosed;
        
        Debug.Log($"가스밸브 잠김");
        if (MissionStepManager.Instance.CurrentStep == MissionStep.Gas)
            MissionStepManager.Instance.NextStep();

        OnGasValveChanged?.Invoke(isGasValveClosed);
        CheckSafetyCompletion();
    }


    
    private void CheckSafetyCompletion()
    {
        bool isCompleted = false;
        
        switch (completionMode)
        {
            case CompletionMode.RequireAll:
                isCompleted = GetCompletedActionsCount() == safetyStates.Count;
                break;
                
            case CompletionMode.RequireAny:
                isCompleted = GetCompletedActionsCount() > 0;
                break;
                
            case CompletionMode.RequireMinimum:
                isCompleted = GetCompletedActionsCount() >= minimumRequiredActions;
                break;
                
        }
        
        if (isCompleted)
        {
            Debug.Log($"모든 완전 조치 완료 ({GetCompletedActionsCount()}/{safetyStates.Count})");

            OnAllSafetyActionsCompleted?.Invoke();
        }
    }
    
    private int GetCompletedActionsCount()
    {
        int count = 0;
        foreach (var state in safetyStates.Values)
        {
            if (state) count++;
        }
        return count;
    }
    
    public bool IsElectricSwitchOff => isElectricSwitchOff;
    public bool IsGasValveClosed => isGasValveClosed;
    public bool IsSafetyCompleted 
    {
        get
        {
            switch (completionMode)
            {
                case CompletionMode.RequireAll:
                    return GetCompletedActionsCount() == safetyStates.Count;
                    
                case CompletionMode.RequireAny:
                    return GetCompletedActionsCount() > 0;
                    
                case CompletionMode.RequireMinimum:
                    return GetCompletedActionsCount() >= minimumRequiredActions;
                    
                default:
                    return false;
            }
        }
    }
    
}