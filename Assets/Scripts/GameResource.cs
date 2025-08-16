using UnityEngine;
using System.Collections.Generic;

public class GameResource : MonoBehaviour
{
    [SerializeField] private GameResourceAsset gameResourceAsset;

    public GameResourceAsset GameResourceAsset
    {
        get => gameResourceAsset;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public ItemAsset[] GetItemAssets()
    {
        var temp = new List<ItemAsset>();
        temp.AddRange(gameResourceAsset.moduleItems);
        temp.AddRange(gameResourceAsset.ingredientItems);
        return temp.ToArray();
    }
}
