using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Stars : MonoBehaviour
{
    [Min(1)] public int layerCount;
    public float scale = 3;
    public Color backgroundColor;

    private new SpriteRenderer renderer;
    
    private readonly int cameraPosID = Shader.PropertyToID("_CameraPos");
    private readonly int backColorID = Shader.PropertyToID("_BackColor");
    private readonly int scaleID = Shader.PropertyToID("_Scale");
    private readonly int layerID = Shader.PropertyToID("_Layer");

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }


    private void Update()
    {
        var ratio = Screen.width / Screen.height;

        var position = Camera.main.transform.position; position.z = 0;
        var size = CameraController.instance.zoomMax;
        size *= ratio * 4.1f;

        transform.position = position;
        transform.localScale = new Vector3(size, size, 1);

        renderer.sharedMaterial.SetVector(cameraPosID, Camera.main.transform.position);
        renderer.sharedMaterial.SetColor(backColorID, backgroundColor);
        renderer.sharedMaterial.SetFloat(scaleID, scale);
        renderer.sharedMaterial.SetInt(layerID, layerCount);
    }
}
