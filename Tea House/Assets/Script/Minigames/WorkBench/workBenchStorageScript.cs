using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
