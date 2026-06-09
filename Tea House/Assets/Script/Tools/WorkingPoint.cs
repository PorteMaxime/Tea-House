using System.Collections.Generic;
using UnityEngine;

public class WorkingPoint : MonoBehaviour
{
    private List<GameObject> mCollider = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        mCollider.Add(other.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        mCollider.Remove(other.gameObject);
    }

    public bool IsColliding(GameObject other)
    {
        return mCollider.Contains(other);
    }
}
