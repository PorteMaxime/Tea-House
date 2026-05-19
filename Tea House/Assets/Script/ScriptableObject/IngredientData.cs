using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Game", menuName = "Game/ingredientData")]
public class IngredientData : ScriptableObject
{
    public string mName;
    public GameObject mModel;
    public int mWarmth;
    public int mFreshness;
    public int mRelaxation;
    public int mEnergy;
    public int mPrestige;

    public bool mIsBase;

    public List<IngredientData> mIngredients;

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
