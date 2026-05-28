using UnityEngine;

public class WorkingPoint : MonoBehaviour
{
    public GameObject mCollider = null;

    private void OnTriggerEnter(Collider other)
    {
        mCollider = other.gameObject;
    }
    private void OnTriggerExit(Collider other)
    {
        mCollider = null;
    }
}
