using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private InputField cheatInput;

    private AlertUI alertUI;

    private void Start()
    {
        alertUI = GameManager.Instance.GetSystem<AlertUI>();

        newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        loadGameButton.onClick.AddListener(OnLoadGameButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
        resetButton.onClick.AddListener(OnResetButtonClicked);
        cheatInput.onEndEdit.AddListener(OnCheatInputSubmitted);

        if (GameManager.Instance.CheckSaveFile())
        {
            newGameButton.interactable = false;
            loadGameButton.interactable = true;
            resetButton.interactable = true;
        }
        else
        {
            newGameButton.interactable = true;
            loadGameButton.interactable = false;
            resetButton.interactable = false;
        }
    }

    private void OnNewGameButtonClicked()
    {
        GameManager.Instance.SaveStarterData();
        GameManager.Instance.LoadGame();
        SceneManager.LoadScene("Main");
    }

    private void OnLoadGameButtonClicked()
    {
        GameManager.Instance.LoadGame();
        SceneManager.LoadScene("Main");
    }

    private void OnExitButtonClicked()
    {
        GameManager.Instance.QuitGame();
    }

    private void OnResetCancelButtonClicked()
    {
        alertUI.Hide();
    }

    private void OnResetOKButtonClicked()
    {
        alertUI.Hide();
        GameManager.Instance.ResetGame();
        SceneManager.LoadScene("Menu");
    }

    private void OnResetButtonClicked()
    {
        alertUI.Show("게임 초기화", "정말 초기화하시겠습니까?", OnResetCancelButtonClicked, OnResetOKButtonClicked);
    }

    private void OnCheatInputSubmitted(string cheatCode)
    {
        var args = cheatCode.Split(' ');
        if (args.Length == 0) return;
        if (args[0].Trim().Length == 0) return;

        if (args[0].ToLower() == "give")
        {
            if (args.Length < 2)
            {
                Debug.LogError("치트 오류: 아이템 이름을 입력해주세요.");
                return;
            }

            var itemName = args[1];
            var amount = args.Length > 2 ? int.Parse(args[2]) : 1;

            var itemAsset = GameManager.Instance.GetSystem<GameResource>().GetItemAssets()
                .FirstOrDefault(x => x.name.Equals(itemName));
            if (itemAsset != null)
            {
                GameManager.Instance.GetSystem<InventoryManager>().Inventory.AddItem(itemAsset, amount);
                Debug.Log($"치트 성공: {itemAsset.title} 아이템을 {amount}개 획득했습니다.");
            }
            else
            {
                Debug.LogError($"치트 오류: {itemName} 아이템이 존재하지 않습니다.");
            }
        }
        else if (args[0].ToLower() == "giveall")
        {
            var allItems = GameManager.Instance.GetSystem<GameResource>().GetItemAssets();
            var amount = args.Length > 1 ? int.Parse(args[1]) : 1;
            foreach (var item in allItems)
            {
                GameManager.Instance.GetSystem<InventoryManager>().Inventory.AddItem(item, amount);
            }
            Debug.Log($"치트 성공: 모든 아이템을 {amount}개씩 획득했습니다.");
        }
        else if (args[0].ToLower() == "giveallrandom")
        {
            var allItems = GameManager.Instance.GetSystem<GameResource>().GetItemAssets();
            foreach (var item in allItems)
            {
                var amount = Random.Range(1, 10);
                GameManager.Instance.GetSystem<InventoryManager>().Inventory.AddItem(item, amount);
            }
            Debug.Log($"치트 성공: 모든 아이템을 랜덤하게 획득했습니다.");
        }
        else if (args[0].ToLower() == "addcoin")
        {
            if (args.Length < 2 || !int.TryParse(args[1], out int coinAmount))
            {
                Debug.LogError("치트 오류: 유효한 코인 금액을 입력해주세요.");
                return;
            }

            GameManager.Instance.GetSystem<GameState>().AddCoin(coinAmount);
            Debug.Log($"치트 성공: {coinAmount} 코인을 추가했습니다.");
        }
        else if (args[0].ToLower() == "removecoin")
        {
            if (args.Length < 2 || !int.TryParse(args[1], out int coinAmount))
            {
                Debug.LogError("치트 오류: 유효한 코인 금액을 입력해주세요.");
                return;
            }

            GameManager.Instance.GetSystem<GameState>().RemoveCoin(coinAmount);
            Debug.Log($"치트 성공: {coinAmount} 코인을 제거했습니다.");
        }
        else
        {
            Debug.LogError("치트 오류: 알 수 없는 치트 코드입니다.");
        }

        cheatInput.text = string.Empty;
        cheatInput.ActivateInputField();
    }
}
