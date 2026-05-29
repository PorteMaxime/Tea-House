using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
