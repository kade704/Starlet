using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

public class GameState : MonoBehaviour, ISerializable, IDeserializable
{
    private StationAsset station;
    private int coin;

    public StationAsset Station => station;
    public int Coin => coin;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public bool HasCoin(int amount)
    {
        return coin >= amount;
    }


    public void AddCoin(int amount)
    {
        coin += amount;
    }

    public void RemoveCoin(int amount)
    {
        coin -= amount;
        if (coin < 0) coin = 0; 
    }

    public void SetStation(StationAsset newStation)
    {
        station = newStation;
    }

    public JToken Serialize()
    {
        var token = new JObject
        {
            ["station"] = station.name,
            ["coin"] = coin,
        };
        return token;
    }

    public void Deserialize(JToken token)
    {
        var gameResource = GameManager.Instance.GetSystem<GameResource>();
        if (gameResource == null)
        {
            Debug.LogError("GameResource system not found.");
            return;
        }

        var newStation = gameResource.GameResourceAsset.stationAssets.Where(x => x.name == token["station"].ToString()).FirstOrDefault();
        if (newStation == null)
        {
            Debug.LogError($"Station [{token["station"]}] not found.");
            return;
        }

        station = newStation;
        coin = (int)token["coin"];
    }
}
