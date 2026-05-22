using UnityEngine;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class WorkBenchScript : MonoBehaviour
{
    private Packaging_MainScript mPackaging;

    private IngredientData currentItem = null;
    private GameObject currentItemG = null;
    private GameObject mMortar;

    private Vector3 posOffset = new Vector3(0f,0.1f,0f);
    private IngredientData mTotalIngredient;
    private void Awake()
    {
        mTotalIngredient = ScriptableObject.CreateInstance<IngredientData>();
        mMortar = transform.Find("SM_Mortar_bowl").gameObject;

    }
    public void SelectItem(IngredientData ingredient)
    {
        currentItem = ingredient;
        currentItemG = ingredient.mModel;

        Instantiate(currentItemG, mMortar.transform.position + posOffset, Quaternion.identity, mMortar.transform);
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
        mPackaging.SetIngredientData(mTotalIngredient);
    }
}