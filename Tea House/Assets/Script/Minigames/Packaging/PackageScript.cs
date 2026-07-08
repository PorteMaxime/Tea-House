using System.Collections.Generic;
using UnityEngine;

public class PackageScript : MonoBehaviour
{
    public IngredientData mIngredient = null; // Total of stats
    public Mail mTargetMail = null;

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

    public int GetCompletionScore()
    {
        // TODO: Add scoring system
        return 0;
    }
}
