using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Objects/Inventory")]
public class Inventory : ScriptableObject
{
    //public IngredientData GreenTea;
    //public IngredientData BlackTea;

    //public IngredientData Mint;
    //public IngredientData Chamomille;
    //public IngredientData Cinnamon;
    //public IngredientData DriedBerries;
    //public IngredientData DriedOrange;
    //public IngredientData Ginger;
    //public IngredientData Jasmine;
    //public IngredientData Lavender;

    public Dictionary<ItemData, int> mIngredients = new Dictionary<ItemData, int>();

    public void Clear()
    {
        //mIngredients.Add(GreenTea, 0);
        //mIngredients.Add(BlackTea, 0);

        //mIngredients.Add(Mint, 5);
        //mIngredients.Add(Chamomille, 0);
        //mIngredients.Add(Cinnamon, 0);
        //mIngredients.Add(DriedBerries, 0);
        //mIngredients.Add(DriedOrange, 0);
        //mIngredients.Add(Ginger, 0);
        //mIngredients.Add(Jasmine, 0);
        //mIngredients.Add(Lavender, 0);
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
