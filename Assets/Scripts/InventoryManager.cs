using UnityEngine;
using System.Linq;
using Newtonsoft.Json.Linq;

public class InventoryManager : MonoBehaviour, ISerializable, IDeserializable
{
    private Inventory inventory = new();

    public Inventory Inventory => inventory;

    private GameResource gameResource;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        gameResource = GameManager.Instance.GetSystem<GameResource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            gameResource.GetItemAssets().ToList().ForEach(x => inventory.AddItem(x, Random.Range(1, 10)));
        }
    }

    public JToken Serialize()
    {
        var items = new JArray();
        foreach (var item in inventory.ItemAmounts)
        {
            var itemData = new JObject
            {
                ["name"] = item.Key.name,
                ["amount"] = item.Value
            };
            items.Add(itemData);
        }
        return items;
    }

    public void Deserialize(JToken token)
    {
        foreach (var itemData in token.Children<JObject>())
        {
            var itemName = itemData["name"].ToString();
            var itemAmount = itemData["amount"].ToObject<int>();

            var item = gameResource.GetItemAssets().Where(x => x.name == itemName).FirstOrDefault();
            if (item == null)
            {
                Debug.LogError($"Item {itemName} not found in game resources.");
            }
            inventory.AddItem(item, itemAmount);
        }
    }
}