using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ModuleSelector : MonoBehaviour
{
    public static ModuleSelector instance;

    private ModuleSelectorPanel[] itemPanels;
    private Text statusText;
    private Button leftButton;
    private Button rightButton;
    private InventoryManager inventoryManager;
    private GameResource gameResource;

    private ModuleItemAsset[] sortedItems;
    private ModuleType currType = ModuleType.None;
    private int currIndex = 0;

    private void Awake()
    {
        instance = this;

        itemPanels = GetComponentsInChildren<ModuleSelectorPanel>();

        statusText = transform.Find("Info/StatusText").GetComponent<Text>();
        
        leftButton = transform.Find("Left").GetComponent<Button>();
        leftButton.onClick.AddListener(OnLeftButtonClicked);

        rightButton = transform.Find("Right").GetComponent<Button>();
        rightButton.onClick.AddListener(OnRightButtonClicked);

        var allSort = transform.Find("Info/Icons/All").GetComponent<Button>();
        allSort.onClick.AddListener(() => Sort());

        var blockSort = transform.Find("Info/Icons/Block").GetComponent<Button>();
        blockSort.onClick.AddListener(() => Sort(ModuleType.Block));

        var machinerySort = transform.Find("Info/Icons/Machinery").GetComponent<Button>();
        machinerySort.onClick.AddListener(() => Sort(ModuleType.Machinery));

        var energySort = transform.Find("Info/Icons/Energy").GetComponent<Button>();
        energySort.onClick.AddListener(() => Sort(ModuleType.Energy));

        var resourceSort = transform.Find("Info/Icons/Resource").GetComponent<Button>();
        resourceSort.onClick.AddListener(() => Sort(ModuleType.Resource));
    }

    private void Start()
    {
        inventoryManager = GameManager.Instance.GetSystem<InventoryManager>();
        gameResource = GameManager.Instance.GetSystem<GameResource>();

        Sort();
    }

    public void Sort(ModuleType type = ModuleType.None)
    {
        currIndex = 0;
        currType = type;
        Refresh();
    }

    public void Refresh()
    {
        var moduleItems = inventoryManager.Inventory.GetOwnedModuleItems();

        if (currType == ModuleType.None)
        {
            sortedItems = moduleItems;
        }
        else
        {
            var result = new List<ModuleItemAsset>();
            foreach (var moduleItem in moduleItems)
            {
                if (moduleItem.type == currType)
                {
                    result.Add(moduleItem);
                }
            }
            sortedItems = result.ToArray();
        }

        for (int i = 0; i < 7; i++)
        {
            if (sortedItems.Length > currIndex + i)
            { 
                var moduleItem = sortedItems[currIndex + i];
                var amount = inventoryManager.Inventory.GetItemAmount(moduleItem);
                var typeSprite = gameResource.GameResourceAsset.moduleTypeSprites[moduleItem.type];

                itemPanels[i].itemAsset = moduleItem;
                itemPanels[i].canvasGroup.alpha = 1.0f;
                itemPanels[i].icon.sprite = moduleItem.icon;
                itemPanels[i].icon.color = Color.white;
                itemPanels[i].type.sprite = typeSprite;
                itemPanels[i].type.color = Color.white;
                itemPanels[i].title.text = moduleItem.title;
                itemPanels[i].amount.text = $"x{amount}";
            }
            else  
            {
                itemPanels[i].itemAsset = null;
                itemPanels[i].canvasGroup.alpha = 0.5f;
                itemPanels[i].icon.color = Color.clear;
                itemPanels[i].type.color = Color.clear;
                itemPanels[i].title.text = "비어있음";
                itemPanels[i].amount.text = "";
            }
        }

        var start = currIndex + 1;
        var end = Mathf.Min(currIndex + 7, sortedItems.Length);

        statusText.text = $"{start}~{end}/{sortedItems.Length}";

        leftButton.image.color = start == 1 ? Color.grey : Color.white;
        rightButton.image.color = (end >= sortedItems.Length) ? Color.grey : Color.white;
    }

    private void OnLeftButtonClicked()
    {
        if (currIndex > 0)
        {
            currIndex -= 7;
            Refresh();
        }
    }

    private void OnRightButtonClicked()
    {
        if(currIndex < sortedItems.Length - 7)
        {
            currIndex += 7;
            Refresh();
        }
    }
}
