using UnityEngine;

public class TableScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
    }
}
