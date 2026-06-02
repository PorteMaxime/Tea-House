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

    private GameObject mPrinterScreenUI = null;
    private GameObject mPrinterViewPoint = null;

    private void Init()
    {
        mPrinterViewPoint = mMain.transform.Find("Printer").transform.Find("ViewPoint").gameObject;
        mPrinterScreenUI = mPrinterViewPoint.transform.parent.Find("PrinterScreen").gameObject;
    }
    public void OnEnter()
    {
        mMain.mCameraController.GoTo(mPrinterViewPoint.transform);
        InputManager.Get().mPlayerInteract.Player.Open.Disable();
    }

    public void OnExit()
    {
        mMain.mCameraController.Back();
        InputManager.Get().mPlayerInteract.Player.Open.Enable();
        mPrinterScreenUI.SetActive(false);
    }

    public void OnUpdate()
    {
        if (mMain.mCameraController.IsMoving(0.001f) == false && mMain.mCameraController.IsRotating(0.001f) == false)
            mPrinterScreenUI.SetActive(true);

        if(InputManager.Get().mPlayerInteract.Player.Close.triggered == true)
        {
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
        }
    }
}