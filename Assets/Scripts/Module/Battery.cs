using UnityEngine;

[RequireComponent(typeof(Module))]
public class Battery : MonoBehaviour
{
    public BatteryProperty batteryProperty;
    public ModuleSprite energySprite;
    private Module module;

    private void Awake()
    {
        batteryProperty.energy = batteryProperty.capacity;

        module = GetComponent<Module>();
        module.RegisterProperty(batteryProperty);

        module.onAttached.AddListener(OnAttached);
        module.onDetached.AddListener(OnDetached);
    }

    private void Update()
    {
        if (!module.IsRunning()) return;

        if (energySprite != null)
        { 
            energySprite.fillAmount = batteryProperty.energy / batteryProperty.capacity;
        }
    }

    private void OnAttached(Spacecraft spacecraft)
    {
        spacecraft.AddBattery(this);
    }

    private void OnDetached(Spacecraft spacecraft)
    {
        spacecraft.RemoveBattery(this);
    }
}
