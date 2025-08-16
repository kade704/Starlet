using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorkshopUI : MonoBehaviour
{
    [SerializeField] private Transform itemPanelParent;
    [SerializeField] private CraftItemPanel itemPanelPrefab;
    [SerializeField] private Transform ingredientPanelParent;
    [SerializeField] private CraftIngredientPanel ingredientPanelPrefab;
    [SerializeField] private Text craftTitleText;
    [SerializeField] private Text craftInventoryText;
    [SerializeField] private Button craftButton;
    [SerializeField] private Button craftAmountMinusButton;
    [SerializeField] private Button craftAmountPlusButton;
    [SerializeField] private Text craftAmountText;
    [SerializeField] private Button backButton;

    private InventoryManager inventoryManager;
    private GameResource gameResource;
    private ItemAsset selectedItem;
    private CraftIngredientPanel[] ingredientPanels;
    private Coroutine inventoryAnimationCoroutine;
    private int craftAmount = 1;

    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClicked);
        craftButton.onClick.AddListener(OnCraftButtonClicked);
        craftAmountMinusButton.onClick.AddListener(OnCraftAmountMinusButtonClicked);
        craftAmountPlusButton.onClick.AddListener(OnCraftAmountPlusButtonClicked);
    }

    private void Start()
    {
        inventoryManager = GameManager.Instance.GetSystem<InventoryManager>();
        gameResource = GameManager.Instance.GetSystem<GameResource>();

        ingredientPanels = new CraftIngredientPanel[5];
        for (int i = 0; i < 5; i++)
        {
            var ingredientPanel = Instantiate(ingredientPanelPrefab, ingredientPanelParent);
            ingredientPanel.gameObject.SetActive(false);
            ingredientPanels[i] = ingredientPanel;
        }

        UpdateCraftAmountText();

        RefreshItemPanels();
    }

    private void RefreshItemPanels()
    {
        foreach (Transform child in itemPanelParent)
            Destroy(child.gameObject);

        var items = gameResource.GetItemAssets().Where(x => x.canCraft);
        foreach (var item in items)
        {
            var canCraft = HasAllIngredientItems(item, 1);
            var itemPanel = Instantiate(itemPanelPrefab, itemPanelParent);
            itemPanel.Initialize(item, canCraft, () => OnItemPanelClicked(item));
        }
    }

    private void UpdateCraftAmountText()
    {
        craftAmountText.text = $"x{craftAmount}";
    }

    private void OnCraftAmountMinusButtonClicked()
    {
        if (craftAmount > 1)
        {
            craftAmount--;
            UpdateCraftAmountText();
            RefreshCraftPanels(selectedItem);
        }
    }

    private void OnCraftAmountPlusButtonClicked()
    {
        craftAmount++;
        UpdateCraftAmountText();
        RefreshCraftPanels(selectedItem);
    }

    private void OnItemPanelClicked(ItemAsset item)
    {
        selectedItem = item;
        RefreshCraftPanels(item);
    }

    private void OnCraftButtonClicked()
    {
        if (selectedItem != null && HasAllIngredientItems(selectedItem, craftAmount))
        {
            CraftItem(selectedItem);
        }
        else
        {
            Debug.LogWarning("Not enough ingredients to craft this item.");
        }
    }

    private void RefreshCraftPanels(ItemAsset target)
    {
        craftTitleText.text = target.title;
        craftInventoryText.text = $"보유 x{inventoryManager.Inventory.GetItemAmount(target)}";
        var recipeItems = target.craftItems;
        for (int i = 0; i < ingredientPanels.Length; i++)
        {
            if (i < recipeItems.Count())
            {
                ingredientPanels[i].gameObject.SetActive(true);
                var item = recipeItems[i];
                var myAmount = inventoryManager.Inventory.GetItemAmount(item.item);
                ingredientPanels[i].Initialize(item.item, item.amount * craftAmount, myAmount);
            }
            else
            {
                ingredientPanels[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnBackButtonClicked()
    {
        SceneManager.LoadScene("Main");
    }

    private bool HasAllIngredientItems(ItemAsset item, int amount = 1)
    {
        foreach (var ingredient in item.craftItems)
        {
            if (!inventoryManager.Inventory.HasItemAmount(ingredient.item, ingredient.amount * amount))
                return false;
        }
        return true;
    }

    private void CraftItem(ItemAsset item)
    {
        foreach (var ingredient in item.craftItems)
        {
            inventoryManager.Inventory.RemoveItem(ingredient.item, ingredient.amount * craftAmount);
        }
        inventoryManager.Inventory.AddItem(item, craftAmount);

        RefreshItemPanels();
        RefreshCraftPanels(item);

        PlayAnimation();
    }

    private void PlayAnimation()
    {
        PlayInventoryAnimation();
        for (int i = 0; i < ingredientPanels.Length; i++)
        {
            if (ingredientPanels[i].gameObject.activeSelf)
            {
                ingredientPanels[i].PlayAnimation();
            }
        }
    }
    
    public void PlayInventoryAnimation()
    {
        if (inventoryAnimationCoroutine != null)
        {
            StopCoroutine(inventoryAnimationCoroutine);
        }
        inventoryAnimationCoroutine = StartCoroutine(PlayInventoryAnimationCoroutine());
    }

    private IEnumerator PlayInventoryAnimationCoroutine(float duration = 0.2f)
    {
        Color originalColor = craftInventoryText.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            craftInventoryText.color = Color.Lerp(Color.white, originalColor, elapsedTime / duration);
            craftInventoryText.transform.localScale = Vector3.Lerp(Vector3.one * 1.1f, Vector3.one, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        craftInventoryText.color = originalColor;
    }
}
