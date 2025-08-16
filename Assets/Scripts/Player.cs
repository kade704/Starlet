using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Spacecraft))]
public class Player : MonoBehaviour
{
    [HideInInspector] public Spacecraft spacecraft;
    [HideInInspector] public Module selectedModule;
    [HideInInspector] public bool canSelectModule = false;
    
    [HideInInspector] public UnityEvent<Module> onModuleSelected = new UnityEvent<Module>();
    [HideInInspector] public UnityEvent<Module> onModuleDeselected = new UnityEvent<Module>();

    private InventoryManager inventoryManager;
    private GameState gameState;

    private void Awake()
    {
        spacecraft = GetComponent<Spacecraft>();
    }

    private void Start()
    {
        inventoryManager = GameManager.Instance.GetSystem<InventoryManager>();
        gameState = GameManager.Instance.GetSystem<GameState>();

        InputManager.instance.onTapped.AddListener(OnTapped);
        
        var hangarManager = GameManager.Instance.GetSystem<HangarManager>();
        spacecraft.Deserialize(hangarManager.CurrentHangarData.spacecraftData);

        if (GameManager.Instance.CheckSceneState(SceneState.Space))
        {
            var stationAsset = gameState.Station;
            var stations = FindObjectsOfType<Station>();
            var station = stations.FirstOrDefault(s => s.stationAsset == stationAsset);
            if (station != null)
            {
                var newPos = station.transform.position + Vector3.up * 8.0f;
                transform.position = newPos;
                Camera.main.transform.position = newPos;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            transform.Find("Base(Clone)").GetComponent<Health>().ApplyDamage(1000, null);
        }
    }

    private void OnTapped(Vector2 screenPos)
    {
        if (!canSelectModule) return;
        if (Utils.ExistUIElementAtPosition(screenPos)) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        var module = GetModuleAtPosition(worldPos);
        if (module != null)
        {
            if (selectedModule == module) return;
            if (module.attached && module.spacecraft.IsPlayer())
            {
                SelectModule(module);
            }
            else
            {
                DeselectModule();
            }
        }
        else
        {
            if (selectedModule != null)
            {
                DeselectModule();
            }
        }
    }


    public void SelectModule(Module module)
    {
        if (selectedModule != null && selectedModule != module)
        {
            DeselectModule();
        }
        selectedModule = module;
        selectedModule.onPlayerSelected.Invoke();
        selectedModule.health.onDeath.AddListener((_) => DeselectModule());
        onModuleSelected.Invoke(selectedModule);
    }

    public void DeselectModule()
    {
        if (selectedModule == null) return;

        onModuleDeselected.Invoke(selectedModule);
        selectedModule.onPlayerDeselected.Invoke();
        selectedModule.health.onDeath.RemoveListener((_) => DeselectModule());
        selectedModule = null;
    }
    
    private Module GetModuleAtPosition(Vector2 position)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return null;

        var zoom = CameraController.instance.zoom;
        var zoomMax = CameraController.instance.zoomMax;
        var zoomMin = CameraController.instance.zoomMin;

        var t = (zoom - zoomMin) / (zoomMax - zoomMin);

        var modules = FindObjectsOfType<Module>();
        var hits = new Dictionary<Module, float>();
        foreach (var module in modules)
        {
            var local = module.transform.InverseTransformPoint(position);
            var offset = module.offset;
            var size = module.size * (1 + t * 2);
            var dist = Vector2.Distance(local, offset);
            var insideX = offset.x - size.x / 2 <= local.x && local.x <= offset.x + size.x / 2;
            var insideY = offset.y - size.y / 2 <= local.y && local.y <= offset.y + size.y / 2;            
            if (insideX && insideY) hits.Add(module, dist);
        }

        Module result = null;
        var minDist = float.MaxValue;
        foreach(var hit in hits)
        {
            if(hit.Value < minDist)
            {
                minDist = hit.Value;
                result = hit.Key;
            }
        }

        return result;
    }

    public void AddItemsFromStorages()
    {
        foreach (var storage in spacecraft.storages)
        {
            var items = storage.storageProperty.inventory.GetOwnedItems();
            foreach (var item in items)
            {
                var amount = storage.storageProperty.inventory.GetItemAmount(item);
                inventoryManager.Inventory.AddItem(item, amount);
            }
        }
    }
}
