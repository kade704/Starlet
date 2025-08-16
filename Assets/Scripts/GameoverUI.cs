using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameoverUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Button lobbyButton;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        lobbyButton = transform.Find("MainButton").GetComponent<Button>();
        lobbyButton.onClick.AddListener(OnRestartButtonClicked);
    }

    public void ShowGameover()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        StartCoroutine(Fade(canvasGroup, 1, 1.0f, 1.0f));
    }

    private static IEnumerator Fade(CanvasGroup group, float alpha, float duration, float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        var fadeSpeed = Mathf.Abs(group.alpha - alpha) / duration;
        while (!Mathf.Approximately(group.alpha, alpha))
        {
            group.alpha = Mathf.MoveTowards(group.alpha, alpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        group.alpha = alpha;
    }

    private void OnRestartButtonClicked()
    {
        SceneManager.LoadScene("Main");
    }
}
