using UnityEngine;

public class Connector : MonoBehaviour
{
    [HideInInspector] public Module origin;
    [HideInInspector] public Module dest;

    private SpriteRenderer spriteRenderer;
    private bool blinking;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        origin = GetComponentInParent<Module>();
    }

    private void Start()
    {
        if (ModuleEditor.instance)
        {
            ModuleEditor.instance.onModuleDragBegin.AddListener(OnModuleDraggedBegin);
            ModuleEditor.instance.onModuleDragEnd.AddListener(OnModuleDraggedEnd);
        }

        spriteRenderer.color = Color.clear;
    }

    private void Update()
    {
        if (!blinking) return;
        
        var a = Mathf.Sin(Time.time * 6) * .4f + .6f;
        spriteRenderer.color = new Color(1, 1, 1, a);
    }

    private void OnModuleDraggedBegin(Module module)
    {
        if (origin.spacecraft != null && origin.spacecraft.IsPlayer() && dest == null)
        {
            blinking = true;
        }
    }

    private void OnModuleDraggedEnd(Module module)
    {
        blinking = false;
        spriteRenderer.color = Color.clear;
    }
}