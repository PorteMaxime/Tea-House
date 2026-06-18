using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

[CreateAssetMenu(fileName = "Game", menuName = "Scriptable Objects/ingredientData")]
public class IngredientData : ItemData
{
    public bool mIsBase;
    public GameObject mFragIngredient;

    public List<StatValue> GetTorrefiedStats()
    {
        List<StatValue> torrefiedStats = new List<StatValue>();

        for (int i = 0; i < mStats.Count; i++)
        {
            torrefiedStats.Add(mStats[i]);

            if (mStats[i].mValue > 0 && mStats[i].mStat != ItemData.Stat.Prestige)
            {
                torrefiedStats[i].mValue += 1;
            }
        }

        return torrefiedStats;
    }
}
