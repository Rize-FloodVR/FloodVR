using HurricaneVR.Framework.Components;   // HVRSocket, HVRGrabberBase
using HurricaneVR.Framework.Core;         // HVRGrabbable
using HurricaneVR.Framework.Core.Grabbers;
using UnityEngine;

public class SocketCollectRelay : MonoBehaviour
{
    public UIToast toast;                // 패널 버전 UIToast (앞서 만든 것)
    //public bool hideAfterCollect = true; // 수집 시 아이템 숨김

    // HVR Socket -> Grabbed 이벤트에 연결할 함수
    public void OnSocketGrabbed(HVRGrabberBase grabber, HVRGrabbable grabbable)
    {
        if (!grabbable) return;

        var meta = grabbable.GetComponent<CollectibleItem2>();
        if (meta == null) return;

        bool firstTime = InventoryManager.I?.Register(meta) ?? false;

        if (!firstTime && toast) toast.Show($"{meta.displayName} 이미 수집됨");
        //if (firstTime && hideAfterCollect) grabbable.gameObject.SetActive(false);
    }

    // (선택) 소켓에서 빠졌을 때 처리하고 싶으면 Released에 연결
    public void OnSocketReleased(HVRGrabberBase grabber, HVRGrabbable grabbable)
    {
        // 필요 시 로직 추가
        // toast?.Show("소켓에서 분리됨");
    }
}