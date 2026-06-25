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
    private GameObject mLabelSpot = null;
    private WorkingPoint mLabelSpotWP = null;

    private CameraDragAndDrop mDragAndDrop = null;

    private void Init()
    {
        mDragAndDrop = mMain.mCameraController.GetControllerMod<CameraDragAndDrop>();
    }
    public void OnEnter()
    {
        if(mDragAndDrop.StartDragging(mLabelGo) == false) mMain.mStateMachine.SwitchState(mMain.mCompleting);
        mLabelSpot = mMain.mOutputGO.transform.Find("LabelPlace").gameObject;
        mLabelSpotWP = mLabelSpot.GetComponent<WorkingPoint>();
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
        // Leave state if the player release the label
        if(mDragAndDrop.IsDragging() == false || InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame() == true)
            mMain.mStateMachine.SwitchState(mMain.mCompleting);

        mLabelGo.transform.rotation = mMain.mCameraController.gameObject.transform.rotation * Quaternion.Euler(90, 0, 180);
        
        if(mLabelSpotWP.IsColliding(mLabelGo))
        {
            mLabelGo.transform.parent = mMain.mOutputGO.transform;

            mMain.mCompleting.mIsLabelled = true;
            mDragAndDrop.StopDragging();
            mLabelGo.GetComponent<Rigidbody>().isKinematic = true;
            mLabelGo.transform.SetPositionAndRotation(mLabelSpotWP.transform.position, mLabelSpotWP.transform.rotation);
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
        }
    }
}