using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

[System.Serializable]
public struct HangarData
{
    public string name;
    public JArray spacecraftData;
    public JArray controllersData;
}

public class HangarManager : MonoBehaviour, ISerializable, IDeserializable
{
    private int currentHangarIndex = 0;
    private List<HangarData> hangarsData = new();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public int CurrentHangarIndex
    {
        get => currentHangarIndex;
        set
        {
            if (value >= 0 && value < hangarsData.Count)
            {
                currentHangarIndex = value;
            }
            else
            {
                Debug.LogWarning($"Invalid hangar index: {value}. Must be between 0 and {hangarsData.Count - 1}.");
            }
        }
    }
    
    public HangarData CurrentHangarData
    {
        get
        {
            return hangarsData.ElementAtOrDefault(currentHangarIndex);
        }
    }

    public int HangarCount => hangarsData.Count;

    public void SetSpacecraftData(int index, JArray data)
    {
        if (index < 0 || index >= hangarsData.Count)
        {
            Debug.LogWarning($"Invalid hangar index: {index}. Must be between 0 and {hangarsData.Count - 1}.");
            return;
        }

        HangarData updatedHangar = hangarsData[index];
        updatedHangar.spacecraftData = data;
        hangarsData[index] = updatedHangar;
    }

    public void SetControllersData(int index, JArray data)
    {
        if (index < 0 || index >= hangarsData.Count)
        {
            Debug.LogWarning($"Invalid hangar index: {index}. Must be between 0 and {hangarsData.Count - 1}.");
            return;
        }

        HangarData updatedHangar = hangarsData[index];
        updatedHangar.controllersData = data;
        hangarsData[index] = updatedHangar;
    }


    public HangarData? GetHangarDataByIndex(int index)
    {
        if (index < 0 || index >= hangarsData.Count)
            return null;

        return hangarsData[index];
    }

    public int CreateDefaultHangar()
    {
        var spacecraftData = new JArray
        {
            new JObject
            {
                ["name"] = "Base",
                ["parent"] = -1,
                ["connector"] = -1,
                ["properties"] = new JArray()
            }
        };

        HangarData newHangar = new HangarData
        {
            name = "이름없는 함선",
            spacecraftData = spacecraftData,
            controllersData = new JArray()
        };
        hangarsData.Add(newHangar);
        return hangarsData.Count - 1;
    }

    public void RenameHangar(int index, string newName)
    {
        if (index < 0 || index >= hangarsData.Count)
        {
            Debug.LogWarning($"Invalid hangar index: {index}. Must be between 0 and {hangarsData.Count - 1}.");
            return;
        }

        HangarData updatedHangar = hangarsData[index];
        updatedHangar.name = newName;
        hangarsData[index] = updatedHangar;
    }

    public void DeleteHangar(int index)
    {
        if (index < 0 || index >= hangarsData.Count)
        {
            Debug.LogWarning($"Invalid hangar index: {index}. Must be between 0 and {hangarsData.Count - 1}.");
            return;
        }

        hangarsData.RemoveAt(index);
        if (currentHangarIndex >= hangarsData.Count)
        {
            currentHangarIndex = hangarsData.Count > 0 ? hangarsData.Count - 1 : 0;
        }
    }

    public void Deserialize(JToken token)
    {
        currentHangarIndex = token["currentHangarIndex"].ToObject<int>();
        hangarsData = token["hangarsData"].ToObject<List<HangarData>>();
    }

    public JToken Serialize()
    {
        return new JObject
        {
            ["currentHangarIndex"] = currentHangarIndex,
            ["hangarsData"] = JArray.FromObject(hangarsData)
        };
    }
}
