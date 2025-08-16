using UnityEngine;
using UnityEngine.UI;

public class StorageItemPanel : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text titleText;
    [SerializeField] private Text amountText;

    public void Initialize(ItemAsset item, int amount)
    {
        titleText.text = item.title;
        iconImage.sprite = item.icon;

        amountText.text = $"x{amount}";
    }
}
