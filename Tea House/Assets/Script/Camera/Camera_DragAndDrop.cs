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
    public GameObject mDragging = null;

    private float mDraggedOriginX = 0f;
    private float mDraggedOriginDistance = 0f;

    private Vector3 mTargetPos = Vector3.zero;
    private float mMoveSpeed = 10f;

    private bool mUseGravity = false;
    private bool mIsKinematic = false;
    private bool mWasMeshColliding = false;

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

        mTargetPos = worldPos - mDraggingOffset;
    }

    public void OnFixedUpdate()
    {
        if(IsDragging() == false || mDraggingRB == null) return;
        //Debug.Log(mDragging.name);
        Vector3 moveVector = mTargetPos - mDraggingRB.position;
        moveVector *= mMoveSpeed;
        mDraggingRB.linearVelocity = moveVector;
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
            mIsKinematic = mDraggingRB.isKinematic;
            mUseGravity = mDraggingRB.useGravity;

            mDraggingRB.interpolation = RigidbodyInterpolation.Interpolate;
            mDraggingRB.isKinematic = false;
            mDraggingRB.useGravity = false;
            mDraggingRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX;
        }

        MeshCollider meshCollider = target.GetComponent<MeshCollider>();
        if(meshCollider != null)
        {
            mWasMeshColliding = meshCollider.enabled;
            meshCollider.enabled = false;
        }

        mDragging = target;
        mDraggingOffset = offset;
        return true;
    }

    public void StopDragging()
    {
        if (mDragging == null) return;

        Rigidbody rb = mDragging.GetComponent<Rigidbody>();
        if (rb != null) 
        {
            rb.useGravity = mUseGravity;
            rb.isKinematic = mIsKinematic;
            mDragging.transform.position = rb.position;
            mDraggingRB.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX;
        }

        MeshCollider meshCollider = mDragging.GetComponent<MeshCollider>();
        if(meshCollider != null)
            meshCollider.enabled = mWasMeshColliding;


        mDragging = null;
        mDraggingOffset = Vector3.zero;
        mDraggedOriginX = 0f;
        mTargetPos = Vector3.zero;
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

        return IsDragging() == false && hits.Any(hit => hit.collider.gameObject == target);
    }
}