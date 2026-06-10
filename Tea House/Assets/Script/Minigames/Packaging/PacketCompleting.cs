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

    private void Init()
    {
    }
    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
        if (mIsOpened == true && mIsFilled == false)
        {
            // TODO : RAYCAST HIT
            // List<GameObject> bowlHit = mMain.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "MixtureBowl");
            // if (bowlHit.Count > 0)
            // {
            //     mMain.mFilling.mBowl = bowlHit[0];
            //     mMain.mStateMachine.SwitchState(mMain.mFilling);
            // }
        }
        // TODO : RAYCAST HIT
        // List<GameObject> lidHit = mMain.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "PackageLid");
        // if (lidHit.Count > 0 && (mMain.mOpening.mLid != null ? lidHit.Contains(mMain.mOpening.mLid) : true))
        //         mMain.mStateMachine.SwitchState(mMain.mOpening);

        if(mLabelPrinted == false)
        {
            // TODO : RAYCAST HIT
            // List<GameObject> printerHit = mMain.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Printer");
            // if (printerHit.Count > 0)
            //     mMain.mStateMachine.SwitchState(mMain.mPrinting);
        }
        else if(mLabelPrinted == true && mIsLabelled == false)
        {
            // TODO : RAYCAST HIT
            // List<GameObject> labelHit = mMain.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Label");
            // if (labelHit.Count > 0)
            //     mMain.mStateMachine.SwitchState(mMain.mLabelling);
        }
    }
}
