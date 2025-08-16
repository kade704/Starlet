using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform itemPanelParent;
    [SerializeField] private InventoryItemPanel itemPanelPrefab;
    [SerializeField] private Button backButton;

    private InventoryManager inventoryManager;

    void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void Start()
    {
        inventoryManager = GameManager.Instance.GetSystem<InventoryManager>();
        RefreshInventoryPanels();
    }

    private void RefreshInventoryPanels()
    {
        foreach (Transform child in itemPanelParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var itemAmount in inventoryManager.Inventory.ItemAmounts)
        {
            var panel = Instantiate(itemPanelPrefab, itemPanelParent);
            panel.Initialize(itemAmount.Key, itemAmount.Value);
        }

        var restItems = GameManager.Instance.GetSystem<GameResource>().GetItemAssets();
        foreach (var item in restItems)
        {
            if (!inventoryManager.Inventory.ItemAmounts.ContainsKey(item) && item.name != "Base")
            {
                var panel = Instantiate(itemPanelPrefab, itemPanelParent);
                panel.Initialize(item, 0);
            }
        }
    }

    private void OnBackButtonClicked()
    {
        SceneManager.LoadScene("Main");
    }
}
