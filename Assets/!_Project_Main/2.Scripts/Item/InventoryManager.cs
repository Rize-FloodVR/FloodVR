using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager I;

    [Header("Optional")]
    public UIToast toast;

    [Tooltip("총 모아야 하는 오브젝트 개수")]
    public int totalNeeded = 5;

    private HashSet<string> collected = new HashSet<string>();

    void Awake() => I = this;

    public bool Register(CollectibleItem2 item)
    {
        if (item == null || string.IsNullOrEmpty(item.id)) return false;

        // 이미 수집되었으면 무시
        if (!collected.Add(item.id)) return false;

        //  UI 표시
        if (toast)
        {
            var progress = totalNeeded > 0 ? $" ({collected.Count}/{totalNeeded})" : "";
            toast.Show($"{item.displayName}이 수집되었습니다{progress}");

            if (totalNeeded > 0 && collected.Count >= totalNeeded && MissionStepManager.Instance.CurrentStep == MissionStep.Collect)
                MissionStepManager.Instance.NextStep();
        }
        return true;
    }
}