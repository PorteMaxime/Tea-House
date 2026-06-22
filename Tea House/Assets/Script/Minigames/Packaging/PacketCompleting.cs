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

    public bool mIsChoosed = false;
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
        if (mPlayerInteract.Player.Select.triggered == false) return;

        // Package Choosing
        if(mIsFilled == false && mIsOpened == false && mIsLabelled == false)
        {
            mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "Package", out RaycastHit packageChoosed);
            if(packageChoosed.collider != null && (packageChoosed.collider != mMain.mOutputGO && packageChoosed.collider.transform.parent.gameObject != mMain.mOutputGO))
            {
                mMain.mChoosing.mChoosedPackage = packageChoosed.collider.gameObject;
                mMain.mStateMachine.SwitchState(mMain.mChoosing);
                return;
            }
        }

        // Package Opening / Closing
        mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), (string[])null, out RaycastHit package);
        if(package.collider != null && (package.collider == mMain.mOutputGO || package.collider.transform.parent.gameObject == mMain.mOutputGO))
        {
            mMain.mStateMachine.SwitchState(mMain.mOpening);
            return;
        }

        // Package Filling
        if(mIsFilled == false && mIsOpened == true)
        {
            mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "MixtureBowl", out RaycastHit bowl);
            if(bowl.collider != null)
            {
                mMain.mFilling.mBowl = bowl.collider.gameObject;
                mMain.mStateMachine.SwitchState(mMain.mFilling);
                return;
            }
        }

        // Label Printing
        if(mLabelPrinted == false)
        {
            mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "Printer", out RaycastHit printer);
            if(printer.collider != null)
            {
                mMain.mStateMachine.SwitchState(mMain.mPrinting);
                return;
            }
        }

        // Package Labelling
        if(mLabelPrinted == true)
        {
            mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "Label", out RaycastHit label);
            if(label.collider != null)
            {
                mMain.mStateMachine.SwitchState(mMain.mLabelling);
                return;
            }
        }

        // if (mIsOpened == true && mIsFilled == false)
        // {
        //     mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "MixtureBowl", out RaycastHit bowl);
        //     if (bowl.collider != null)
        //     {
        //         mMain.mFilling.mBowl = bowl.collider.gameObject;
        //         mMain.mStateMachine.SwitchState(mMain.mFilling);
        //         return;
        //     }
        // }
        // mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "PackageLid", out RaycastHit lid);
        // if (lid.collider != null && (mMain.mOpening.mLid != null ? lid.collider.gameObject == mMain.mOpening.mLid : true))
        // {
        //     mMain.mStateMachine.SwitchState(mMain.mOpening);
        //     return;
        // }

        // if(mLabelPrinted == false)
        // {
        //     mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "Printer", out RaycastHit printer);
        //     if (printer.collider != null)
        //     {
        //         mMain.mStateMachine.SwitchState(mMain.mPrinting);
        //         return;
        //     }
        // }
        // else if(mLabelPrinted == true && mIsLabelled == false)
        // {
        //     mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "Label", out RaycastHit label);
        //     if (label.collider != null)
        //     {
        //         mMain.mStateMachine.SwitchState(mMain.mLabelling);
        //         return;
        //     }
        // }
    }
}
