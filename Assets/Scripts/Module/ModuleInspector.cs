using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleInspector : MonoBehaviour
{
    public static ModuleInspector instance;

    [HideInInspector] public bool editing = false;

    private new RectTransform transform;
    private CanvasGroup canvasGroup;
    private Text titleText;
    private GameObject detachPanel;
    private Button detachButton;
    private Image hpBarImage;
    private Text hpText;
    private Player player;
    
    private SignalPanel signalPanelPrefab;
    private BatteryPanel energyPanelPrefab;
    private FloatPanel floatPanelPrefab;
    private StoragePanel storagePanelPrefab;

    private Module selectedModule;
    private List<GameObject> propertyPanels = new List<GameObject>();


    private void Awake()
    {
        instance = this;

        transform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        titleText = transform.Find("Title/Text").GetComponent<Text>();
        detachPanel = transform.Find("Detach").gameObject;
        detachButton = transform.Find("Detach/Button").GetComponent<Button>();
        hpBarImage = transform.Find("Health/Bar").GetComponent<Image>();
        hpText = transform.Find("Health/HP").GetComponent<Text>();

        signalPanelPrefab = Resources.Load<SignalPanel>("Prefabs/PropertyPanels/SignalPanel");
        energyPanelPrefab = Resources.Load<BatteryPanel>("Prefabs/PropertyPanels/EnergyPanel");
        floatPanelPrefab = Resources.Load<FloatPanel>("Prefabs/PropertyPanels/FloatPanel");
        storagePanelPrefab = Resources.Load<StoragePanel>("Prefabs/PropertyPanels/StoragePanel");
    }

    private void Start()
    {
        player = GameManager.Instance.GetSystem<Player>();
        player.onModuleSelected.AddListener(OnModuleSelected);
        player.onModuleDeselected.AddListener(OnModuleDeselected);

        detachButton.onClick.AddListener(OnDetachButtonClicked);
    }

    private void Update()
    {
        if (selectedModule == null) return;
        
        hpText.text = selectedModule.health.hp.ToString("F0") + "/" + selectedModule.health.maxHP + "hp";
        hpBarImage.fillAmount = selectedModule.health.hp / selectedModule.health.maxHP;
    }

    private void OnModuleSelected(Module module)
    {
        selectedModule = module;

        Utils.ShowCanvasGroup(canvasGroup);

        titleText.text = selectedModule.itemAsset.title;

        foreach (var property in module.properties)
        {
            if(!editing && 
                property.GetType().GetCustomAttributes(typeof(SavableProperty), false).Length > 0)
                continue;

            if (property is SignalProperty signalProperty)
            {
                var signalPanel = Instantiate(signalPanelPrefab, transform);
                signalPanel.signalProperty = signalProperty;
                propertyPanels.Add(signalPanel.gameObject);
            }
            if (property is FloatProperty floatProperty)
            {
                var floatPanel = Instantiate(floatPanelPrefab, transform);
                floatPanel.floatProperty = floatProperty;
                propertyPanels.Add(floatPanel.gameObject);
            }
            if (property is BatteryProperty batteryProperty)
            {
                var energyPanel = Instantiate(energyPanelPrefab, transform);
                energyPanel.batteryProperty = batteryProperty;
                propertyPanels.Add(energyPanel.gameObject);
            }
            if (property is StorageProperty storageProperty)
            {
                var storagePanel = Instantiate(storagePanelPrefab, transform);
                storagePanel.storageProperty = storageProperty;
                propertyPanels.Add(storagePanel.gameObject);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform);

        if (!editing) return;
        if (selectedModule.IsBase()) return;

        detachPanel.SetActive(true);
    }

    private void OnModuleDeselected(Module module)
    {
        selectedModule = null;

        Utils.HideCanvasGroup(canvasGroup);

        detachPanel.SetActive(false);      

        foreach (var propertyBox in propertyPanels)
        {
            Destroy(propertyBox);
        }
        propertyPanels.Clear();
    }

    private void OnDetachButtonClicked()
    {
        var temp = player.selectedModule;
        player.DeselectModule();
        player.spacecraft.DetachModule(temp);
    }
}
