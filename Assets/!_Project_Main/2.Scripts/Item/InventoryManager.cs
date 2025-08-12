using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager I;

    [Header("Optional")]
    public UIToast toast;

    [Tooltip("        ؾ   ϴ      (ǥ ÿ )")]
    public int totalNeeded = 5;

    private HashSet<string> collected = new HashSet<string>();

    void Awake() => I = this;

    public bool Register(CollectibleItem2 item)
    {
        if (item == null || string.IsNullOrEmpty(item.id)) return false;

        //  ̹                 
        if (!collected.Add(item.id)) return false;

        //  佺Ʈ       ൵ ǥ  
        if (toast)
        {
            var progress = totalNeeded > 0 ? $" ({collected.Count}/{totalNeeded})" : "";
            toast.Show($"{item.displayName}이 수집되었습니다{progress}");

            if (totalNeeded > 0 && collected.Count >= totalNeeded && MissionStepManager.Instance.CurrentStep == MissionStep.Escape)
                MissionStepManager.Instance.NextStep();
        }
        return true;
    }
}