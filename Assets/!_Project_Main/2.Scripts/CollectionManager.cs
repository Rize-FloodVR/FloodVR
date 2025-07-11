using UnityEngine;

public class CollectionManager : MonoBehaviour
{
public static CollectionManager Instance;

    private bool hasWallet = false;
    private bool hasFlashlight = false;
    private bool hasLaptop = false;
    private bool hasSmartphone = false;
    
    private bool AllItemsCollected => hasWallet && hasFlashlight && hasLaptop && hasSmartphone;
    
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
                Debug.Log("지갑 수집!");
                break;
                
            case ItemType.Flashlight:
                hasFlashlight = true;
                Debug.Log("손전등 수집!");
                break;
                
            case ItemType.Laptop:
                hasLaptop = true;
                Debug.Log("랩탑 수집!");
                break;
                
            case ItemType.Smartphone:
                hasSmartphone = true;
                Debug.Log("스마트폰 수집!");
                break;
        }
        
        CheckCollectionComplete();
    }
    
    void CheckCollectionComplete()
    {
        if (AllItemsCollected)
        {
            Debug.Log("모든 아이템 수집 완료");
            OnAllItemsCollected();
        }
    }
    
    void OnAllItemsCollected()
    {
        // 모든 아이템 수집 완료 시 처리(문 열림?)
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
