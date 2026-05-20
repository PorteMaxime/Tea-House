using UnityEngine;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class WorkBenchScript : MonoBehaviour
{
    Packaging_MainScript packaging;

    public IngredientData currentItem = null;
    public IngredientData mTotalIngredient;
    private void Awake()
    {
        mTotalIngredient = ScriptableObject.CreateInstance<IngredientData>();
    }
    public void SelectItem()
    {

    }
    public void AddIngredient()
    {
        if (currentItem == null)
        {
            Debug.LogWarning("Player.currentItem == null when using !");
        }

        mTotalIngredient.mWarmth += currentItem.mWarmth;
        mTotalIngredient.mFreshness += currentItem.mFreshness;
        mTotalIngredient.mEnergy += currentItem.mEnergy;
        mTotalIngredient.mRelaxation += currentItem.mRelaxation;
        mTotalIngredient.mPrestige += currentItem.mPrestige;

        currentItem = null;
    }
    public void Clear()
    {
        mTotalIngredient.mWarmth = 0;
        mTotalIngredient.mFreshness = 0;
        mTotalIngredient.mEnergy = 0;
        mTotalIngredient.mRelaxation = 0;
        mTotalIngredient.mPrestige = 0;
    }
    public void SetIngredientData()
    {
        packaging.SetIngredientData(mTotalIngredient);
    }
}

class Packaging_MainScript {

    public void SetIngredientData(IngredientData ingredientData) { }
};