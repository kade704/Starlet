using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    [HideInInspector] public Vector2 touchPos;
    [HideInInspector] public UnityEvent<Vector2> onTapped = new UnityEvent<Vector2>();
    [HideInInspector] public UnityEvent<Vector2> onDragBegin = new UnityEvent<Vector2>();
    [HideInInspector] public UnityEvent<Vector2> onDragging = new UnityEvent<Vector2>();
    [HideInInspector] public UnityEvent<Vector2> onDragEnd = new UnityEvent<Vector2>();
    
    private Vector2 touchStartPos;
    private float touchStartTime;
    private bool dragState;
    private bool tapState;

    private void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        Vector2 touchPos;
        bool touchBegan;
        bool touchEnd;
        if (Input.touchSupported && Input.touchCount > 0)
        {
            touchPos = Input.GetTouch(0).position;
            touchBegan = Input.GetTouch(0).phase == TouchPhase.Began;
            touchEnd = Input.GetTouch(0).phase == TouchPhase.Ended;
        }
        else
        {
            touchPos = Input.mousePosition;
            touchBegan = Input.GetMouseButtonDown(0);
            touchEnd = Input.GetMouseButtonUp(0);
        }

        if (touchBegan)
        {
            tapState = true;
            touchStartPos = touchPos;
            touchStartTime = Time.time;
        }

        if (touchEnd)
        {
            if(tapState)
            {
                onTapped.Invoke(touchPos);
            }
            else
            {
                onDragEnd.Invoke(touchPos);
                touchStartPos = Vector2.zero;
                touchStartTime = 0;
            }
            tapState = false;
            dragState = false;
        }

        if(tapState)
        {
            if (Vector2.Distance(touchPos, touchStartPos) > 10 ||
               (Time.time - touchStartTime) > .2f)
            {
                tapState = false;
                dragState = true;
                onDragBegin.Invoke(touchStartPos);
            }
        }

        if (dragState)
        {
            onDragging.Invoke(touchPos);
        }
    }
}
