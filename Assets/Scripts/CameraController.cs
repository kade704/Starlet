using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public Transform target;
    public Vector2 offset;
    public float moveSmooth = 5.0f;
    public float zoomSmooth = 5.0f;
    public float zoomMin = 2.0f;
    public float zoomMax = 10.0f;
    
    [HideInInspector] public float zoom;

    private new Camera camera;
    private bool touch0Began;
    private bool touch1Began;

    private void Awake()
    {
        instance = this;
        camera = GetComponent<Camera>();
    }

    private void Start()
    {
        zoom = camera.orthographicSize;
    }

    private void Update()
    {
        zoom -= Input.mouseScrollDelta.y;

        if (Input.touchSupported)
        {
            switch (Input.touchCount)
            {
                case 0:
                {
                    touch0Began = false;
                    touch1Began = false;
                    break;
                }
                case 1:
                {
                    var touch0 = Input.GetTouch(0);

                    if (touch0.phase == TouchPhase.Began &&
                        !EventSystem.current.IsPointerOverGameObject(touch0.fingerId))
                    {
                        touch0Began = true;
                    }

                    break;
                }
                case 2:
                {
                    var touch0 = Input.GetTouch(0);
                    var touch1 = Input.GetTouch(1);

                    if (touch1.phase == TouchPhase.Began && 
                        !EventSystem.current.IsPointerOverGameObject(touch1.fingerId))
                    {
                        touch1Began = true;
                    }

                    if (touch0Began && touch1Began)
                    {
                        var touch0PrevPos = touch0.position - touch0.deltaPosition;
                        var touch1PrevPos = touch1.position - touch1.deltaPosition;

                        var prevTouchDeltaDist = (touch0PrevPos - touch1PrevPos).magnitude;
                        var touchDeltaDist = (touch0.position - touch1.position).magnitude;

                        var deltaDistDiff = prevTouchDeltaDist - touchDeltaDist;

                        zoom += deltaDistDiff * 0.3f * Time.deltaTime;
                    }
                    break;
                }
            }
        }

        zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
    }

    void FixedUpdate()
    {
        if (!target) return;

        Vector2 targetPos = target.transform.position;
        Vector2 currPos = transform.position;
        Vector2 newPos = Vector2.Lerp(currPos, targetPos, moveSmooth * Time.fixedDeltaTime) + offset;

        transform.position = new Vector3(newPos.x, newPos.y, -10.0f);

        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, zoom, zoomSmooth * Time.fixedDeltaTime);
    }
}