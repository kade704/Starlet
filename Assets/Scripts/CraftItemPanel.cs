using System;
using UnityEngine;
using UnityEngine.UI;

public class CraftItemPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private Text title;

    public void Initialize(ItemAsset item, bool canCraft, Action onClickAction = null)
    {
        if (onClickAction != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClickAction());
        }

        title.text = item.title;
        icon.sprite = item.icon;

        canvasGroup.alpha = canCraft ? 1f : 0.5f;
    }
}
