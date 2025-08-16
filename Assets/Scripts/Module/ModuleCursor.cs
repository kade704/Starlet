using UnityEngine;

public class ModuleCursor : MonoBehaviour
{
    [HideInInspector] public Module selectedModule;

    private SpriteRenderer spriteRenderer;
    private float selectTime;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        var player = GameManager.Instance.GetSystem<Player>();
        player.onModuleSelected.AddListener(OnModuleSelected);
        player.onModuleDeselected.AddListener(OnModuleDeselected);
    }

    private void Update()
    {
        if (selectedModule == null) return;

        ApplyTransform();
    }

    private void ApplyTransform()
    {
        var elapsed = Time.time - selectTime;
        var scale = Mathf.Abs(Mathf.Sin(elapsed * 2));
        scale = 1.3f + (1 - scale) * .4f;

        var offset = selectedModule.transform.TransformVector(selectedModule.offset);

        transform.position = selectedModule.transform.position + offset;
        spriteRenderer.transform.rotation = selectedModule.transform.rotation;

        spriteRenderer.size = selectedModule.size * scale;
    }

    private void OnModuleSelected(Module module)
    {
        selectedModule = module;
        spriteRenderer.enabled = true;
        selectTime = Time.time;

        ApplyTransform();
    }

    private void OnModuleDeselected(Module module)
    {
        selectedModule = null;
        spriteRenderer.enabled = false;
    }
}
