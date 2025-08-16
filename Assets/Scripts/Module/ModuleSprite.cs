using System.Collections;
using UnityEngine;

public enum FilledDirection
{
    Right, Left, Up, Down
}

[RequireComponent(typeof(SpriteRenderer))]
public class ModuleSprite : MonoBehaviour
{
    public FilledDirection fillDirection;
    [Range(0, 1)] public float fillAmount = 1;

    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public float startMultiplier;

    private readonly int fillAmountID = Shader.PropertyToID("_FillAmount");
    private readonly int fillDirID = Shader.PropertyToID("_FillDir");

    private Color attachedColor;
    private Color detachedColor;
    private Module module;
    private int order;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        module = GetComponentInParent<Module>();
    }

    private void Start()
    {
        order = spriteRenderer.sortingOrder;

        attachedColor = spriteRenderer.color;
        detachedColor = attachedColor * .5f;
        detachedColor.a = 1;

        spriteRenderer.color = module.attached ? attachedColor : detachedColor;
        module.onAttached.AddListener(OnAttached);
        module.onDetached.AddListener(OnDetached);
        module.onPlayerDragBegin.AddListener(OnPlayerDragBegin);
        module.onPlayerDragEnd.AddListener(OnPlayerDragEnd);
        module.onPlayerDeselected.AddListener(OnPlayerDeselected);
    }

    private void Update()
    {
        spriteRenderer.material.SetFloat(fillAmountID, fillAmount);
        spriteRenderer.material.SetInt(fillDirID, (int)fillDirection);
    }

    private void OnAttached(Spacecraft spacecraft)
    {
        spriteRenderer.color = attachedColor;
    }

    private void OnDetached(Spacecraft spacecraft)
    {
        spriteRenderer.color = detachedColor;
    }

    private void OnPlayerDragBegin()
    {
        spriteRenderer.sortingOrder = order + 100;
    }

    private void OnPlayerDragEnd()
    {
        spriteRenderer.sortingOrder = order;
    }

    private void OnPlayerDeselected()
    {
        spriteRenderer.color = module.attached ? attachedColor : detachedColor;
    }
}
