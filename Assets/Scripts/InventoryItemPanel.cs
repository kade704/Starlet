using UnityEngine;
using UnityEngine.UI;

public class InventoryItemPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text title;
    [SerializeField] private Text inventory;
    [SerializeField] private Image icon;

    public void Initialize(ItemAsset item, int inventoryAmount)
    {
        title.text = item.title;
        icon.sprite = item.icon;

        if (inventoryAmount > 0)
        {
            canvasGroup.alpha = 1f;
            inventory.text = $"보유 x{inventoryAmount}";
        }
        else
        {
            canvasGroup.alpha = 0.5f;
            inventory.text = "미보유";
        }
    }
}
