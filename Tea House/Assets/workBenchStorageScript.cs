using UnityEngine;
using UnityEngine.InputSystem;

public class workBenchStorageScript : MonoBehaviour
{
    public IngredientData ingredient;
    public int amount;

    public Inventory inventory;
    public WorkBenchScript workBenchScript;

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
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Objet cliqué : " + hit.collider.name);

                if(amount > 0)
                {
                    workBenchScript.currentItem = ingredient;
                    amount--;
                }
            }
        }
    }
}
