using UnityEngine;
using HurricaneVR.Framework.Components;

public class ExitDoorHandleController : MonoBehaviour
{
    private HVRRotationTracker rotationTracker;
    public bool conditionMet = false;

    private void Awake()
    {
        if (rotationTracker == null)
        {
            rotationTracker = GetComponent<HVRRotationTracker>();
            
            if (rotationTracker == null)
            {
                enabled = false;
            }
        }
    }

    private void Start()
    {
        rotationTracker.enabled = false;
        conditionMet = false;
   }

    private void Update()
    {
        bool currentConditionMet = MissionStepManager.Instance.CurrentStep == MissionStep.Escape;

        if (currentConditionMet != conditionMet)
        {
            conditionMet = currentConditionMet;

            if (conditionMet)
            {
                rotationTracker.enabled = true;
            }
            else
            {
                rotationTracker.enabled = false;
            }
        }
    }

    public void SetHandleEnabled(bool enable)
    {
        if (rotationTracker != null)
        {
            rotationTracker.enabled = enable;
            conditionMet = enable;
            
        }
    }
}

