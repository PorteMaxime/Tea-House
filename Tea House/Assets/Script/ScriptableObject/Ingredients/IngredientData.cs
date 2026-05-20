using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Game", menuName = "Scriptable Objects/ingredientData")]
public class IngredientData : ItemData
{
    public bool mIsBase;
    
    public GameObject mFragIngredient;
    public IngredientData GetTorrefied()
    {
        IngredientData torrefied = ScriptableObject.CreateInstance<IngredientData>();

        mWarmth +=          1;
        mFreshness +=       0;
        mEnergy +=          0;
        mRelaxation +=     -1;
        mPrestige +=        1;

        return torrefied;
    }
}
