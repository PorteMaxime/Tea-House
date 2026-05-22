using UnityEngine;
using UnityEngine.InputSystem;

public class workBenchStorageScript : MonoBehaviour
{
    public IngredientData ingredient;
    public int amount;

    public Inventory inventory;
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
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Objet cliqué : " + hit.collider.name);

                workBenchScript.SelectItem(ingredient);
                inventory.RemoveItem(ingredient, 1);
                CheckIngredient();

            }
        }
    }
}
