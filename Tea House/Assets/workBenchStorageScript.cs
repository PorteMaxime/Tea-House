using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorkBenchStorageScript : MonoBehaviour
{
    public IngredientData ingredient;
    public CameraController mCameraController;
    public Inventory inventory;
    public int amount;

    private WorkBenchScript workBenchScript;

    void Awake()
    {
        workBenchScript = transform.parent.GetComponent<WorkBenchScript>();
    }
    void Start()
    {
        inventory.AddItem(ingredient, 5);
        CheckIngredient();
    }
    public void CheckIngredient()
    {
        amount = inventory.mIngredients[ingredient];
    }
    private void OnMouseDown()
    {
        Debug.Log("TestMouseEnter");
    }
    void Update()
    {
        
    }
}
