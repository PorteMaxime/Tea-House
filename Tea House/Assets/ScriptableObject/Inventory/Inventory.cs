using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemData> mStartingItems = new List<ItemData>();
    [SerializeField] private int mStartingItemsAmount = 5;

    // Items du joueur
    private Dictionary<ItemData, int> mItems = new Dictionary<ItemData, int>();

    [SerializeField] public int mMoneyAmount;

    [SerializeField] public bool isTorrefactionUnlocked = true;

    #region Singleton

    public static Inventory msInstance;
    void Awake()
    {
        msInstance = this;
    }
    #endregion

    private void Start()
    {
        foreach (var item in mStartingItems)
            mItems.Add(item, mStartingItemsAmount);
    }

    public void ClearInventory()
    {
        foreach (ItemData key in mItems.Keys)
            mItems[key] = 0;
    }

    public void ChangeItemAmount(ItemData item, int amount)
    {
        mItems[item] += amount;
    }

    public void UnlockItem(ItemData item)
    {
        item.mIsUnlocked = true;
    }

    public List<Type> GetItemsOfType<Type>() where Type : ItemData
    {
        List<Type> list = new List<Type>();

        foreach (var item in mItems)
        {
            if (item.Key.mIsUnlocked == true && item.Key is Type)
                list.Add((Type)item.Key);
        }

        return list;
    }

    public int GetAmount(ItemData item)
    {
        return mItems[item];
    }
}
