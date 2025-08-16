using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json.Linq;

public class ControllerManager : MonoBehaviour, ISerializable, IDeserializable
{
    [HideInInspector] public bool editing = false;
    [HideInInspector] public bool playing = false;
    [HideInInspector] public CanvasGroup canvasGroup;
    [HideInInspector] public Controller selectedController;
    [HideInInspector] public Controller draggedController;
    [HideInInspector] public List<Controller> controllers = new List<Controller>();

    [HideInInspector] public UnityEvent<Controller> onControllerSelected = new UnityEvent<Controller>();
    [HideInInspector] public UnityEvent<Controller> onControllerDeselected = new UnityEvent<Controller>();
    [HideInInspector] public UnityEvent<Controller> onControllerDragBegin = new UnityEvent<Controller>();
    [HideInInspector] public UnityEvent<Controller> onControllerDragEnd = new UnityEvent<Controller>();
    [HideInInspector] public UnityEvent<Controller> onControllerDragging = new UnityEvent<Controller>();
    [HideInInspector] public UnityEvent onEditEnable = new UnityEvent();
    [HideInInspector] public UnityEvent onEditDisable = new UnityEvent();

    private Vector2 dragOffset;
    private bool controllerDragging;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        InputManager.instance.onTapped.AddListener(OnTapped);
        InputManager.instance.onDragBegin.AddListener(OnDragBegin);
        InputManager.instance.onDragEnd.AddListener(OnDragEnd);
        InputManager.instance.onDragging.AddListener(OnDragging);
    }

    private void Start()
    {
        var hangarManager = GameManager.Instance.GetSystem<HangarManager>();
        Deserialize(hangarManager.CurrentHangarData.controllersData);
    }

    private void OnTapped(Vector2 position)
    {
        if (!editing) return;

        var controllerTapped = GetControllerAtPosition(position);
        if (controllerTapped != null)
        {
            SelectController(controllerTapped);
        }
        else
        {
            if (!Utils.ExistUIElementAtPosition(position))
            {
                DeselectController();
            }
        }
    }

    private void OnDragBegin(Vector2 position)
    {
        if (!editing) return;

        var controllerPanel = GetControllerPanelAtPosition(position);
        if (controllerPanel != null)
        {
            draggedController = InstantiateController(controllerPanel.asset);
            dragOffset = Vector2.zero;
            draggedController.onDragBegin.Invoke();
            onControllerDragBegin.Invoke(draggedController);
            SelectController(draggedController);
            controllerDragging = true;
            return;
        }

        var controller = GetControllerAtPosition(position);
        if (controller != null && controller == selectedController)
        {
            draggedController = controller;
            dragOffset = (Vector2)controller.rect.position - position;
            draggedController.onDragBegin.Invoke();
            onControllerDragBegin.Invoke(draggedController);
            controllerDragging = true;
            return;
        }
    }

    private void OnDragging(Vector2 position)
    {
        if (!editing) return;
        if (!controllerDragging) return;

        var x = position.x + dragOffset.x;
        var y = position.y + dragOffset.y;
        var width = Screen.width;
        var height = Screen.height;

        var resolutionFactor = new Vector2(width / 1920, height / 1080);

        var size = draggedController.rect.sizeDelta * resolutionFactor;

        x = Mathf.Clamp(x, size.x / 2, width - size.x / 2);
        y = Mathf.Clamp(y, size.y / 2, height - size.y / 2 - 200);

        draggedController.rect.position = new Vector2(x, y);

        draggedController.onDragged.Invoke();
        onControllerDragging.Invoke(draggedController);
    }

    private void OnDragEnd(Vector2 position)
    {
        if (!editing) return;
        if (!controllerDragging) return;

        draggedController.onDragEnd.Invoke();
        onControllerDragEnd.Invoke(draggedController);
        controllerDragging = false;
    }

    private Controller GetControllerAtPosition(Vector2 position)
    {
        var elements = Utils.GetUIElementsAtPosition(position);
        return elements.Select(x => x.GetComponent<Controller>())
            .FirstOrDefault(controller => controller != null);
    }

    private ControllerSelectorPanel GetControllerPanelAtPosition(Vector2 position)
    {
        var elements = Utils.GetUIElementsAtPosition(position);
        return elements.Select(x => x.GetComponent<ControllerSelectorPanel>())
            .FirstOrDefault(controller => controller != null);
    }

    public void SelectController(Controller controller)
    {
        if (selectedController != null && selectedController != controller)
        {
            DeselectController();
        }
        selectedController = controller;
        selectedController.onSelected.Invoke();
        onControllerSelected.Invoke(controller);
    }

    public void DeselectController()
    {
        if (selectedController == null) return;

        onControllerDeselected.Invoke(selectedController);
        selectedController.onDeselected.Invoke();
        selectedController = null;
    }

    public Controller InstantiateController(ControllerAsset asset)
    {
        var controller = Instantiate(asset.prefab, transform);
        controller.asset = asset;
        controllers.Add(controller);
        return controller;
    }

    public void DestroyController(Controller controller)
    {
        controllers.Remove(controller);
        Destroy(controller.gameObject); 
    }

    public void ClearControllers()
    {
        DeselectController();

        foreach (var controller in controllers)
        {
            Destroy(controller.gameObject);
        }
        controllers.Clear();
    }

    public JToken Serialize()
    {
        var controllersData = new JArray();
        foreach (var controller in controllers)
        {
            var rect = controller.GetComponent<RectTransform>();
            var properties = PropertyFunc.ConvertToDatas(controller.properties);
            controllersData.Add(new JObject
            {
                ["name"] = controller.asset.name,
                ["x"] = rect.anchoredPosition.x,
                ["y"] = rect.anchoredPosition.y,
                ["properties"] = properties
            });
        }
        return controllersData;
    }

    public void Deserialize(JToken token)
    {
        GameResource gameResource = GameManager.Instance.GetSystem<GameResource>();

        ClearControllers();
        foreach (var item in token.Children<JObject>())
        {
            var name = item["name"].ToString();
            var x = (float)item["x"];
            var y = (float)item["y"];
            var properties = item["properties"];

            var assets = gameResource.GameResourceAsset.controllerAssets;
            var asset = assets.Where(x => x.name == name).FirstOrDefault();
            if (asset == null)
            {
                Debug.LogError($"Deserializing Controllers Error. Controller ID [{name}] Not Found");
                continue;
            }

            var controller = InstantiateController(asset);
            var rect = controller.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(x, y);
            PropertyFunc.OverwriteFromDatas(controller.properties, (JArray)properties);
        }
    }
}
