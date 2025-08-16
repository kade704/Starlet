using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public class StationUI : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text emptyText;
    [SerializeField] private Transform points;
    [SerializeField] private Transform point;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button editButton;
    [SerializeField] private Button departureButton;
    [SerializeField] private Button createButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Spacecraft spacecraft;

    private int selectedIndex = 0;
    private HangarManager hangarManager;
    private AlertUI alertUI;
    
    private void Awake()
    {
        prevButton.onClick.AddListener(OnPrevButtonClicked);
        nextButton.onClick.AddListener(OnNextButtonClicked);
        createButton.onClick.AddListener(OnCreateButtonClicked);
        editButton.onClick.AddListener(OnEditButtonClicked);
        departureButton.onClick.AddListener(OnDepartureButtonClicked);
        deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    void Start()
    {
        hangarManager = GameManager.Instance.GetSystem<HangarManager>();
        alertUI = GameManager.Instance.GetSystem<AlertUI>();

        selectedIndex = hangarManager.CurrentHangarIndex;

        if (hangarManager.HangarCount == 0)
        {
            prevButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
            departureButton.gameObject.SetActive(false);
            deleteButton.gameObject.SetActive(false);
            emptyText.gameObject.SetActive(true);
        }
        else
        {
            titleText.gameObject.SetActive(true);
            emptyText.gameObject.SetActive(false);

            OnSelectedIndexChanged();
        }
    }

    private void OnCreateButtonClicked()
    {
        hangarManager.CurrentHangarIndex = hangarManager.CreateDefaultHangar();
        SceneManager.LoadScene("Editor");
    }

    private void OnEditButtonClicked()
    {
        SceneManager.LoadScene("Editor");
    }

    private void OnDepartureButtonClicked()
    {
        SceneManager.LoadScene("Space");
    }

    private void OnDeleteButtonClicked()
    {
        alertUI.Show("함선 삭제", "정말 삭제하시겠습니까?", OnDeleteCancelButtonClicked, OnDeleteOKButtonClicked);
    }

    private void OnDeleteCancelButtonClicked()
    {
        alertUI.Hide();
    }

    private void OnDeleteOKButtonClicked()
    {
        alertUI.Hide();
        var hangar = hangarManager.GetHangarDataByIndex(selectedIndex).Value;

        var spacecraftData = hangar.spacecraftData;

        // foreach (var moduleData in spacecraftData)
        // {
        //     if (moduleData.name == "Base") continue;
        //     var item = GameManager.Instance.gameResources.GetItemAssets().Where(x => x.name == moduleData.name).FirstOrDefault();
        //     InventoryManager.instance.inventory.AddItem(item, 1);
        // }
        // InventoryManager.instance.SaveInventory();

        hangarManager.DeleteHangar(selectedIndex);


        SceneManager.LoadScene("Station");
    }

    private void OnPrevButtonClicked()
    {
        if (selectedIndex > 0)
        {
            selectedIndex--;
            OnSelectedIndexChanged();
        }
    }

    private void OnNextButtonClicked()
    {
        if (selectedIndex < hangarManager.HangarCount - 1)
        {
            selectedIndex++;
            OnSelectedIndexChanged();
        }
    }

    private void OnBackButtonClicked()
    {
        SceneManager.LoadScene("Main");
    }

    private void OnSelectedIndexChanged()
    {
        foreach (Transform point in points)
            Destroy(point.gameObject);

        for (int i = 0; i < hangarManager.HangarCount; i++)
        {
            var pointImage = Instantiate(point, points).GetComponent<Image>();
            pointImage.color = (i == selectedIndex) ? Color.white : Color.gray;
        }

        hangarManager.CurrentHangarIndex = selectedIndex;
        
        var hangarData = hangarManager.CurrentHangarData;

        titleText.text = hangarData.name;
        spacecraft.Deserialize(hangarData.spacecraftData);

        prevButton.image.color = selectedIndex == 0 ? Color.grey : Color.white;
        nextButton.image.color = selectedIndex == hangarManager.HangarCount - 1 ? Color.grey : Color.white;
    }
}
