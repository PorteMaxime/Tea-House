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
            mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "MixtureBowl", out RaycastHit bowl);
            if (bowl.collider != null)
            {
                mMain.mFilling.mBowl = bowl.collider.gameObject;
                mMain.mStateMachine.SwitchState(mMain.mFilling);
                return;
            }
        }
        mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "PackageLid", out RaycastHit lid);
        if (lid.collider != null && (mMain.mOpening.mLid != null ? lid.collider.gameObject == mMain.mOpening.mLid : true))
        {
            mMain.mStateMachine.SwitchState(mMain.mOpening);
            return;
        }

        if(mLabelPrinted == false)
        {
            mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "Printer", out RaycastHit printer);
            if (printer.collider != null)
            {
                mMain.mStateMachine.SwitchState(mMain.mPrinting);
                return;
            }
        }
        else if(mLabelPrinted == true && mIsLabelled == false)
        {
            mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "Label", out RaycastHit label);
            if (label.collider != null)
            {
                mMain.mStateMachine.SwitchState(mMain.mLabelling);
                return;
            }
        }
    }
}
