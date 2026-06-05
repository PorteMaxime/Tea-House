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
    public bool mIsLabelled = false;

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
        if (mIsFilled == false)
        {
            List<GameObject> hits = mMain.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Ingredient");
            if (hits.Count > 0)
            {
                mMain.mFilling.mBowl = hits[0];
                mMain.mStateMachine.SwitchState(mMain.mFilling);
            }
        }

        Debug.Log(mIsLabelled);
        Debug.Log(mMain.mLabelling.mLabelPrinted);
        if (mIsLabelled == false)
        {
            // If the player click on the printer
            if(mMain.mLabelling.mLabelPrinted == false)
            {
                List<GameObject> hits = mMain.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Printer");
                if (hits.Count > 0)
                    mMain.mStateMachine.SwitchState(mMain.mLabelling);
            }
            // If the player click on the label
            else
            {
                List<GameObject> hits = mMain.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Label");
                Debug.Log(hits.Count);
                if (hits.Count > 0)
                    mMain.mStateMachine.SwitchState(mMain.mLabelling);
            }
        }
    }
}
