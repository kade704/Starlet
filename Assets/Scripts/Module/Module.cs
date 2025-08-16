using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Health))]
public class Module : MonoBehaviour
{
    public Vector2 offset = Vector2.zero;
    public Vector2 size = Vector2.one;
    public float mass = 1;
    [Required] public ModuleSprite mainSprite;
    public bool hasCooltime = false;
    [EnableIf("hasCooltime")] public float cooltime = 0;

    [HideInInspector] public ModuleItemAsset itemAsset;
    [HideInInspector] public List<Property> properties = new List<Property>();
    [HideInInspector] public List<Connector> connectors = new List<Connector>();
    [HideInInspector] public new Rigidbody2D rigidbody;
    [HideInInspector] public new Collider2D collider;
    [HideInInspector] public Health health;
    [HideInInspector] public Spacecraft spacecraft;
    [HideInInspector] public Connector owner;
    [HideInInspector] public bool attached;

    [HideInInspector] public UnityEvent<Spacecraft> onAttached = new UnityEvent<Spacecraft>();
    [HideInInspector] public UnityEvent<Spacecraft> onDetached = new UnityEvent<Spacecraft>();
    [HideInInspector] public UnityEvent onPlayerDragBegin = new UnityEvent();
    [HideInInspector] public UnityEvent onPlayerDragEnd = new UnityEvent();
    [HideInInspector] public UnityEvent onPlayerDragged = new UnityEvent();
    [HideInInspector] public UnityEvent onPlayerSelected = new UnityEvent();
    [HideInInspector] public UnityEvent onPlayerDeselected = new UnityEvent();

    private BrokenItem brokenItemPrefab;
    
    private float remainTime;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        health = GetComponent<Health>();

        var connectorContainer = transform.Find("Connectors");
        if (connectorContainer != null)
        {
            foreach (Transform connector in connectorContainer)
            {
                connectors.Add(connector.GetComponent<Connector>());
            }
        }

        brokenItemPrefab = Resources.Load<BrokenItem>("Prefabs/BrokenItem");
    }

    private void Start()
    {
        onAttached.AddListener(OnAttached);
        onDetached.AddListener(OnDetached);
        onPlayerDragBegin.AddListener(OnPlayerDragBegin);

        health.onDamaged.AddListener(OnDamaged);
        health.onDeath.AddListener(OnDeath);
    }

    private void Update()
    {
        if (!IsRunning()) return;

        if (hasCooltime)
        {
            if (remainTime > 0) remainTime -= Time.deltaTime;
            else remainTime = 0;

            mainSprite.fillAmount = 1 - remainTime / cooltime;
        }
    }

    private void FixedUpdate()
    {
        if (!attached && rigidbody != null)
        {
            rigidbody.velocity = Vector2.Lerp(rigidbody.velocity, Vector2.zero, Time.fixedDeltaTime * 0.5f);
            rigidbody.angularVelocity = Mathf.LerpAngle(rigidbody.angularVelocity, 0f, Time.fixedDeltaTime * 0.5f);
        }
    }

    private void OnAttached(Spacecraft spacecraft)
    {
        Destroy(rigidbody);
    }

    private void OnDetached(Spacecraft spacecraft)
    {
        rigidbody = gameObject.AddComponent<Rigidbody2D>();
        rigidbody.mass = mass;
        rigidbody.AddForce(transform.up * 10.0f);
    }

    private void OnPlayerDragBegin()
    {
        rigidbody.velocity = Vector2.zero;
    }

    public bool HasComponent<T>() where T : MonoBehaviour
    {
        return GetComponent<T>() != null;
    }

    public void RegisterProperty(Property property)
    {
        properties.Add(property);
    }

    private void OnDamaged(HealthEventInfo info)
    {
        StopAllCoroutines();
        StartCoroutine(IEnuBlink());
    }

    private IEnumerator IEnuBlink()
    {
        var blinkDuration = 0.5f;

        var startTime = Time.time;
        var endTime = Time.time + blinkDuration;

        while (Time.time < endTime)
        {
            var f = (Time.time - startTime) / blinkDuration;
            f = Mathf.Lerp(0.5f, 1.0f, f);
            var color = Color.white * f; color.a = 1;
            mainSprite.spriteRenderer.color = color;
            yield return null;
        }
        mainSprite.spriteRenderer.color = Color.white;
    }

    private void OnDeath(HealthEventInfo info)
    {
        if (attached) spacecraft.DetachModule(this);

        if (itemAsset.canCraft)
        {
            foreach (var craftItem in itemAsset.craftItems)
            {
                var amount = Random.Range(0, craftItem.amount + 1);
                for (var i = 0; i < amount; i++)
                {
                    var brokenItemBase = Instantiate(brokenItemPrefab, transform.position, transform.rotation);
                    brokenItemBase.item = craftItem.item;
                }
            }
        }
        
        Destroy(gameObject);
    }

    public void ResetCoolTime()
    {
        remainTime = cooltime;
    }

    public bool IsCoolTimeWaiting()
    {
        return remainTime > 0;
    }

    public bool IsRunning()
    {
        return attached && spacecraft.running;
    }

    public bool IsBase()
    {
        return itemAsset.name == "Base";
    }
}
