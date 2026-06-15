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

    private Input_PlayerInteract mPlayerInteract = null;

    public GameObject mLid = null;
    private Rigidbody mLidRB = null;

    private GameObject mLidClosedPlace = null;
    private WorkingPoint mLidClosedPlaceCollider = null;

    public bool mIsOpened = false;

    private CameraDragAndDrop mDragAndDrop = null;

    void Init()
    {
        mDragAndDrop = mMain.mCameraController.GetControllerMod<CameraDragAndDrop>();
        mPlayerInteract = InputManager.Get().mPlayerInteract;
        mPlayerInteract.Enable();
    }

    public void OnEnter()
    {
        mLid = mMain.mOutputGO.transform.Find("Lid").gameObject;
        mLidRB = mLid.GetComponent<Rigidbody>();

        mLidClosedPlace = mMain.mOutputGO.transform.Find("LidClosePlace").gameObject;
        mLidClosedPlaceCollider = mLidClosedPlace.GetComponent<WorkingPoint>();

        if(mDragAndDrop.StartDragging(mLid) == false)
        {
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
            Debug.Log("can't drag lid");
            return;
        }
        
        if(mIsOpened == false)
        {
            mIsOpened = true;
            mMain.mCompleting.mIsOpened = true;
        }
    }
    public void OnExit()
    {
        mDragAndDrop.StopDragging();
    }

    public void OnUpdate()
    {
        if(mIsOpened == true && mLidClosedPlaceCollider.IsColliding(mLid) && mPlayerInteract.Player.Select.WasReleasedThisFrame() == true)
        {
            mIsOpened = false;
            mMain.mCompleting.mIsOpened = false;
            mLid.transform.position = mLidClosedPlace.transform.position;
            mDragAndDrop.StopDragging();
        }
        else if(mPlayerInteract.Player.Select.WasReleasedThisFrame() == true)
        {
            mDragAndDrop.StopDragging();
        }

        if(mDragAndDrop.IsDragging() == false)
        {
            mLidRB.isKinematic = mIsOpened != true;
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
        }
    }
}
