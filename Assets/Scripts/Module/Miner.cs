using UnityEngine;

[RequireComponent(typeof(Module))]
public class Miner : MonoBehaviour
{
    public SignalProperty signalInput;
    public float energyMaxUse;
    public float laserDamage;
    public float laserDist;

    private Module module;
    private LineRenderer laserA;
    private LineRenderer laserB;
    private ParticleSystem particle;

    private void Awake()
    {
        module = GetComponent<Module>();
        module.RegisterProperty(signalInput);

        laserA = transform.Find("LaserA").GetComponent<LineRenderer>();
        laserB = transform.Find("LaserB").GetComponent<LineRenderer>();
        particle = transform.Find("Particle").GetComponent<ParticleSystem>();
    }

    private void LateUpdate()
    {
        if (!module.IsRunning()) return;

        var value = module.spacecraft.GetSignalValue(signalInput.value);

        var energy = energyMaxUse * value * Time.deltaTime;
        var enoughEnergy = module.spacecraft.HasEnergy(energy);

        var blocked = false;
        if (enoughEnergy && energy > 0)
        {
            module.spacecraft.UseEnergy(energy);

            var layerMask = LayerMask.GetMask("Ore");
            var hit = Physics2D.Raycast(transform.position, transform.up, laserDist, layerMask);
            blocked = hit.collider != null;

            var end = (Vector2)transform.position + (Vector2)transform.up * laserDist;

            if (blocked)
            {
                particle.transform.position = hit.point;
                particle.transform.up = hit.normal;

                end = hit.point;

                var ore = hit.collider.GetComponent<Ore>();
                if (ore != null)
                {
                    var damage = laserDamage * value * Time.deltaTime;

                    ore.health.ApplyDamage(damage, gameObject);
                }
            }

            var laserAEnd = laserA.transform.InverseTransformPoint(end);
            var laserBEnd = laserB.transform.InverseTransformPoint(end);

            laserA.SetPosition(1, laserAEnd);
            laserB.SetPosition(1, laserBEnd);
        }

        var emission = particle.emission;
        emission.enabled = blocked;
        laserA.enabled = value > 0 && enoughEnergy;
        laserB.enabled = value > 0 && enoughEnergy;
    }
}
