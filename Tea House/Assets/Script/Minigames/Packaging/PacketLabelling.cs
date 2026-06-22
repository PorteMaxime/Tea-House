using System.Collections.Generic;
using UnityEngine;

public class PacketLabelling : IPackaging
{
    #region Main
    private PackagingScript mMain;
    public PacketLabelling(PackagingScript main)
    {
        mMain = main;
        Init();
    }
    #endregion

    private Dictionary<Mail, GameObject> mOrders = new Dictionary<Mail, GameObject>();

    public GameObject mLabelGo = null;

    private CameraDragAndDrop mDragAndDrop = null;

    private void Init()
    {
        mDragAndDrop = mMain.mCameraController.GetControllerMod<CameraDragAndDrop>();
    }
    public void OnEnter()
    {
        if(mDragAndDrop.StartDragging(mLabelGo) == false) mMain.mStateMachine.SwitchState(mMain.mCompleting);
    }

    public void OnExit()
    {
        mDragAndDrop.StopDragging();
    }

    public void OnUpdate()
    {
        // Leave state if the player release the label
        if(mDragAndDrop.IsDragging() == false)
            mMain.mStateMachine.SwitchState(mMain.mCompleting);

        mLabelGo.transform.rotation = mMain.mCameraController.gameObject.transform.rotation * Quaternion.Euler(90, 0, 180);
        
        if(mMain.mWorkingPoint.IsColliding(mLabelGo))
        {
            mLabelGo.transform.parent = mMain.mOutputGO.transform;
            mLabelGo.transform.rotation = Quaternion.Euler(80, 90, 0);

            SnapLabelToPackage(mLabelGo);
            
            mMain.mCompleting.mIsLabelled = true;
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
        }
    }

    private void SnapLabelToPackage(GameObject labelGO)
    {
        Transform labelPlace = mMain.mOutputGO.transform.Find("LabelPlace");
        if(labelPlace == null)
        {
            GameObject packageBase = mMain.mOutputGO.transform.Find("Base").gameObject;
            labelGO.transform.localPosition = new Vector3(
                0.00105f,
                0.003f,
                0
            );
            return;
        }
        labelGO.transform.localPosition = labelPlace.transform.localPosition;
    }
}