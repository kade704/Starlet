using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Controller))]
public class PushButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Controller controller;
    private Image releasedImage;
    private Image pressedImage;
    private Text signalText;
    private bool pressed = false;

    private ControllerManager controllerManager;
    private Player player;

    public SignalProperty signalOutput;

    private void Awake()
    {
        controller = GetComponent<Controller>();
        controller.RegisterProperty(signalOutput);

        releasedImage = transform.Find("ReleasedImage").GetComponent<Image>();
        pressedImage = transform.Find("PressedImage").GetComponent<Image>();
        signalText = transform.Find("SignalText").GetComponent<Text>();
    }

    private void Start()
    {
        controllerManager = GameManager.Instance.GetSystem<ControllerManager>();
        player = GameManager.Instance.GetSystem<Player>();
    }

    private void Update()
    {
        player.spacecraft.SetSignalValue(signalOutput.value, pressed ? 1 : 0);
        signalText.text = signalOutput.value;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!controllerManager.playing) return;
        pressed = true;
        releasedImage.gameObject.SetActive(false);
        pressedImage.gameObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!controllerManager.playing) return;
        pressed = false;
        releasedImage.gameObject.SetActive(true);
        pressedImage.gameObject.SetActive(false);
    }
}
