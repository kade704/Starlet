using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Button backButton;
    [SerializeField] private Transform coinContainer;
    [SerializeField] private Text coinText;
    [SerializeField] private Button sellCategoryButton;
    [SerializeField] private Button buyCategoryButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Transform shopPanelParent;
    [SerializeField] private ShopItemPanel shopPanelPrefab;
    [SerializeField] private Transform pointParent;
    [SerializeField] private Image pointPrefab;

    private ShopItemPanel[] shopPanels;
    private GameState gameState;
    private GameResource gameResource;
    private InventoryManager inventoryManager;
    private bool isBuying = true;
    private int currIndex = 0;
    private ItemAsset[] items;
    private Coroutine coinAnimationCoroutine;

    private const int PANELS_PER_PAGE = 5;

    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClicked);
        prevButton.onClick.AddListener(OnPrevButtonClicked);
        nextButton.onClick.AddListener(OnNextButtonClicked);
        buyCategoryButton.onClick.AddListener(OnBuyCategoryButtonClicked);
        sellCategoryButton.onClick.AddListener(OnSellCategoryButtonClicked);
    }

    private void Start()
    {
        gameState = GameManager.Instance.GetSystem<GameState>();
        titleText.text = $"{gameState.Station.title} 상점";
        coinText.text = gameState.Coin.ToString();

        gameResource = GameManager.Instance.GetSystem<GameResource>();
        inventoryManager = GameManager.Instance.GetSystem<InventoryManager>();

        shopPanels = new ShopItemPanel[PANELS_PER_PAGE];
        for (int i = 0; i < PANELS_PER_PAGE; i++)
        {
            shopPanels[i] = Instantiate(shopPanelPrefab, shopPanelParent);
            shopPanels[i].gameObject.SetActive(false);
        }

        UpdateItems();
        RefreshPoints();
        RefreshShopPanels();
    }

    private void OnBuyCategoryButtonClicked()
    {
        isBuying = true;
        currIndex = 0;
        buyCategoryButton.interactable = false;
        sellCategoryButton.interactable = true;
        UpdateItems();
        RefreshPoints();
        RefreshShopPanels();
    }

    private void OnSellCategoryButtonClicked()
    {
        isBuying = false;
        currIndex = 0;
        buyCategoryButton.interactable = true;
        sellCategoryButton.interactable = false;
        UpdateItems();
        RefreshPoints();
        RefreshShopPanels();
    }

    private void UpdateItems()
    {
        items = isBuying
            ? gameResource.GetItemAssets().Where(x => x.canBuy).ToArray()
            : inventoryManager.Inventory.GetOwnedItems().Where(x => x.canSell).ToArray();
    }

    private void UpdateCoinText()
    {
        coinText.text = gameState.Coin.ToString();
    }

    private void RefreshShopPanels()
    {
        for (int i = 0; i < PANELS_PER_PAGE; i++)
        {
            if (currIndex + i >= items.Length)
            {
                shopPanels[i].gameObject.SetActive(false);
            }
            else
            {
                var item = items.ElementAt(i + currIndex);
                var inventoryAmount = inventoryManager.Inventory.GetItemAmount(item);
                ShopItemPanel shopPanel = shopPanels[i];
                shopPanel.gameObject.SetActive(true);
                shopPanel.Initialize(item, isBuying, inventoryAmount, gameState.Coin, isBuying
                    ? () => OnBuyItem(item, shopPanel)
                    : () => OnSellItem(item, shopPanel));
            }
        }
    }

    private void RefreshPoints()
    {
        foreach (Transform child in pointParent)
        {
            Destroy(child.gameObject);
        }

        var pointCount = Mathf.CeilToInt((float)items.Length / PANELS_PER_PAGE);
        var pointIndex = Mathf.CeilToInt((float)currIndex / PANELS_PER_PAGE);
        for (int i = 0; i < pointCount; i++)
        {
            var pointImage = Instantiate(pointPrefab, pointParent);
            pointImage.color = i == pointIndex ? Color.white : Color.gray;
        }
    }

    private void OnPrevButtonClicked()
    {
        if (currIndex > 0)
        {
            currIndex -= PANELS_PER_PAGE;
            RefreshShopPanels();
            RefreshPoints();
        }
    }

    private void OnNextButtonClicked()
    {
        if (currIndex < items.Length - PANELS_PER_PAGE)
        {
            currIndex += PANELS_PER_PAGE;
            RefreshShopPanels();
            RefreshPoints();
        }
    }

    private void OnBackButtonClicked()
    {
        SceneManager.LoadScene("Main");
    }

    private void OnBuyItem(ItemAsset item, ShopItemPanel panel)
    {
        if (gameState.HasCoin(item.buyPrice))
        {
            gameState.RemoveCoin(item.buyPrice);

            inventoryManager.Inventory.AddItem(item, 1);
            PlayCoinAnimation();
            panel.PlayAnimation();
            RefreshShopPanels();
            UpdateCoinText();
        }
    }

    private void OnSellItem(ItemAsset item, ShopItemPanel panel)
    {
        if (inventoryManager.Inventory.HasItemAmount(item, 1))
        {
            gameState.AddCoin(item.sellPrice);

            inventoryManager.Inventory.RemoveItem(item, 1);
            PlayCoinAnimation();
            panel.PlayAnimation();
            RefreshShopPanels();
            UpdateCoinText();

            Debug.Log($"Sold item: {item.title}, Remaining coins: {gameState.Coin}");
        }
    }

    public void PlayCoinAnimation()
    {
        if (coinAnimationCoroutine != null)
        {
            StopCoroutine(coinAnimationCoroutine);
        }
        coinAnimationCoroutine = StartCoroutine(PlayCoinAnimationCoroutine());
    }

    private IEnumerator PlayCoinAnimationCoroutine(float duration = 1.0f)
    {
        float elapsedTime = 0f;
        int originalCoinAmount = int.Parse(coinText.text);
        while (elapsedTime < duration)
        {
            coinContainer.transform.localScale = Vector3.Lerp(Vector3.one * 1.1f, Vector3.one, elapsedTime / duration);
            originalCoinAmount = Mathf.RoundToInt(Mathf.Lerp(originalCoinAmount, gameState.Coin, elapsedTime / duration));
            coinText.text = originalCoinAmount.ToString();
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
