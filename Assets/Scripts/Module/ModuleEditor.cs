using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ModuleEditor : MonoBehaviour
{
    public static ModuleEditor instance;

    [HideInInspector] public Module draggedModule;
    [HideInInspector] public UnityEvent<Module> onModuleDragBegin = new UnityEvent<Module>();
    [HideInInspector] public UnityEvent<Module> onModuleDragEnd = new UnityEvent<Module>();
    [HideInInspector] public UnityEvent<Module> onModuleDragging = new UnityEvent<Module>();

    private InventoryManager inventoryManager;
    private float dragModuleActualAngle;
    private float dragModuleIdealAngle;
    private Connector selectedConnector;
    private List<Connector> attachableConnectors = new();
    private Player player;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = GameManager.Instance.GetSystem<Player>();

        inventoryManager = GameManager.Instance.GetSystem<InventoryManager>();

        InputManager.instance.onDragBegin.AddListener(OnDragBegin);
        InputManager.instance.onDragEnd.AddListener(OnDragEnd);
        InputManager.instance.onDragging.AddListener(OnDragging);

        player.spacecraft.onModuleAttached.AddListener(OnModuleAttached);
        player.spacecraft.onModuleDetached.AddListener(OnModuleDetached);
        player.spacecraft.onModuleDetachedComplete.AddListener(OnModuleDetachedComplete);

        RefreshAttachableConnector();
    }

    private void OnDragBegin(Vector2 screenPos)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        var itemPanel = GetItemPanelAtPosition(screenPos);
        if (itemPanel == null || itemPanel.itemAsset == null || itemPanel.amount.text == "0") return;

        var prefab = itemPanel.itemAsset.prefab;
        draggedModule = Instantiate(prefab, worldPos, Quaternion.identity);
        draggedModule.itemAsset = itemPanel.itemAsset;
        onModuleDragBegin.Invoke(draggedModule);
        draggedModule.onPlayerDragBegin.Invoke();
        draggedModule.collider.isTrigger = true;
        dragModuleIdealAngle = draggedModule.transform.eulerAngles.z;
        dragModuleActualAngle = dragModuleIdealAngle;
    }

    private void OnDragEnd(Vector2 screenPos)
    {
        if (draggedModule == null) return;

        onModuleDragEnd.Invoke(draggedModule);
        draggedModule.onPlayerDragEnd.Invoke();

        if (selectedConnector == null || CheckModuleCollided(draggedModule))
        {
            Destroy(draggedModule.gameObject);
        }
        else
        {
            draggedModule.collider.isTrigger = false;
            player.spacecraft.AttachModule(draggedModule, selectedConnector);
            inventoryManager.Inventory.RemoveItem(draggedModule.itemAsset, 1);
            ModuleSelector.instance.Refresh();
        }
        draggedModule = null;
    }

    private void OnDragging(Vector2 screenPos)
    {
        if (draggedModule == null) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        onModuleDragging.Invoke(draggedModule);
        draggedModule.onPlayerDragged.Invoke();

        selectedConnector = FindProperConnector(worldPos);
        if (selectedConnector != null)
        {
            draggedModule.transform.position = selectedConnector.transform.position;
            dragModuleIdealAngle = selectedConnector.transform.eulerAngles.z;
        }
        else
        {
            draggedModule.transform.position = worldPos;
        }
        dragModuleActualAngle = Mathf.LerpAngle(dragModuleActualAngle, dragModuleIdealAngle, Time.deltaTime * 10);
        draggedModule.transform.eulerAngles = new Vector3(0, 0, dragModuleActualAngle);
    }

    private void OnModuleAttached(SpacecraftEventInfo info)
    {
        RefreshAttachableConnector();
    }

    private void OnModuleDetached(SpacecraftEventInfo info)
    {
        inventoryManager.Inventory.AddItem(info.module.itemAsset, 1);
        Destroy(info.module.gameObject);
    }

    private void OnModuleDetachedComplete(Module[] modules)
    {
        RefreshAttachableConnector();
        ModuleSelector.instance.Refresh();
    }

    private ModuleSelectorPanel GetItemPanelAtPosition(Vector2 position)
    {
        var elements = Utils.GetUIElementsAtPosition(position);
        return elements.Select(element => element.GetComponent<ModuleSelectorPanel>())
            .FirstOrDefault(itemPanel => itemPanel != null);
    }

    private Connector FindProperConnector(Vector2 position)
    {
        return attachableConnectors
            .Select(x => new
            {
                connector = x,
                dist = Vector2.Distance(position, x.transform.position)
            })
            .Where(x => x.dist < 1.0f)
            .OrderBy(x => x.dist)
            .Select(x => x.connector)
            .FirstOrDefault();
    }

    public bool CheckModuleCollided(Module module)
    {
        var hits = new List<Collider2D>();
        module.collider.OverlapCollider(new ContactFilter2D(), hits);
        return hits.Count > 0;
    }

    private void RefreshAttachableConnector()
    {
        attachableConnectors.Clear();
        var connectors = player.spacecraft.GetComponentsInChildren<Connector>();
        foreach (Connector connector in connectors)
        {
            if (connector.dest == null)
            {
                attachableConnectors.Add(connector);
            }
        }
    }
}
