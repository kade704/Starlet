using System.Collections;
using UnityEngine;

public class BrokenItem : MonoBehaviour
{
    public ItemAsset item;

    private new Rigidbody2D rigidbody;
    private new Collider2D collider;
    private SpriteRenderer spriteRenderer;
    private Storage transferTarget;
    private float transferAccel = 20.0f;
    private Player player;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        player = GameManager.Instance.GetSystem<Player>();
        InputManager.instance.onDragging.AddListener(OnDragging);

        var x = Random.Range(-1f, 1f);
        var y = Random.Range(-1f, 1f);
        rigidbody.velocity = new Vector2(x, y).normalized;

        var hue = Random.Range(0f, 1);
        var color = Color.HSVToRGB(hue, 1f, 1f);
        spriteRenderer.color = color;
    }

    private void FixedUpdate()
    {
        if (transferTarget != null)
        {
            var dist = Vector2.Distance(transform.position, transferTarget.transform.position);
            if (dist < 0.1f)
            {
                Destroy(gameObject);
            }

            rigidbody.position = Vector2.MoveTowards(transform.position, transferTarget.transform.position, transferAccel * Time.fixedDeltaTime);

            transferAccel -= 5.0f * Time.fixedDeltaTime;
            transferAccel = Mathf.Clamp(transferAccel, 5.0f, 20.0f);
        }
        else
        {
            rigidbody.velocity = Vector2.MoveTowards(rigidbody.velocity, Vector2.zero, Time.fixedDeltaTime * 0.4f);
        }
    }


    private void OnDragging(Vector2 screenPos)
    {
        if (transferTarget != null) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        if (Vector2.Distance(worldPos, rigidbody.position) < 1f)
        {
            var storage = player.spacecraft.StoreItem(item);
            if (storage != null)
            {
                collider.isTrigger = true;
                transferTarget = storage;
            }
        }
    }
}
