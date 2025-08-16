using UnityEngine;
using UnityEngine.UI;

public class ControllerCursor : MonoBehaviour
{
    private RectTransform rect;
    private Image image;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();    
        image = GetComponent<Image>();
    }

    private void Start()
    {
        var controllerManager = GameManager.Instance.GetSystem<ControllerManager>();

        controllerManager.onControllerSelected.AddListener(OnControllerSelected);
        controllerManager.onControllerDeselected.AddListener(OnControllerDeselected);
        controllerManager.onControllerDragging.AddListener(OnControllerDragging);
    }

    private void OnControllerSelected(Controller controller)
    {
        rect.position = controller.rect.position;
        rect.sizeDelta = controller.rect.sizeDelta;
        image.enabled = true;
    }

    private void OnControllerDeselected(Controller controller)
    {
        image.enabled = false;
    }

    private void OnControllerDragging(Controller controller)
    {
        rect.position = controller.rect.position;
    }
}
