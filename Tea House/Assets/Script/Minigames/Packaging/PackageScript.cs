using System.Collections.Generic;
using UnityEngine;

public class PackageScript : MonoBehaviour
{
    public IngredientData mIngredient; // Total of stats
    public Mail mTargetMail;

    public void AddStat(ItemData item)
    {
        for(int i = 0; i < mIngredient.mStats.Count; i++)
            mIngredient.mStats[i].mValue += item.mStats[i].mValue;
    }
    public void AddStat(List<ItemData> items)
    {
        for(int i = 0; i < mIngredient.mStats.Count; i++)
        {
            for(int j = 0; j < items.Count; j++)
                mIngredient.mStats[i].mValue += items[j].mStats[i].mValue;
        }
    }
}
