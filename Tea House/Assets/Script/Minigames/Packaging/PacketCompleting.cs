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

    private GameObject mLabelTrash = null;
    private bool mIsThrowing = false;

    private GameObject mExport = null;
    private bool mIsExporting = false;

    private void Init()
    {
        mRaycast = mMain.mCameraController.GetControllerMod<Camera_Raycast>();
        mPlayerInteract = InputManager.Get().mPlayerInteract;
        mPlayerInteract.Enable();

        mLabelTrash = mMain.transform.Find("LabelTrash").gameObject;
        mExport = mMain.transform.Find("PackageExport").gameObject;
    }
    public void OnEnter()
    {
    }

    public void OnExit()
    {
        mIsThrowing = false;
    }

    public void OnReset()
    {
        mIsChoosed = false;
        mIsFilled = false;
        mLabelPrinted = false;
        mIsLabelled = false;
        mIsOpened = false;

        mIsThrowing = false;
        mIsExporting = false;
    }

    public void OnUpdate()
    {
        if (mPlayerInteract.Player.Select.triggered == true)
        {
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
            else
            {
                mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "Label", out RaycastHit label);
                if(label.collider != null)
                {
                    mMain.mStateMachine.SwitchState(mMain.mLabelling);
                    return;
                }
            }
        }

        if(mIsLabelled == true || mLabelPrinted == true)
        {
            if(mIsThrowing == true)
            {
                mLabelTrash.transform.GetChild(0).gameObject.SetActive(false);
                mLabelTrash.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                mLabelTrash.transform.GetChild(0).gameObject.SetActive(true);
                mLabelTrash.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        else
        {
            mLabelTrash.transform.GetChild(0).gameObject.SetActive(false);
            mLabelTrash.transform.GetChild(1).gameObject.SetActive(false);
        }
        Debug.Log("Labelled: " + mIsLabelled + " | Choosed: " + mIsChoosed + " | Filled: " + mIsFilled + " | Opened: " + mIsOpened);
        if(mIsLabelled == true && mIsChoosed == true && mIsFilled == true && mIsOpened == false)
        {
            if(mIsExporting == true)
            {
                mExport.transform.GetChild(0).gameObject.SetActive(false);
                mExport.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                mExport.transform.GetChild(0).gameObject.SetActive(true);
                mExport.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        else
        {
            mExport.transform.GetChild(0).gameObject.SetActive(false);
            mExport.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void ThrowLabel()
    {
        if(mIsLabelled == false && mLabelPrinted == false) return;

        if(mIsThrowing == true)
        {
            Transform label = null;
            if(mIsLabelled == true)
                label = mMain.mOutputGO.transform.Find("Label(Clone)");
            else if(mLabelPrinted == true)
                label = mMain.transform.Find("Printer/Label(Clone)");

            GameObject.Destroy(label.gameObject);
            mIsThrowing = false;
            mLabelPrinted = false;
            mIsLabelled = false;
        }
        else
            mIsThrowing = true;
    }

    public void ExportPackage()
    {
        if(mIsLabelled == false || mIsChoosed == false || mIsFilled == false || mIsOpened == true) return;

        if(mIsExporting == true)
        {
            mMain.mOutputGO.transform.position = Vector3.zero;
            mMain.mOutput.transform.parent = null;
            mMain.ResetFSM();

            // TODO: Add export to the table

            mIsExporting = false;
        }
        else
            mIsExporting = true;
    }
}
