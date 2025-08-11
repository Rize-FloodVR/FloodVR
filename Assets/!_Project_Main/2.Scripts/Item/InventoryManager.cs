using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager I;

    [Header("Optional")]
    public UIToast toast;

    [Tooltip("총 수집해야 하는 개수(표시용)")]
    public int totalNeeded = 5;

    private HashSet<string> collected = new HashSet<string>();

    void Awake() => I = this;

    public bool Register(CollectibleItem2 item)
    {
        if (item == null || string.IsNullOrEmpty(item.id)) return false;

        // 이미 수집됐으면 무시
        if (!collected.Add(item.id)) return false;

        // 토스트 및 진행도 표시
        if (toast)
        {
            var progress = totalNeeded > 0 ? $" ({collected.Count}/{totalNeeded})" : "";
            toast.Show($"{item.displayName}이 수집되었습니다{progress}");

            if (totalNeeded > 0 && collected.Count >= totalNeeded && MissionStepManager.Instance.CurrentStep == MissionStep.Electric)
                MissionStepManager.Instance.NextStep();
        }
        return true;
    }
}