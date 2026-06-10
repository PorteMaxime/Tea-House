using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq.Expressions;
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

    public GameObject mLid = null;
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

        // TODO : Start Dragging
        //mMain.mCameraController.StartDragging(mLid);
        if(mIsOpened == false)
        {
            mIsOpened = true;
            mMain.mCompleting.mIsOpened = true;
        }
    }
    public void OnExit()
    {
    }

    public void OnUpdate()
    {
        if(mIsOpened == true && mLidClosedPlaceCollider.IsColliding(mLid))
        {
            mIsOpened = false;
            mMain.mCompleting.mIsOpened = false;
            mLid.transform.position = mLidClosedPlace.transform.position;
            // TODO : Stop Dragging
            //mMain.mCameraController.StopDragging();
        }

        // TODO : Is Dragging
        if(/*mMain.mCameraController.IsDragging()*/ true == false)
        {
            mLidRB.isKinematic = mIsOpened != true;
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
        }
    }
}
