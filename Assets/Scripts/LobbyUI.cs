using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    private UnityEngine.UI.Button startButton;
    private UnityEngine.UI.Button settingButton;
    private UnityEngine.UI.Button quitButton;

    private void Awake()
    {
        startButton = transform.Find("StartButton").GetComponent<UnityEngine.UI.Button>();
        settingButton = transform.Find("SettingButton").GetComponent<UnityEngine.UI.Button>();
        quitButton = transform.Find("QuitButton").GetComponent<UnityEngine.UI.Button>();
    }

    private void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        settingButton.onClick.AddListener(OnSettingButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
    }

    private void OnSettingButtonClicked()
    {
        SceneManager.LoadSceneAsync("Setting", LoadSceneMode.Additive);
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
