using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[System.Serializable]
public class Inventory
{
    [HideInInspector] public UnityEvent<InventoryEventInfo> OnItemAdded = new UnityEvent<InventoryEventInfo>();
    [HideInInspector] public UnityEvent<InventoryEventInfo> OnItemRemoved = new UnityEvent<InventoryEventInfo>();
    [HideInInspector] public UnityEvent<InventoryEventInfo> OnItemChanged = new UnityEvent<InventoryEventInfo>();
    [ReadOnly] public int totalAmount = 0;
    public int capacity = int.MaxValue;

    private readonly Dictionary<ItemAsset, int> itemAmounts = new();

    public IReadOnlyDictionary<ItemAsset, int> ItemAmounts => itemAmounts;

    public void AddItem(ItemAsset item, int amount)
    {
        if (itemAmounts.ContainsKey(item))
        {
            itemAmounts[item] += amount;
        }
        else
        {
            itemAmounts.Add(item, amount);
        }
        totalAmount += amount;

        if (totalAmount > capacity)
        {
            Debug.LogError("Inventory capacity exceed");
        }

        var info = new InventoryEventInfo
        {
            item = item,
            amount = amount
        };

        OnItemAdded.Invoke(info);
        OnItemChanged.Invoke(info);
    }

    public void RemoveItem(ItemAsset item, int amount)
    {
        if (itemAmounts.ContainsKey(item))
        {
            itemAmounts[item] -= amount;
            if (itemAmounts[item] == 0)
            {
                itemAmounts.Remove(item);
            }
            else if (itemAmounts[item] < 0)
            {
                Debug.LogError("Amount parameter must be less or equal to inventory item");
            }
        }
        else
        {
            Debug.LogError($"There is no item [{item.title}] in inventory");
        }
        totalAmount -= amount;

        var info = new InventoryEventInfo
        {
            item = item,
            amount = amount
        };

        OnItemRemoved.Invoke(info);
        OnItemChanged.Invoke(info);
    }

    public int GetItemAmount(ItemAsset item)
    {
        if (itemAmounts.ContainsKey(item))
        {
            return itemAmounts[item];
        }
        else
        {
            return 0;
        }
    }

    public bool HasItemAmount(ItemAsset item, int amount)
    {
        if (itemAmounts.ContainsKey(item))
        {
            if (itemAmounts[item] >= amount)
            {
                return true;
            }
        }
        return false;
    }

    public int GetRemainingAmount()
    {
        return capacity - totalAmount;
    }

    public ModuleItemAsset[] GetOwnedModuleItems()
    {
        return itemAmounts
            .Select(x => x.Key)
            .Where(x => x is ModuleItemAsset)
            .Select(x => x as ModuleItemAsset)
            .ToArray();
    }

    public IngredientItemAsset[] GetOwnedIngredientItems()
    {
        return itemAmounts
            .Select(x => x.Key)
            .Where(x => x is IngredientItemAsset)
            .Select(x => x as IngredientItemAsset)
            .ToArray();
    }

    public ItemAsset[] GetOwnedItems()
    {
        return itemAmounts.Keys.ToArray();
    }

    public void ClearItems()
    {
        itemAmounts.Clear();
    }
}

public struct InventoryEventInfo
{
    public ItemAsset item;
    public int amount;
}
