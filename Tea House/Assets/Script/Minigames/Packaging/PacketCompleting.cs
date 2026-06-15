using System.Collections.Generic;
using UnityEngine;

public class PacketCompleting : IPackaging
{
    #region Main
    private PackagingScript mMain;
    public PacketCompleting(PackagingScript main)
    {
        mMain = main;
        Init();
    }
    #endregion

    public bool mIsFilled = false;
    public bool mLabelPrinted = false;
    public bool mIsLabelled = false;
    public bool mIsOpened = false;

    private Camera_Raycast mRaycast = null;
    private Input_PlayerInteract mPlayerInteract = null;

    private void Init()
    {
        mRaycast = mMain.mCameraController.GetControllerMod<Camera_Raycast>();
        mPlayerInteract = InputManager.Get().mPlayerInteract;
        mPlayerInteract.Enable();
    }
    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
        if (mPlayerInteract.Player.Select.IsPressed() == false) return;

        if (mIsOpened == true && mIsFilled == false)
        {
            GameObject bowl = null;
            mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "MixtureBowl", out bowl);
            if (bowl != null)
            {
                mMain.mFilling.mBowl = bowl;
                mMain.mStateMachine.SwitchState(mMain.mFilling);
            }
        }
        GameObject lid = null;
        mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "PackageLid", out lid);
        if (lid != null && (mMain.mOpening.mLid != null ? lid == mMain.mOpening.mLid : true))
                mMain.mStateMachine.SwitchState(mMain.mOpening);

        if(mLabelPrinted == false)
        {
            GameObject printer = null;
            mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "Printer", out printer);
            if (printer != null)
                mMain.mStateMachine.SwitchState(mMain.mPrinting);
        }
        else if(mLabelPrinted == true && mIsLabelled == false)
        {
            GameObject label = null;
            mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "Label", out label);
            if (label != null)
                mMain.mStateMachine.SwitchState(mMain.mLabelling);
        }
    }
}
