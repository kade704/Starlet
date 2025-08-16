using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemPanel : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Text title;
    [SerializeField] private Text inventory;
    [SerializeField] private Text price;
    [SerializeField] private Image icon;
    [SerializeField] private Text buttonText;

    private Coroutine animationCoroutine;

    public void Initialize(ItemAsset item, bool isBuying, int inventoryAmount, int coin, Action action)
    {
        var itemPrice = isBuying ? item.buyPrice : item.sellPrice;
        price.text = itemPrice.ToString();
        title.text = item.title;
        inventory.text = $"보유 x{inventoryAmount}";
        icon.sprite = item.icon;
        buttonText.text = isBuying ? "구매" : "판매";
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => action.Invoke());

        button.interactable = isBuying ? coin >= itemPrice : inventoryAmount > 0;
    }

    public void PlayAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(PlayAnimationCoroutine());
    }

    private IEnumerator PlayAnimationCoroutine(float duration = 0.2f)
    {
        Color originalTextColor = inventory.color;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            inventory.transform.localScale = Vector3.Lerp(Vector3.one * 1.1f, Vector3.one, elapsedTime / duration);
            inventory.color = Color.Lerp(Color.white, originalTextColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
