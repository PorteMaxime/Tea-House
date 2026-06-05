using UnityEngine;

[CreateAssetMenu(fileName = "Game", menuName = "Scriptable Objects/ingredientData")]
public class IngredientData : ItemData
{
    public bool mIsBase;
    public GameObject mFragIngredient;
    public IngredientData GetTorrefied()
    {
        IngredientData torrefied = ScriptableObject.CreateInstance<IngredientData>();

        torrefied.mWarmth +=          1;
        torrefied.mFreshness +=       0;
        torrefied.mEnergy +=          0;
        torrefied.mRelaxation +=     -1;
        torrefied.mPrestige +=        1;

        return torrefied;
    }
}
