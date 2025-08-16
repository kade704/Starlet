using UnityEngine;
using UnityEngine.UI;

public class ControllerSelectorPanel : MonoBehaviour
{
    public ControllerAsset asset;
    [HideInInspector] public Button button;
    [HideInInspector] public Image icon;
    [HideInInspector] public Text title;

    private void Awake()
    {
        button = GetComponent<Button>();
        icon = transform.Find("Icon").GetComponent<Image>();
        title = transform.Find("Title").GetComponent<Text>();
    }
}
