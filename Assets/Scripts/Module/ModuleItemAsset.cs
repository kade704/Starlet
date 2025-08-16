using UnityEngine;

public enum ModuleType { None, Block, Machinery, Energy, Resource }

[CreateAssetMenu(fileName = "Module Item Asset", menuName = "Custom Asset/Module Item Asset")]
public class ModuleItemAsset : ItemAsset
{
    public Module prefab;
    public ModuleType type;
}
