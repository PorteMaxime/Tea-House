using UnityEngine;

public class WorkBenchStorageScript : MonoBehaviour
{
    public IngredientData ingredient;
    public Inventory inventory;
    public int amount;

    void Start()
    {
        inventory.AddItem(ingredient, 5);
        CheckIngredient();
    }
    public void CheckIngredient()
    {
        amount = inventory.mIngredients[ingredient];
    }
}
