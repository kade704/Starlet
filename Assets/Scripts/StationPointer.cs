using UnityEngine;
using UnityEngine.UI;

public class StationPointer : MonoBehaviour
{
    public Station station;
    private CanvasGroup canvasGroup;
    private Text stationText;
    private Text distanceText;
    private Transform arrow;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        stationText = transform.Find("StationText").GetComponent<Text>();
        distanceText = transform.Find("DistanceText").GetComponent<Text>();
        arrow = transform.Find("Arrow");
    }

    void Start()
    {
    }

    private void Update()
    {
        var viewportPos = Camera.main.WorldToViewportPoint(station.transform.position);
        if (viewportPos.x < 1 && viewportPos.x > 0 && viewportPos.y < 1 && viewportPos.y > 0)
        {
            if (Utils.CanvasGroupVisible(canvasGroup))
                Utils.HideCanvasGroup(canvasGroup);
        }
        else
        {
            if (!Utils.CanvasGroupVisible(canvasGroup))
                Utils.ShowCanvasGroup(canvasGroup);

            Vector2 vec = station.transform.position - Camera.main.transform.position;

            var dist = vec.magnitude;

            distanceText.text = $"{Mathf.Floor(dist)}km";
            stationText.text = station.stationAsset.title;

            var dir = vec.normalized;
            var extend = new Vector2(760, 340);
            var x = Mathf.Clamp(extend.y * dir.x / Mathf.Abs(dir.y), -extend.x, extend.x);
            var y = Mathf.Clamp(extend.x * dir.y / Mathf.Abs(dir.x), -extend.y, extend.y);
            transform.localPosition = new Vector2(x, y);

            var scale = Mathf.Clamp(-dist / 100.0f + 1, 0.7f, 1.5f);
            transform.localScale = new Vector3(scale, scale, 1);

            var newRot = Vector3.zero;
            newRot.z = -Vector2.SignedAngle(dir, Vector2.right);
            arrow.localEulerAngles = newRot;
        }
    }
}
