using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public enum SceneState
{
    None = -1,
    Lobby,
    Station,
    Editor,
    Space,
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    Debug.LogError("GameManager instance not found in the scene.");
                }
            }
            return instance;
        }
    }

    private readonly Dictionary<string, MonoBehaviour> systems = new();

    public T GetSystem<T>() where T : MonoBehaviour
    {
        var systemType = typeof(T).Name;
        if (systems.ContainsKey(systemType))
        {
            if (!systems[systemType])
            {
                systems[systemType] = FindObjectOfType<T>();
            }
            return systems[systemType] as T;
        }
        else
        {
            var system = FindObjectOfType<T>();
            systems.Add(systemType, system);
            return system;
        }
    }

    public SceneState GetSceneState()
    {
        var name = SceneManager.GetActiveScene().name;
        switch (name)
        {
            case "Lobby": return SceneState.Lobby;
            case "Station": return SceneState.Station;
            case "Editor": return SceneState.Editor;
            case "Space": return SceneState.Space;
        }
        return SceneState.None;
    }

    public bool CheckSceneState(SceneState state)
    {
        return GetSceneState() == state;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    public void SaveGame()
    {
        var savePath = Application.persistentDataPath + $"/SaveData.json";
        var root = new JObject
        {
            ["gameState"] = GetSystem<GameState>().Serialize(),
            ["inventory"] = GetSystem<InventoryManager>().Serialize(),
            ["hangarManager"] = GetSystem<HangarManager>().Serialize()
        };

        var json = JsonConvert.SerializeObject(root, Formatting.Indented);
        File.WriteAllText(savePath, json);
        Debug.Log($"Game saved to {savePath}");
    }

    public void LoadGame()
    {
        var savePath = Application.persistentDataPath + $"/SaveData.json";
        if (!File.Exists(savePath))
        {
            Debug.LogError("Save file not found.");
            return;
        }

        var json = File.ReadAllText(savePath);
        var root = JObject.Parse(json);

        GetSystem<GameState>().Deserialize(root["gameState"]);
        GetSystem<InventoryManager>().Deserialize(root["inventory"]);
        GetSystem<HangarManager>().Deserialize(root["hangarManager"]);
    }

    public void ResetGame()
    {
        var savePath = Application.persistentDataPath + $"/SaveData.json";
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Game reset successfully.");
        }
        else
        {
            Debug.LogWarning("No save file found to reset.");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public bool CheckSaveFile()
    {
        var savePath = Application.persistentDataPath + $"/SaveData.json";
        return File.Exists(savePath);
    }

    public void SaveStarterData()
    {
        var savePath = $"{Application.persistentDataPath}/SaveData.json";
        File.WriteAllText(savePath, StarterSaveFile.StarterSaveFileData);
        Debug.Log($"Save Starter Data to {savePath}");
    }
}
