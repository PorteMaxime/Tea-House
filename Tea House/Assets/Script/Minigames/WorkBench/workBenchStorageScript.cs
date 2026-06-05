using UnityEngine;

public class WorkBenchStorageScript : MonoBehaviour
{
    public IngredientData ingredient;
    private Inventory inventory;
    public int amount;

    void Start()
    {
        inventory = Inventory.msInstance;

        inventory.AddItem(ingredient, 5);
        CheckIngredient();
    }
    public void CheckIngredient()
    {
        amount = inventory.mItems[ingredient];
    }
}
