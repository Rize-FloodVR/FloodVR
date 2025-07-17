using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance { get; private set; }

    public bool hasWallet = false;
    public bool hasFlashlight = false;
    public bool hasLaptop = false;
    public bool hasSmartphone = false;

    public bool AllItemsCollected => hasWallet && hasFlashlight && hasLaptop && hasSmartphone;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CollectItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Wallet:
                hasWallet = true;
                if (ObjectStepManager.Instance.CurrentStep == ObjectStep.Wallet)
                    ObjectStepManager.Instance.Step();
                Debug.Log("지갑 수집");
                break;

            case ItemType.Flashlight:
                hasFlashlight = true;
                if (ObjectStepManager.Instance.CurrentStep == ObjectStep.Flashlight)
                    ObjectStepManager.Instance.Step();
                Debug.Log("손전등 수집");
                break;

            case ItemType.Laptop:
                hasLaptop = true;
                if (ObjectStepManager.Instance.CurrentStep == ObjectStep.Laptop)
                    ObjectStepManager.Instance.Step();
                Debug.Log("랩탑 수집");
                break;

            case ItemType.Smartphone:
                hasSmartphone = true;
                if (ObjectStepManager.Instance.CurrentStep == ObjectStep.Phone)
                    ObjectStepManager.Instance.Step();
                Debug.Log("스마트폰 수집");
                break;
        }

        CheckCollectionComplete();
    }

    void CheckCollectionComplete()
    {
        if (AllItemsCollected)
        {
            Debug.Log("모든 아이템 수집 완료");
            if (MissionStepManager.Instance.CurrentStep == MissionStep.Escape)
                MissionStepManager.Instance.NextStep();
            OnAllItemsCollected();
        }
    }

    void OnAllItemsCollected()
    {
        Debug.Log("모든 아이템을 모았습니다. 이제 문을 열 수 있습니다!");
        // 자동으로 열지 않음! XR에서 수동으로 열도록 놔둠
    }




    public bool IsItemCollected(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Wallet => hasWallet,
            ItemType.Flashlight => hasFlashlight,
            ItemType.Laptop => hasLaptop,
            ItemType.Smartphone => hasSmartphone,
            _ => false
        };
    }
}
