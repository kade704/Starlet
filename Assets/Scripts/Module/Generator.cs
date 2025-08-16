using UnityEngine;

[RequireComponent(typeof(Module))]
public class Generator : MonoBehaviour
{
    public float chargeAmount;

    private Module module;

    private void Awake()
    {
        module = GetComponent<Module>();
    }

    private void Update()
    {
        if (!module.IsRunning()) return;

        var cooltime = module.IsCoolTimeWaiting();
        var fullEnergy = module.spacecraft.IsFullEnergy();
        if (!cooltime && !fullEnergy)
        {
            module.spacecraft.GainEnergy(chargeAmount);
            module.ResetCoolTime();
        }
    }
}
