using UnityEngine;

public class WorkBenchStorageScript : MonoBehaviour
{
    public IngredientData ingredient;
    private Inventory inventory;
    public int amount;

    void Start()
    {
        inventory = Inventory.msInstance;
    }

    public void CheckIngredient()
    {
        amount = inventory.GetAmount(ingredient);
    }
}
