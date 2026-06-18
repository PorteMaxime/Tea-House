using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public string mName;

    public ItemData mItemData;
}

public class Inventory : MonoBehaviour
{
    // Items existant dans le jeu
    [SerializeField] private List<Item> mGameItems = new List<Item>();

    [SerializeField] private List<ItemData> mStartingItems = new List<ItemData>();
    [SerializeField] private int mStartingItemsAmount = 5;

    // Items du joueur
    [SerializeField] public Dictionary<ItemData, int> mItems = new Dictionary<ItemData, int>();

    [SerializeField] public int mMoneyAmount;

    [SerializeField] public bool isTorrefactionUnlocked = false;

    #region Singleton
    public static event Action<Inventory> mOnInventoryCreated;

    public static Inventory msInstance;
    void Awake()
    {
        msInstance = this;

        foreach (var item in mStartingItems)
            mItems.Add(item, mStartingItemsAmount);

        mOnInventoryCreated?.Invoke(this);
    }
    #endregion

    public void ClearInventory()
    {
        foreach (ItemData key in mItems.Keys)
            mItems[key] = 0;
    }

    public void AddItem(ItemData item, int amount)
    {
        if (mItems.ContainsKey(item))
            mItems[item] += amount;
        else
            mItems[item] = amount;
    }

    public bool RemoveItem(ItemData item, int amount)
    {
        if (mItems.ContainsKey(item))
        {
            if (mItems[item] < amount)
                return false;

            mItems[item] -= amount;
            return true;
        }
        return false;
    }

    public void UnlockItem(ItemData item)
    {
        if (mItems.ContainsKey(item))
            return;

        AddItem(item, 0);
    }

    public ItemData GetItem(string itemName)
    {
        foreach (var item in mGameItems)
        {
            if (item.mName == itemName)
                return item.mItemData;
        }

        return null;
    }
}
