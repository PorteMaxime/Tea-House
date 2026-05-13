using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class CameraXRAY : MonoBehaviour
{
    [SerializeField] private string[] mLayerNameToXRAY;
    private Camera mCamera;

    [SerializeField] private GameObject mTarget;
    private List<GameObject> mLastCollided = new List<GameObject>();
    private List<GameObject> mCollided = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = mCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));

        Vector3 dir = mTarget.transform.position - pos;
        dir.Normalize();

        float dist = Vector3.Distance(pos, dir);

        if(dist > 0f)
        {
            int layers = LayerMask.GetMask(mLayerNameToXRAY);            
            RaycastHit[] hits = Physics.RaycastAll(pos, dir, dist, layers);

            foreach (RaycastHit hit in hits)
            {
                MeshRenderer mesh = hit.collider.GetComponentInChildren<MeshRenderer>();
                if (mesh != null)
                    mesh.enabled = false;
                mCollided.Add(hit.collider.gameObject);
            }
            UpdateCollided();
        }
    }

    void UpdateCollided()
    {
        List<GameObject> toRemove = new List<GameObject>();
        foreach(GameObject collided in mLastCollided)
        {
            if(mCollided.Contains(collided) == false)
            {
                MeshRenderer mesh = collided.GetComponentInChildren<MeshRenderer>();
                if (mesh != null)
                    mesh.enabled = true;
                toRemove.Add(collided);
            }
        }
        foreach(GameObject remove in toRemove)
            mLastCollided.Remove(remove);
        foreach (GameObject collided in mCollided)
        {
            if (mLastCollided.Contains(collided) == false)
                mLastCollided.Add(collided);
        }
        mCollided.Clear();
    }
}
