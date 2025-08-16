using UnityEngine;
using UnityEngine.UI;

public class FloatPanel : MonoBehaviour
{
    [HideInInspector] public FloatProperty floatProperty;

    private Text nameText;
    private Text valueText;
    private UnityEngine.UI.Slider slider;

    private void Awake()
    {
        nameText = transform.Find("Name").GetComponent<Text>();
        valueText = transform.Find("Value").GetComponent<Text>();
        slider = transform.Find("Slider").GetComponent<UnityEngine.UI.Slider>();
    }

    private void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        slider.minValue = floatProperty.min;
        slider.maxValue = floatProperty.max;
        slider.value = floatProperty.value;
        nameText.text = floatProperty.name + ":";
    }

    private void Update()
    {
        valueText.text = floatProperty.value.ToString("F1");
    }

    private void OnSliderValueChanged(float value)
    {
        floatProperty.value = value;
    }
}
