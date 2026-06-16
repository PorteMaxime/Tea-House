using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParentingXRAY : MonoBehaviour
{
    [SerializeField] private GameObject[] mChilds;
    private bool mIsShown = false;

    private GameObject mCamera = null;

    public void Start()
    {
        mCamera = GameObject.Find("CameraOrigin/Main Camera").gameObject;
        mIsShown = GetComponent<MeshRenderer>().enabled;
    }

    public void Update()
    {
        Ray ray = new Ray(mCamera.transform.position, new Vector3(0, 0, 0) - mCamera.transform.position);
        Camera_Raycast.Raycast(ray, 2f, Mathf.Infinity, LayerMask.GetMask("Wall"), (string[])null, out List<RaycastHit> hits);

        if (hits.Any(hit => hit.collider.gameObject == transform.gameObject) == true)
            Show(false);
        else
            Show(true);
    }

    private void Show(bool toShow)
    {
        foreach(GameObject child in mChilds)
            Show(toShow, child);
        
        Show(toShow, transform.gameObject);
        
        mIsShown = toShow;
    }

    private void Show(bool toShow, GameObject target)
    {
        MeshRenderer meshR = target.GetComponent<MeshRenderer>();
        if(meshR != null) meshR.enabled = toShow;

        MeshRenderer[] meshRenderers = target.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer mesh in meshRenderers)
            mesh.enabled = toShow;
    }
}
