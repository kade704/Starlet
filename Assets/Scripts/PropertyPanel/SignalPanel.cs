using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SignalPanel : MonoBehaviour
{
	[HideInInspector] public SignalProperty signalProperty;

	private new RectTransform transform;
	private GameObject selector;
	private Button chooseButton;
	private Text nameText;
	private Text valueText;

	private void Awake()
    {
		transform = GetComponent<RectTransform>();
		selector = transform.Find("Selector").gameObject;
        chooseButton = transform.Find("ChooseButton").GetComponent<UnityEngine.UI.Button>();
		nameText = transform.Find("NameText").GetComponent<Text>();
		valueText = transform.Find("ChooseButton/Text").GetComponent<Text>();

		var signalHolder = transform.Find("Selector/Viewport/Content");
		foreach(Transform signal in signalHolder)
        {
			var button = signal.GetComponent<UnityEngine.UI.Button>();
			var text = signal.GetChild(0).GetComponent<Text>();
			button.onClick.AddListener(() => OnPickSignal(text.text));
        }
	}

	private void Start()
    {
		chooseButton.onClick.AddListener(OnChooseButtonClicked);
		nameText.text = signalProperty.name;
	}

	private void Update()
    {
		valueText.text = signalProperty.value;
	}

	private void OnChooseButtonClicked()
    {
		transform.sizeDelta = new Vector2(transform.sizeDelta.x, 192);
		selector.SetActive(true);
	}

	private void OnPickSignal(string signal)
    {
		signalProperty.value = signal;
		transform.sizeDelta = new Vector2(transform.sizeDelta.x, 100);
		selector.SetActive(false);
    }
}
