using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraDragAndDrop : ICameraControllerMod
{
    #region Main
    private CameraController mMain = null;
    public CameraDragAndDrop(CameraController main)
    {
        mMain = main;
        OnInit();
    }
    #endregion

    private Camera_Raycast mRaycast = null;
    private Camera mCamera = null;

    private Vector3 mDraggingOffset = Vector3.zero;
    private Rigidbody mDraggingRB = null;
    private GameObject mDragging = null;
    private bool mDraggedRBWasKinematic = false;
    private float mDraggedOriginX = 0f;
    private float mDraggedOriginDistance = 0f;

    private Vector3 mTargetPos = Vector3.zero;

    #region Basics Methods
    public void OnInit()
    {
        mRaycast = mMain.GetControllerMod<Camera_Raycast>();
        mCamera = mMain.GetComponent<Camera>();
    }

    public void OnUpdate()
    {
        if(IsDragging() == false) return;

        Vector3 mousePos = Mouse.current.position.ReadValue();      // Coordonnées X / Y de la souris sur l'écran
        mousePos.z = mDraggedOriginDistance;                               
        Vector3 worldPos = mCamera.ScreenToWorldPoint(mousePos);

        Vector3 dir = (worldPos - mMain.transform.position).normalized;
        float dist = Vector3.Distance(mMain.transform.position, new Vector3(mDraggedOriginX, worldPos.y, worldPos.z));
        worldPos = mMain.transform.position + dir * dist; 

        Camera_Raycast.Raycast(
            new Ray(mMain.transform.position, dir),
            dist, 
            (LayerMask[])null, 
            (string[])null, 
            out List<RaycastHit> obstacles);

        if(obstacles != null && obstacles.Count > 0 && obstacles[0].collider.gameObject.tag != "Player" && obstacles[0].collider.gameObject != mDragging) 
        {
            float distance = Vector3.Distance(obstacles[0].point, mMain.transform.position);
            worldPos = mMain.transform.position + dir * distance;

            Vector3 normal = obstacles[0].normal;
            Vector3 size = mDragging.GetComponent<Collider>().bounds.extents;
            worldPos += new Vector3(normal.x * size.x, normal.y * size.y, normal.z * size.z);
        }
        
        worldPos = worldPos - mDraggingOffset;
        mDragging.transform.position = new Vector3(mDraggedOriginX, worldPos.y, worldPos.z);
    }

    public void OnFixedUpdate()
    {
    }
    #endregion

    public bool StartDragging(GameObject target)
    {
        if(target == null) return false;
        if(CanDrag(target) == false) return false;

        MeshRenderer mesh = target.GetComponent<MeshRenderer>();
        
        return StartDragging(target, mesh.bounds.center - target.transform.position);
    }
    public bool StartDragging(GameObject target, Vector3 offset)
    {
        if(target == null) return false;
        if(CanDrag(target) == false) return false;
        
        mDraggedOriginX = target.transform.position.x;
        Vector3 toTarget = target.transform.position - mMain.transform.position;
        mDraggedOriginDistance = Vector3.Dot(toTarget, mMain.transform.forward);

        mDraggingRB = target.GetComponent<Rigidbody>();
        if (mDraggingRB != null)
        {
            mDraggedRBWasKinematic = mDraggingRB.isKinematic;
            mDraggingRB.isKinematic = true;
        }

        mDragging = target;
        mDraggingOffset = offset;
        Debug.Log("Start Drag");
        return true;
    }

    public void StopDragging()
    {
        if (mDragging == null) return;

        Rigidbody rb = mDragging.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = mDraggedRBWasKinematic;

        mDragging = null;
        mDraggingOffset = Vector3.zero;
        mDraggedOriginX = 0f;
        mTargetPos = Vector3.zero;
        Debug.Log("Stop Drag");
    }

    public bool IsDragging()
    {
        return mDragging != null;
    }
    public bool IsDragging(GameObject target)
    {
        return mDragging == target;
    }

    public bool CanDrag(GameObject target)
    {
        mRaycast.MouseRaycast(
            Mathf.Infinity,
            LayerMask.GetMask(LayerMask.LayerToName(target.layer)),
            target.transform.tag,
            out List<RaycastHit> hits
             );
        return hits.Any(hit => hit.collider.gameObject == target);
    }
}