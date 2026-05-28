using UnityEngine;

public class PacketFilling : IPackaging
{
    #region Main
    private PackagingScript mMain;
    public PacketFilling(PackagingScript main)
    {
        mMain = main;
        Init();
    }
    #endregion
    bool isFilled = false;

    public GameObject mBowl = null;
    private void Init()
    {

    }
    public void OnEnter()
    {
        mMain.mCameraController.StartDragging(mBowl);
    }

    public void OnExit()
    {
        mMain.mCompleting.mIsFilled = isFilled;
    }

    public void OnUpdate()
    {
        if (mMain.mCameraController.IsDragging() == false)
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
    }
}
