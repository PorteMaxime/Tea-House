using System.Collections.Generic;
using UnityEngine;

public class CameraXRAY : MonoBehaviour
{
    [SerializeField] private string[] mLayerNameToXRAY;
    private Camera mCamera;

    [SerializeField] private GameObject mTarget;
    public float mRaycastThickness = 1f;

    private List<GameObject> mLastCollided = new List<GameObject>();
    private List<GameObject> mCollided = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     Vector3 pos = mCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));

    //     Vector3 dir = mTarget.transform.position - pos;
    //     float dist = Vector3.Distance(pos, dir);
    //     dir.Normalize();


    //     if (dist > 0f)
    //     {
    //         int layers = LayerMask.GetMask(mLayerNameToXRAY);
    //         RaycastHit[] hits = Physics.SphereCastAll(pos, mRaycastThickness, dir, dist, layers);

    //         foreach (RaycastHit hit in hits)
    //         {
    //             if (hit.collider.GetComponent<Camera>() == null)
    //             {
    //                 MeshRenderer mesh = GetRender(hit.collider.gameObject);
    //                 if (mesh != null)
    //                     mesh.enabled = false;
    //                 ParentingXRAY parentingXRAY = GetParentingXRAY(hit.collider.gameObject);
    //                 if (parentingXRAY != null)
    //                     parentingXRAY.Show(false);

    //                 mCollided.Add(hit.collider.gameObject);
    //             }
    //         }
    //         UpdateCollided();
    //     }
    // }

    void UpdateCollided()
    {
        List<GameObject> toRemove = new List<GameObject>();
        foreach (GameObject collided in mLastCollided)
        {
            if (mCollided.Contains(collided) == false)
            {
                MeshRenderer mesh = GetRender(collided);
                if (mesh != null)
                    mesh.enabled = true;
                ParentingXRAY parentingXRAY = GetParentingXRAY(collided);
                if (parentingXRAY != null)
                    parentingXRAY.Show(true);

                toRemove.Add(collided);
            }
        }
        foreach (GameObject remove in toRemove)
            mLastCollided.Remove(remove);
        foreach (GameObject collided in mCollided)
        {
            if (mLastCollided.Contains(collided) == false)
                mLastCollided.Add(collided);
        }
        mCollided.Clear();
    }

    MeshRenderer GetRender(GameObject collided)
    {
        MeshRenderer mesh = collided.GetComponent<MeshRenderer>();
        if (mesh != null) return mesh;
        return collided.GetComponentInChildren<MeshRenderer>();
    }
    ParentingXRAY GetParentingXRAY(GameObject collided)
    {
        ParentingXRAY mesh = collided.GetComponent<ParentingXRAY>();
        if (mesh != null) return mesh;
        return collided.GetComponentInChildren<ParentingXRAY>();
    }
}
