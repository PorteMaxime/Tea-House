using UnityEngine;

[CreateAssetMenu(fileName = "IngredientItem", menuName = "Scriptable Objects/IngredientItem")]
public class IngredientItem : ItemData
{
    public GameObject mIngredientGrab;

    public GameObject mAmountJar1;
    public GameObject mAmountJar2;
    public GameObject mAmountJar3;

    public GameObject mFragIngredient;

    public TransformType mType;

    public enum TransformType
    {
        none,
        bruising,
        cutting
    }
}
