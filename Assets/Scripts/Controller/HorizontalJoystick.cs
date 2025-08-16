using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Controller))]
public class HorizontalJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Controller controller;
    private Image handleImage;
    private Text signalTextA;
    private Text signalTextB;
    private float value;

    private ControllerManager controllerManager;
    private Player player;

    public SignalProperty signalOutputA;
    public SignalProperty signalOutputB;

    private void Awake()
    {
        controller = GetComponent<Controller>();
        controller.RegisterProperty(signalOutputA);
        controller.RegisterProperty(signalOutputB);

        handleImage = transform.Find("HandleImage").GetComponent<Image>();
        signalTextA = transform.Find("SignalTextA").GetComponent<Text>();
        signalTextB = transform.Find("SignalTextB").GetComponent<Text>();
    }

    private void Start()
    {
        controllerManager = GameManager.Instance.GetSystem<ControllerManager>();
        player = GameManager.Instance.GetSystem<Player>();
    }

    private void Update()
    {
        player.spacecraft.SetSignalValue(signalOutputA.value, value < 0 ? -value : 0);
        player.spacecraft.SetSignalValue(signalOutputB.value, value > 0 ? value : 0);

        signalTextA.text = signalOutputA.value;
        signalTextB.text = signalOutputB.value;
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

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!controllerManager.playing) return;

        handleImage.transform.localPosition = Vector2.zero;
        value = 0;
    }

    private void ModifyHandlePos(Vector2 position)
    {
        position = transform.InverseTransformPoint(position);
        value = position.x / 140;
        value = Mathf.Clamp(value, -1, 1);

        var newPos = value * 140;

        handleImage.transform.localPosition = new Vector2(newPos, 0);
    }
}
