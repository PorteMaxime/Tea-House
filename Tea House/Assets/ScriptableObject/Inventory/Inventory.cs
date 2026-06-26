using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemData> mGameItems = new List<ItemData>();
    [SerializeField] private int mStartingItemsAmount = 5;

    // Items du joueur
    public Dictionary<ItemData, int> mItems = new Dictionary<ItemData, int>();
    public Dictionary<string, ItemData> mItemsLookUp = new Dictionary<string, ItemData>();

    [SerializeField] public int mMoneyAmount;

    [SerializeField] public bool isTorrefactionUnlocked = true;

    #region Singleton

    public static Inventory msInstance;
    void Awake()
    {
        msInstance = this;

        //Jar Start Before
        foreach (var item in mGameItems)
        {
            mItems.Add(item, mStartingItemsAmount);
            mItemsLookUp.Add(item.mName, item);
        }
    }
    #endregion

    private void Start()
    {

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
