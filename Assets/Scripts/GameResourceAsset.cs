using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "Game Resources", menuName = "Custom Asset/Game Resources")]
public class GameResourceAsset : SerializedScriptableObject
{

    [AssetSelector] public ModuleItemAsset[] moduleItems;
    [AssetSelector] public IngredientItemAsset[] ingredientItems;
    [AssetSelector] public ControllerAsset[] controllerAssets;
    [AssetSelector] public StationAsset[] stationAssets;
    [AssetSelector] public TextAsset[] enemyDatas;

    public Dictionary<ModuleType, Sprite> moduleTypeSprites = new Dictionary<ModuleType, Sprite>();
}
