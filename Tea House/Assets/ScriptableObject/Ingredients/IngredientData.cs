using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game", menuName = "Scriptable Objects/ingredientData")]
public class IngredientData : ItemData
{
    public GameObject mFragIngredient;

    public GameObject mAmountJar1;
    public GameObject mAmountJar2;
    public GameObject mAmountJar3;

    public List<StatValue> GetTorrefiedStats()
    {
        List<StatValue> torrefiedStats = new List<StatValue>();
        StatValue statValue;

        for (int i = 0; i < mStats.Count; i++)
        {
            statValue = new StatValue();
            statValue.mStat = mStats[i].mStat;
            statValue.mValue = mStats[i].mValue;

            torrefiedStats.Add(statValue);

            if (mStats[i].mValue > 0 && mStats[i].mStat != ItemData.Stat.Prestige)
            {
                torrefiedStats[i].mValue += 1;
            }
        }

        return torrefiedStats;
    }
}
