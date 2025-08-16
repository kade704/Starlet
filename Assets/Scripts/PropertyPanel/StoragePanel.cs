using UnityEngine;
using UnityEngine.UI;

public class StoragePanel : MonoBehaviour
{
    [HideInInspector] public StorageProperty storageProperty;

    private new RectTransform transform;
    [SerializeField] private Text amountText;
    [SerializeField] private Transform itemPanelParent;
    [SerializeField] private StorageItemPanel itemPanelPrefab;

    private void Start()
    {
        OnStorageItemChanged();
        storageProperty.inventory.OnItemChanged.AddListener((_) => OnStorageItemChanged());
    }

    private void OnDestroy()
    {
        storageProperty.inventory.OnItemChanged.RemoveListener((_) => OnStorageItemChanged());
    }

    private void Awake()
    {
        transform = GetComponent<RectTransform>();
    }

    private void OnStorageItemChanged()
    {
        foreach (Transform child in itemPanelParent)
        {
            Destroy(child.gameObject);
        }

        var items = storageProperty.inventory.GetOwnedItems();
        foreach (var item in items)
        {
            var amount = storageProperty.inventory.GetItemAmount(item);
            var itemPanel = Instantiate(itemPanelPrefab, itemPanelParent);
            itemPanel.Initialize(item, amount);
        }

        var height = 60 + items.Length * 60;
        transform.sizeDelta = new Vector2(transform.sizeDelta.x, height);

        amountText.text = $"{storageProperty.inventory.totalAmount}/{storageProperty.inventory.capacity}";
    }
}
