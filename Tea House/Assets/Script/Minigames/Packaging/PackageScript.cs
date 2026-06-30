using System.Collections.Generic;
using UnityEngine;

public class PackageScript : MonoBehaviour
{
    public IngredientData mIngredient; // Total of stats
    public Mail mTargetMail;

    public void AddStat(ItemData item)
    {
        if(mIngredient == null)
            mIngredient = ScriptableObject.CreateInstance<IngredientData>();

        for(int i = 0; i < mIngredient.mStats.Count; i++)
            mIngredient.mStats[i].mValue += item.mStats[i].mValue;
    }
    public void AddStat(List<ItemData> items)
    {
        foreach(ItemData item in items)
            AddStat(item);
    }
}
