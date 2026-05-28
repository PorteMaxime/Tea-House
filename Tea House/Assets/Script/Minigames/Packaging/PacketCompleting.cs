using NUnit.Framework;
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

        if (mIsLabelled == false)
        {
            List<GameObject> hits = mMain.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Printer");
            if (hits.Count > 0)
                mMain.mStateMachine.SwitchState(mMain.mLabelling);
        }
    }
}
