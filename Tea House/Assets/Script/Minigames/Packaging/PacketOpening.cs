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

    private Animator mPackageAnimator = null;

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
        Transform lidTransform = mMain.mOutputGO.transform.Find("Lid");
        if(lidTransform != null)
        {
            mLid = lidTransform.gameObject;
            mLidRB = mLid.GetComponent<Rigidbody>();

            mLidClosedPlace = mMain.mOutputGO.transform.Find("LidClosePlace").gameObject;
            mLidClosedPlaceCollider = mLidClosedPlace.GetComponent<WorkingPoint>();

            if(mDragAndDrop.StartDragging(mLid) == false)
            {
                mMain.mStateMachine.SwitchState(mMain.mCompleting);
                return;
            }
            if (mIsOpened == false)
                mIsOpened = true;
        }
        else
        {
            mPackageAnimator = mMain.mOutputGO.GetComponent<Animator>();
            AnimatorStateInfo animatorInfo =  mPackageAnimator.GetCurrentAnimatorStateInfo(0);
            if(mPackageAnimator.IsInTransition(0) == false || (animatorInfo.normalizedTime < 1f) == false)
            {
                mPackageAnimator.SetFloat("AnimSpeed", mIsOpened == true ? -1f : 1f);
            
                float startFrame = mIsOpened ? 1f : 0f;
                mPackageAnimator.Play("OpenBag", 0, startFrame);

                mIsOpened = mIsOpened == true ? false : true;
            }
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
        }

        mMain.mCompleting.mIsOpened = mIsOpened;
    }
    public void OnExit()
    {
        mDragAndDrop.StopDragging();
    }

    public void OnReset()
    {
        mLid = null;
        mLidRB = null;
        mLidClosedPlace = null;
        mLidClosedPlaceCollider = null;
        mPackageAnimator = null;
        mIsOpened = false;
    }

    public void OnUpdate()
    {        
        if(mIsOpened == true && mLidClosedPlaceCollider.IsColliding(mLid) && mPlayerInteract.Player.Select.WasReleasedThisFrame() == true)
        {
            mIsOpened = false;
            mMain.mCompleting.mIsOpened = false;
            mDragAndDrop.StopDragging();
            mLid.transform.position = mLidClosedPlace.transform.position;
            mLid.transform.rotation = mLidClosedPlace.transform.rotation;
        }
        else if(mPlayerInteract.Player.Select.WasReleasedThisFrame() == true)
            mDragAndDrop.StopDragging();
        
        if(mDragAndDrop.IsDragging() == false)
        {
            if(mLidRB != null)
                mLidRB.isKinematic = mIsOpened != true;
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
        }
    }
}
