using Sirenix.OdinInspector;
using UnityEngine;

public enum BulletType { Normal, Explosive, Tracking }

public class Bullet : MonoBehaviour
{

    [ReadOnly] private float damage;
    [ReadOnly] private float speed;
    [ReadOnly] private BulletType bulletType;
    [ReadOnly] private Color color;
    [ReadOnly] private float distance;

    private GameObject fragmentPrefab;
    private ParticleSystem particle;
    private TrailRenderer trail;
    private Vector2 firePoint;
    private float intensity = 1.0f;
    private float startEmission;
    private Color minColor;
    private Color maxColor;

    private void Awake()
    {
        fragmentPrefab = Resources.Load<GameObject>("Prefabs/BulletFragment");
        particle = GetComponentInChildren<ParticleSystem>();
        trail = GetComponentInChildren<TrailRenderer>();
    }

    private void Start()
    {
        firePoint = transform.position;
        startEmission = particle.emission.rateOverTimeMultiplier;

        Color.RGBToHSV(color, out float h, out float s, out float v);
        float minH = (h - 0.1f + 1.0f) % 1.0f;
        float maxH = (h + 0.1f) % 1.0f;

        minColor = Color.HSVToRGB(minH, s, v);
        maxColor = Color.HSVToRGB(maxH, s, v);
    }

    public void Initialize(BulletType bulletType, float damage, float speed, float distance, Color color)
    {
        this.bulletType = bulletType;
        this.damage = damage;
        this.speed = speed;
        this.distance = distance;
        this.color = color;
    }

    private void Update()
    {
        var currPos = (Vector2)transform.position;
        var newPos = (Vector2)transform.position + (Vector2)transform.up * speed * Time.deltaTime;
        var direction = (newPos - currPos).normalized;
        var dist = Vector2.Distance(newPos, currPos);

        int layerMask = LayerMask.GetMask("Module", "Station", "Ore");

        var hit = Physics2D.Raycast(currPos, direction, dist, layerMask);
        if (hit.collider != null)
        {
            var fragment = Instantiate(fragmentPrefab);
            fragment.transform.position = hit.point;
            fragment.transform.up = hit.normal;

            var health = hit.collider.GetComponent<Health>();
            if (health != null)
            {
                health.ApplyDamage(damage * intensity, gameObject);
            }

            Destroy(fragment, 1);
            Destroy(gameObject);
        }
        transform.position = newPos;

        intensity = 1 - Vector2.Distance(firePoint, transform.position) / distance;
        if (intensity < 0) intensity = 0;

        
        trail.startColor = color * intensity;
        var main = particle.main;
        main.startColor = new ParticleSystem.MinMaxGradient(minColor * intensity, maxColor * intensity);

        var emission = particle.emission;
        emission.rateOverTimeMultiplier = intensity * startEmission;

        if (intensity < 0) Destroy(gameObject);
    }
}
