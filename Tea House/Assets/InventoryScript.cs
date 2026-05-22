using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    public Inventory inventory;

    public void Start()
    {
        inventory.Clear();
    }
}
