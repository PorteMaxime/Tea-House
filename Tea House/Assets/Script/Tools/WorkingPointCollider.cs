using UnityEngine;

public class WorkingPointCollider : MonoBehaviour
{
    public GameObject mCollider = null;

    private void OnTriggerEnter(Collider other)
    {
        mCollider = other.gameObject;
    }
}
