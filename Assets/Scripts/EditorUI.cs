using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public class EditorUI : MonoBehaviour
{
    public static EditorUI instance;

    [SerializeField] private CanvasGroup topPanel;
    [SerializeField] private CanvasGroup moduleSelector;
    [SerializeField] private CanvasGroup controllerSelector;
    [SerializeField] private GameObject controllerBackground;
    [SerializeField] private InputField titleInput;
    [SerializeField] private Button backButton;
    [SerializeField] private Button moduleButton;
    [SerializeField] private Button controllerButton;
    [SerializeField] private Button testButton;

    bool showModuleSelector = false;
    bool showControllerSelector = false;
    private HangarManager hangarManager;
    private Player player;
    private ControllerManager controllerManager;

    private void Awake()
    {
        instance = this;

        titleInput.onSubmit.AddListener(OnTitleSubmit);
        backButton.onClick.AddListener(OnBackButtonClicked);
        moduleButton.onClick.AddListener(OnModuleButtonClicked);
        controllerButton.onClick.AddListener(OnControllerButtonClicked);
        testButton.onClick.AddListener(OnTestButtonClicked);
    }

    private void Start()
    {
        hangarManager = GameManager.Instance.GetSystem<HangarManager>();
        player = GameManager.Instance.GetSystem<Player>();
        controllerManager = GameManager.Instance.GetSystem<ControllerManager>();

        ModuleInspector.instance.editing = true;
        titleInput.text = hangarManager.CurrentHangarData.name;
    }

    private void OnBackButtonClicked()
    {
        if (showModuleSelector)
        {
            Utils.ShowCanvasGroup(topPanel);
            Utils.HideCanvasGroup(moduleSelector);
            Utils.ShowCanvasGroup(controllerManager.canvasGroup);
            CameraController.instance.offset = Vector2.zero;
            player.DeselectModule();
            player.canSelectModule = false;
            showModuleSelector = false;
            hangarManager.SetSpacecraftData(hangarManager.CurrentHangarIndex, (JArray)player.spacecraft.Serialize());
        }
        else if (showControllerSelector)
        {
            Utils.ShowCanvasGroup(topPanel);
            Utils.HideCanvasGroup(controllerSelector);
            controllerManager.DeselectController();
            controllerManager.editing = false;
            controllerBackground.SetActive(false);
            showControllerSelector = false;
            hangarManager.SetControllersData(hangarManager.CurrentHangarIndex, (JArray)controllerManager.Serialize());
        }
        else
        {
            GameManager.Instance.SaveGame();
            SceneManager.LoadScene("Station");
        }
    }

    private void OnTestButtonClicked()
    {
        SceneManager.LoadScene("Test");
    }

    private void OnModuleButtonClicked()
    {
        Utils.HideCanvasGroup(topPanel);
        Utils.ShowCanvasGroup(moduleSelector);
        Utils.HideCanvasGroup(controllerManager.canvasGroup);
        CameraController.instance.offset = new Vector2(0, -0.1f);
        player.canSelectModule = true;
        showModuleSelector = true;
    }

    private void OnControllerButtonClicked()
    {
        Utils.HideCanvasGroup(topPanel);
        Utils.ShowCanvasGroup(controllerSelector);
        controllerManager.editing = true;
        controllerBackground.SetActive(true);
        showControllerSelector = true;
    }

    private void OnTitleSubmit(string newTitle)
    {
        if (newTitle == string.Empty)
        {
            newTitle = "이름없는 함선";
            titleInput.text = newTitle;
        }
        hangarManager.RenameHangar(hangarManager.CurrentHangarIndex, newTitle);
    }
}
