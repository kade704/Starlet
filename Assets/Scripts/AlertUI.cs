using UnityEngine;
using UnityEngine.UI;

public class AlertUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button okButton;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Show(string title, string description, System.Action onCancel = null, System.Action onOk = null)
    {
        titleText.text = title;
        descriptionText.text = description;

        cancelButton.onClick.RemoveAllListeners();
        okButton.onClick.RemoveAllListeners();

        if (onCancel != null)
            cancelButton.onClick.AddListener(() => { onCancel.Invoke(); Hide(); });

        if (onOk != null)
            okButton.onClick.AddListener(() => { onOk.Invoke(); Hide(); });

        Utils.ShowCanvasGroup(canvasGroup);
    }

    public void Hide()
    {
        Utils.HideCanvasGroup(canvasGroup);
    }
}