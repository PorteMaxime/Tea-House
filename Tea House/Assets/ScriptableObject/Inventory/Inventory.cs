using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemData> mUnlockedItems = new List<ItemData>();
    [SerializeField] private int mItemStartingAmount = 5;

    [SerializeField] public Dictionary<ItemData, int> mItems = new Dictionary<ItemData, int>();

    public int mMoneyAmount;

    #region Singleton
    public static Inventory msInstance;
    void Awake()
    {
        msInstance = this;

        foreach (var item in mUnlockedItems)
            mItems.Add(item, mItemStartingAmount);
    }
    #endregion

    public void ClearInventory()
    {
        foreach (ItemData key in mItems.Keys)
            mItems[key] = 0;
    }

    public void AddItem(ItemData _item, int _amount)
    {
        if (mItems.ContainsKey(_item))
            mItems[_item] += _amount;
        else
            mItems[_item] = _amount;
    }

    public bool RemoveItem(ItemData _item, int _amount)
    {
        if (mItems.ContainsKey(_item))
        {
            if (mItems[_item] < _amount)
                return false;

            mItems[_item] -= _amount;
            return true;
        }
        return false;
    }    
}
