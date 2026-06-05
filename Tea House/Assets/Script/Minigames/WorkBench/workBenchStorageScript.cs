using UnityEngine;

public class WorkBenchStorageScript : MonoBehaviour
{
    public IngredientData ingredient;
    private Inventory inventory;
    public int amount;

    void Start()
    {
        inventory = Inventory.msInstance;

        Inventory.mOnInventoryCreated += OnInventoryCreated;
    }

    private void OnInventoryCreated(Inventory inv)
    {
        inventory = inv;

        inventory.AddItem(ingredient, 5);
        CheckIngredient();

        Inventory.mOnInventoryCreated -= OnInventoryCreated;
    }

    public void CheckIngredient()
    {
        amount = inventory.mItems[ingredient];
    }
}
