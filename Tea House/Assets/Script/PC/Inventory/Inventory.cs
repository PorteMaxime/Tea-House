using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Objects/Inventory")]
public class Inventory : ScriptableObject
{
    public Dictionary<ItemData, int> mIngredients = new Dictionary<ItemData, int>();

    private void Awake()
    {
        Clear();
    }

    public void Clear()
    {
        mIngredients.Clear();
    }

    public void AddItem(ItemData _item, int _amount)
    {
        if (mIngredients.ContainsKey(_item))
            mIngredients[_item] += _amount;
        else
            mIngredients[_item] = _amount;
    }

    public bool RemoveItem(ItemData _item, int _amount)
    {
        if (mIngredients.ContainsKey(_item))
        {
            if (mIngredients[_item] < _amount)
                return false;

            mIngredients[_item] -= _amount;
            return true;
        }
        return false;
    }    
}
