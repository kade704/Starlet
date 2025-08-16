using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainUI : MonoBehaviour
{
    [SerializeField] private Button stationButton;
    [SerializeField] private Button workshopButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button homeButton;

    [SerializeField] private Text stationText;
    [SerializeField] private SpriteRenderer stationSpriteRenderer;

    private void Start()
    {
        stationButton.onClick.AddListener(OnStationButtonClicked);
        workshopButton.onClick.AddListener(OnWorkshopButtonClicked);
        shopButton.onClick.AddListener(OnShopButtonClicked);
        homeButton.onClick.AddListener(OnHomeButtonClicked);
        inventoryButton.onClick.AddListener(OnInventoryButtonClicked);

        var station = GameManager.Instance.GetSystem<GameState>().Station.title;
        stationText.text = $"{station} 정거장";

        stationSpriteRenderer.sprite = GameManager.Instance.GetSystem<GameState>().Station.sprite;
    }

    private void OnStationButtonClicked()
    {
        SceneManager.LoadScene("Station");
    }

    private void OnWorkshopButtonClicked()
    {
        SceneManager.LoadScene("Workshop");
    }

    private void OnShopButtonClicked()
    {
        SceneManager.LoadScene("Shop");
    }

    private void OnHomeButtonClicked()
    {
        SceneManager.LoadScene("Menu");
    }

    private void OnInventoryButtonClicked()
    {
        SceneManager.LoadScene("Inventory");
    }
}
