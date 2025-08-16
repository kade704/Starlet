using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Module))]
public class Engine : MonoBehaviour
{
    public float energyMaxUse = 20.0f;
    public float increaseAcceleration = 80.0f;
    public float decreaseAcceleration = 80.0f;
    public float particleSizeMultiplier = 1;
    public float particleRateMultiplier = 1;
    public Color particleColor = Color.yellow;
    public float maxForce = 50.0f;
    [ReadOnly] public float force;
    [HideInInspector] public float targetForce;

    public SignalProperty signalInput;

    private ParticleSystem particle;
    private Transform forceTarget;
    private float emissionRate;
    private Module module;

    private void Awake()
    {
        module = GetComponent<Module>();
        module.RegisterProperty(signalInput);
        module.onAttached.AddListener(OnAttached);
        module.onDetached.AddListener(OnDetached);

        this.particle = transform.Find("Emission").GetComponent<ParticleSystem>();
        forceTarget = transform.Find("Force").transform;

        emissionRate = this.particle.emission.rateOverTimeMultiplier * particleRateMultiplier;
        var particle = this.particle.main;
        particle.startSizeMultiplier *= particleSizeMultiplier;
    }

    private void LateUpdate()
    {
        if (!module.IsRunning()) return;

        targetForce = 0;

        var value = module.spacecraft.GetSignalValue(signalInput.value);

        var energy = value * energyMaxUse * Time.deltaTime;

        var enoughEnergy = module.spacecraft.HasEnergy(energy);
        if(enoughEnergy)
        {
            module.spacecraft.UseEnergy(energy);
            targetForce = value * maxForce;
        }

        var acceleration = force < targetForce ? increaseAcceleration : decreaseAcceleration;
        force = Mathf.MoveTowards(force, targetForce, Time.deltaTime * acceleration);

        var spacecraftRigidbody = module.spacecraft.rigidbody;
        var position = forceTarget.position;
        var direction = -forceTarget.up;
        spacecraftRigidbody.AddForceAtPosition(direction * force * Time.deltaTime, position);

        var emission = particle.emission;
        emission.rateOverTimeMultiplier = (force / maxForce) * emissionRate;

        if (force > 0.05 && !particle.isPlaying)
        {
            particle.Play();
        }

        if (force < 0.05 && particle.isPlaying)
        {
            particle.Stop();
        }
    }

    private void OnAttached(Spacecraft spacecraft)
    {
        particle.gameObject.SetActive(true);
    }

    private void OnDetached(Spacecraft spacecraft)
    {
        particle.gameObject.SetActive(false);
    }
}
