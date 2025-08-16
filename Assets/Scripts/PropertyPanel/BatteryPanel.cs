using UnityEngine;
using UnityEngine.UI;

public class BatteryPanel : MonoBehaviour
{
    [HideInInspector] public BatteryProperty batteryProperty;

    private Text energyText;
    private Image energyBarImage;

    private void Awake()
    {
        energyText = transform.Find("Text").GetComponent<Text>();
        energyBarImage = transform.Find("Bar").GetComponent<Image>();
    }

    private void Update()
    {
        energyBarImage.fillAmount = batteryProperty.energy / batteryProperty.capacity;
        energyText.text =  batteryProperty.energy.ToString("F0") +  "/" + batteryProperty.capacity + "J";
    }
}
