using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerInspector : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Text titleText;
    private Button destroyButton;
    private SignalPanel signalPanelPrefab;
    private List<GameObject> propertyPanels = new List<GameObject>();

    private ControllerManager controllerManager;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        titleText = transform.Find("Title/Text").GetComponent<Text>();
        destroyButton = transform.Find("Destroy/Button").GetComponent<Button>();
    }



    private void Start()
    {
        controllerManager = GameManager.Instance.GetSystem<ControllerManager>();

        controllerManager.onControllerSelected.AddListener(OnControllerSelected);
        controllerManager.onControllerDeselected.AddListener(OnControllerDeselected);

        controllerManager.onEditEnable.AddListener(() => OnControllerDeselected(null));
        controllerManager.onEditDisable.AddListener(() => OnControllerDeselected(null));

        destroyButton.onClick.AddListener(OnDestroyButtonClicked);

        signalPanelPrefab = Resources.Load<SignalPanel>("Prefabs/PropertyPanels/SignalPanel");
    }

    private void OnControllerSelected(Controller controller)
    {
        Utils.ShowCanvasGroup(canvasGroup);

        titleText.text = controller.asset.title;

        foreach(var property in controller.properties)
        {
            if (property is SignalProperty signalProperty)
            {
                var signalPicker = Instantiate(signalPanelPrefab, transform);
                signalPicker.signalProperty = signalProperty;
                propertyPanels.Add(signalPicker.gameObject);
            }
        }
    }

    private void OnControllerDeselected(Controller controller)
    {
        Utils.HideCanvasGroup(canvasGroup);

        foreach (var propertyBox in propertyPanels)
        {
            Destroy(propertyBox);
        }
        propertyPanels.Clear();
    }

    private void OnDestroyButtonClicked()
    {
        var temp = controllerManager.selectedController;
        controllerManager.DeselectController();
        controllerManager.DestroyController(temp);
    }
}
