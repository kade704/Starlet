using UnityEngine;
using Sirenix.OdinInspector;

public class ItemAsset : ScriptableObject
{
    [HorizontalGroup("Split", Width = 100), HideLabel, PreviewField(100), Required] 
    public Sprite icon;

    [VerticalGroup("Split/Right"), Required] 
    public string title;

    [VerticalGroup("Split/Right"), Required] 
    public bool canBuy;

    [VerticalGroup("Split/Right"), EnableIf("canBuy")] 
    public int buyPrice;

    [VerticalGroup("Split/Right"), Required] 
    public bool canSell;

    [VerticalGroup("Split/Right"), EnableIf("canSell")] 
    public int sellPrice;

    public bool canCraft;
    [EnableIf("canCraft")]
    public CraftItem[] craftItems;
}

[System.Serializable]
public struct CraftItem
{
    public ItemAsset item;
    public int amount;
}