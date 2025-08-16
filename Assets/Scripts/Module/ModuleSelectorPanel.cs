using UnityEngine;
using UnityEngine.UI;

public class ModuleSelectorPanel : MonoBehaviour
{
    [HideInInspector] public ModuleItemAsset itemAsset;
    [HideInInspector] public CanvasGroup canvasGroup;
    [HideInInspector] public Image icon;
    [HideInInspector] public Image type;
    [HideInInspector] public Text title;
    [HideInInspector] public Text amount;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        icon = transform.Find("Icon").GetComponent<Image>();
        type = transform.Find("Type").GetComponent<Image>();
        title = transform.Find("Title").GetComponent<Text>();
        amount = transform.Find("Amount").GetComponent<Text>();
    }
}
