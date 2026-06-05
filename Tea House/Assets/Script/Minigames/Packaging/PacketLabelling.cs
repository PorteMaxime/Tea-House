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

    public GameObject mPrinterScreenUI = null;
    private GameObject mPrinterViewPoint = null;

    private GameObject mLabelPrefab = null;

    public GameObject mDebug = null;
    public IngredientData mDebugData = null;

    private Dictionary<Mail, GameObject> mOrders = new Dictionary<Mail, GameObject>();

    private Printer_SelectOrder mPrinterScript = null;
    private GameObject mSelectedGo = null;

    private void Init()
    {
        mPrinterViewPoint = mMain.transform.Find("Printer").transform.Find("ViewPoint").gameObject;
        mPrinterScreenUI = mPrinterViewPoint.transform.parent.Find("PrinterScreen").gameObject;

        mPrinterScript = mMain.transform.Find("Printer").GetComponent<Printer_SelectOrder>();

        mLabelPrefab = GameObject.Find("DynamicTextureGenerator").transform.Find("LabelTexture_Canva").gameObject;
        DynamicTextureManager.AddGenerator<LabelTextureGenerator>(DynamicTextureManager.DynamicTexture.Label, mLabelPrefab);
    }
    public void OnEnter()
    {
        mMain.mCameraController.GoTo(mPrinterViewPoint.transform);
        InputManager.Get().mPlayerInteract.Player.Open.Disable();

        foreach (Mail mail in mOrders.Keys)
        {
            if (mail.mOrder.mState != Order.State.Accepted)
            {
                GameObject.Destroy(mOrders[mail]);
                mOrders.Remove(mail);
            }
        }

        List<Mail> mails = MailManager.msInstance.GetMailsOfState(Order.State.Accepted);
        foreach (Mail mail in mails)
        {
            if (mOrders.ContainsKey(mail) == false)
                mOrders.Add(mail, mPrinterScript.CreateButton());
        }
    }

    public void OnExit()
    {
        mMain.mCameraController.Back();
        InputManager.Get().mPlayerInteract.Player.Open.Enable();
        mPrinterScreenUI.SetActive(false);

        if (mSelectedGo != null)
            mSelectedGo.transform.Find("Order_SelectionRect").gameObject.SetActive(false);
        mSelectedGo = null;
    }

    public void OnUpdate()
    {
        if (mMain.mCameraController.IsMoving(0.001f) == false && mMain.mCameraController.IsRotating(0.001f) == false)
            mPrinterScreenUI.SetActive(true);

        if (InputManager.Get().mPlayerInteract.Player.Close.triggered == true)
        {
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
        }
    }

    #region Printer Button Link
    public void SelectOrder(GameObject newOrderGo)
    {
        if (mSelectedGo != null) mSelectedGo.transform.Find("Order_SelectionRect").gameObject.SetActive(false);

        newOrderGo.transform.Find("Order_SelectionRect").gameObject.SetActive(true);
        mSelectedGo = newOrderGo;
    }
    public void Print()
    {
        if (mSelectedGo == null) return;

        // Generate Texture + Label
        Mail targetMail = null;
        foreach(Mail mail in mOrders.Keys)
        {
            if (mOrders[mail] == mSelectedGo)
            {
                targetMail = mail;
                break;
            }
        }

        RenderTexture texture = DynamicTextureManager.Generate(DynamicTextureManager.DynamicTexture.Label,
            targetMail.mPicture,
            "targetMail.mName",
            mDebugData,
            mDebugData,
            mDebugData,
            mDebugData
            );

        //Generate GameObject of the Label
        mDebug.GetComponent<MeshRenderer>().materials[0].mainTexture = texture;
        Debug.Log("Print");
    }
    #endregion
}