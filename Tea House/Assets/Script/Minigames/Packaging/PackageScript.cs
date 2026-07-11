using System.Collections.Generic;
using UnityEngine;

public class PackageScript : MonoBehaviour
{
    public List<ItemData> mIngredients = null; // Ingredients in mixture
    public ItemData mTotal;
    public Mail mTargetMail = null;

    private bool mIsTotaled = false;

    void Awake()
    {
        mTotal = ScriptableObject.CreateInstance<ItemData>();
    }

    public void AddIngredient(ItemData item)
    {
        mIngredients.Add(item);
        AddStat(item);
    }

    public void AddIngredient(List<ItemData> items)
    {
        foreach(ItemData item in items)
            mIngredients.Add(item);
        
        AddStat(items);
    }

    private void AddStat(ItemData item)
    {
        if(mTotal == null)
            mTotal = ScriptableObject.CreateInstance<IngredientData>();

        for(int i = 0; i < mTotal.mStats.Count; i++)
            mTotal.mStats[i].mValue += item.mStats[i].mValue;
    }
    private void AddStat(List<ItemData> items)
    {
        foreach(ItemData item in items)
            AddStat(item);
    }

    public void MakeTotal()
    {
        if(mIsTotaled == true) return;
        
        AddStat(GetComponent<Associated_SO>().mItem);
        mIsTotaled = true;
    }
}
