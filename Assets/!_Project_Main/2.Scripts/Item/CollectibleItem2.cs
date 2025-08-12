using UnityEngine;

public class CollectibleItem2 : MonoBehaviour
{
    [Tooltip("UI에 표시될 이름")]
    public string displayName = "손전등";

    [Tooltip("중복 수집 방지용 고유 ID")]
    public string id = "flashlight";
}