using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Health))]
public class Ore : MonoBehaviour
{
    public ItemAsset contentItem;
    public int maxAmount;
    [ReadOnly] public int amount;
    [HideInInspector] public Health health;

    private SpriteRenderer spriteRenderer;
    private new Rigidbody2D rigidbody;
    private BrokenItem brokenItemPrefab;

    private void Awake()
    {
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();

        amount = maxAmount;
        brokenItemPrefab = Resources.Load<BrokenItem>("Prefabs/BrokenItem");
    }

    private void Start()
    {
        health.onDeath.AddListener(OnDeath);
    }

    public void Initialize(int amount, ItemAsset item, Color color)
    {
        this.amount = amount;
        health.maxHP = amount * 10;
        health.Reset();
        contentItem = item;
        spriteRenderer.color = color;
        transform.localScale = Vector3.one * (1 + amount * 0.3f);
        transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        rigidbody.velocity = Random.insideUnitCircle * Random.Range(0.0f, 1.0f);
        rigidbody.angularVelocity = Random.Range(-10f, 10f);
        rigidbody.mass = 1 + amount * 0.2f;
    }

    private void OnDeath(HealthEventInfo info)
    {
        Destroy(gameObject);

        var brokenAmount = info.causer.GetComponent<Miner>() != null ? amount : amount * 0.3f;
        for (var i = 0; i < brokenAmount; i++)
        {
            var brokenItemBase = Instantiate(brokenItemPrefab, transform.position, transform.rotation);
            brokenItemBase.item = contentItem;
        }
    }
}
