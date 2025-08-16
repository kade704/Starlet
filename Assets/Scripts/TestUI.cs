using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestUI : MonoBehaviour
{
    private void Awake()
    {
        var backButton = transform.Find("BackButton").GetComponent<Button>();
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void Start()
    {
        GameManager.Instance.GetSystem<ControllerManager>().playing = true;
        GameManager.Instance.GetSystem<Player>().canSelectModule = true;
    }

    private void OnBackButtonClicked()
    {
        SceneManager.LoadScene("Editor");
    }
}
