using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class PacketOpening : IPackaging
{
    #region Main
    private PackagingScript mMain;
    public PacketOpening(PackagingScript main)
    {
        mMain = main;
        Init();
    }
    #endregion

    private GameObject mLid = null;
    private Rigidbody mLidRB = null;

    private GameObject mLidClosedPlace = null;
    private WorkingPoint mLidClosedPlaceCollider = null;

    public bool mIsOpened = false;

    void Init()
    {
        
    }

    public void OnEnter()
    {
        mLid = mMain.mOutputGO.transform.Find("Lid").gameObject;
        mLidRB = mLid.GetComponent<Rigidbody>();

        mLidClosedPlace = mMain.mOutputGO.transform.Find("LidClosePlace").gameObject;
        mLidClosedPlaceCollider = mLidClosedPlace.GetComponent<WorkingPoint>();
    }
    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        if(mIsOpened == true && mMain.mCameraController.IsDragging() == false) mLidRB.isKinematic = false;
        else if(mMain.mCameraController.IsDragging() == false) mLidRB.isKinematic = true;

        if (mMain.mCameraController.IsDragging() == false)
        {
            List<GameObject> hitDraggables = mMain.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "PackageLid");
            if(hitDraggables.Count > 0 && hitDraggables[0] == mLid)
            { 
                mMain.mCameraController.StartDragging(mLid);
                if(mIsOpened == false) mIsOpened = true;
            }
        }
        else
        {
            if(mIsOpened == true && mLidClosedPlaceCollider.IsColliding(mLid))
            {
                mIsOpened = false;
                mLid.transform.position = mLidClosedPlace.transform.position;
                mMain.mCameraController.StopDragging();
            }
        }
    }
}
