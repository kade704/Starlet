using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Controller))]
public class VerticalSlider : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public SignalProperty signalOutput;

    private Controller controller;
    private Image handleImage;
    private Text signalText;
    private float value;

    private ControllerManager controllerManager;
    private Player player;

    private void Awake()
    {
        controller = GetComponent<Controller>();
        controller.RegisterProperty(signalOutput);

        handleImage = transform.Find("HandleImage").GetComponent<Image>();
        signalText = transform.Find("SignalText").GetComponent<Text>();
    }

    private void Start()
    {
        controllerManager = GameManager.Instance.GetSystem<ControllerManager>();
        player = GameManager.Instance.GetSystem<Player>();
    }

    private void Update()
    {
        player.spacecraft.SetSignalValue(signalOutput.value, value > 0 ? value : 0);
        signalText.text = signalOutput.value;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!controllerManager.playing) return;

        ModifyHandlePos(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!controllerManager.playing) return;

        ModifyHandlePos(eventData.position);
    }

    private void ModifyHandlePos(Vector2 position)
    {
        position = transform.InverseTransformPoint(position);
        value = position.y / 280 + .5f;
        value = Mathf.Clamp(value, 0, 1);

        var newPos = -140 + value * 280;

        handleImage.transform.localPosition = new Vector2(0, newPos);
    }
}
