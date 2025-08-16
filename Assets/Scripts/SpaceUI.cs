using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class SpaceUI : MonoBehaviour
{
    [SerializeField] private Button enterButton;
    [SerializeField] private Text enterButtonText;
    [SerializeField] private Transform stationPointerParent;
    [SerializeField] private StationPointer stationPointerPrefab;

    private Station nearestStation;

    private Player player;
    private GameState gameState;
    private ControllerManager controllerManager;

    private void Awake()
    {
        enterButton.onClick.AddListener(OnEnterButtonClicked);
    }

    private void Start()
    {
        player = GameManager.Instance.GetSystem<Player>();
        gameState = GameManager.Instance.GetSystem<GameState>();
        controllerManager = GameManager.Instance.GetSystem<ControllerManager>();

        controllerManager.playing = true;
        player.canSelectModule = true;

        foreach (var station in FindObjectsOfType<Station>())
        {
            var stationPointer = Instantiate(stationPointerPrefab, stationPointerParent);
            stationPointer.station = station;
        }

        player.spacecraft.onDestroyed.AddListener(() =>
        {
            GameManager.Instance.GetSystem<GameoverUI>().ShowGameover();
        });
    }

    private void Update()
    {
        UpdateNearestStation();
    }

    private void OnEnterButtonClicked()
    {
        gameState.SetStation(nearestStation.stationAsset);
        player.AddItemsFromStorages();
        SceneManager.LoadScene("Main");
    }

    private void UpdateNearestStation()
    {
        if (player == null || player.spacecraft == null)
        {
            return;
        }

        var stations = FindObjectsOfType<Station>();

        nearestStation = stations.Where(x => GetPlayerStationDist(x) <= 20.0f).FirstOrDefault();
        if (nearestStation != null)
        {
            enterButtonText.text = nearestStation.stationAsset.title;

            if (!enterButton.gameObject.activeSelf)
            {
                enterButton.gameObject.SetActive(true);
            }
        }
        else
        {
            if (enterButton.gameObject.activeSelf)
            {
                enterButton.gameObject.SetActive(false);
            }
        }
    }

    private float GetPlayerStationDist(Station station)
    {
        return Vector2.Distance(station.transform.position, player.transform.position);
    }
}
