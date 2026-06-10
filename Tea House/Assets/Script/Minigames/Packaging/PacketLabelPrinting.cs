
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class PacketLabelPrinting : IPackaging
{
    #region Main
    private PackagingScript mMain;
    public PacketLabelPrinting(PackagingScript main)
    {
        mMain = main;
        Init();
    }
    #endregion

    private bool mIsPrinted = false;

    public GameObject mPrinterScreenUI = null;
    private GameObject mPrinterViewPoint = null;

    private GameObject mLabelTextureGenerator = null;
    private GameObject mLabelGoPrefab = null;

    private Dictionary<Mail, GameObject> mOrders = new Dictionary<Mail, GameObject>();

    private Printer_SelectOrder mPrinterScript = null;
    private GameObject mSelectedOrderGo = null;

    public GameObject mDebug = null;
    public IngredientData mDebugData = null;

    private void Init()
    {
        mPrinterViewPoint = mMain.transform.Find("Printer").transform.Find("ViewPoint").gameObject;
        mPrinterScreenUI = mPrinterViewPoint.transform.parent.Find("PrinterScreen").gameObject;

        mPrinterScript = mMain.transform.Find("Printer").GetComponent<Printer_SelectOrder>();

        mLabelGoPrefab = Resources.Load<GameObject>("Printer/Label");

        mLabelTextureGenerator = GameObject.Find("DynamicTextureGenerator").transform.Find("LabelTexture_Canva").gameObject;
        DynamicTextureManager.AddGenerator<LabelTextureGenerator>(DynamicTextureManager.DynamicTexture.Label, mLabelTextureGenerator);
    }

    public void OnEnter()
    {
        // TODO : GoTo
        //mMain.mCameraController.GoTo(mPrinterViewPoint.transform);
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
        InputManager.Get().mPlayerInteract.Player.Open.Enable();
        mPrinterScreenUI.SetActive(false);
        
        // TODO : Back
        //mMain.mCameraController.Back();
    }

    public void OnUpdate()
    {
        // TODO : IsMoving + IsRotating
        // if (mMain.mCameraController.IsMoving(0.001f) == false && mMain.mCameraController.IsRotating(0.001f) == false)
        //     mPrinterScreenUI.SetActive(true);

        if (InputManager.Get().mPlayerInteract.Player.Close.triggered == true)
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
    }  

    #region Printer Button Link
    public void SelectOrder(GameObject newOrderGo)
    {
        if (mSelectedOrderGo != null) mSelectedOrderGo.transform.Find("Order_SelectionRect").gameObject.SetActive(false);

        newOrderGo.transform.Find("Order_SelectionRect").gameObject.SetActive(true);
        mSelectedOrderGo = newOrderGo;
    }
    public RenderTexture PrintLabel()
    {
        if (mSelectedOrderGo == null) return null;

        // Generate Texture + Label
        Mail targetMail = null;
        foreach(Mail mail in mOrders.Keys)
        {
            if (mOrders[mail] == mSelectedOrderGo)
            {
                targetMail = mail;
                break;
            }
        }

        // TODO : Generate Texture with targetMail data => Remove Debug Data
        return DynamicTextureManager.Generate(DynamicTextureManager.DynamicTexture.Label,
            targetMail.mPicture,
            "targetMail.mName",
            mDebugData,
            mDebugData,
            mDebugData,
            mDebugData
            );
    }
    #endregion  

    public void CreateLabel()
    {
        if(mIsPrinted == true) return;

        RenderTexture labelTexture = PrintLabel();
        if(labelTexture == null) return;

        GameObject label = GameObject.Instantiate(mLabelGoPrefab);
        label.transform.parent = mPrinterScript.gameObject.transform;
        label.transform.localRotation = Quaternion.identity;
        label.transform.localPosition = new Vector3(0, 0.001f, 0.003f); // TODO : Find a better way to set the position of the label

        label.GetComponent<MeshRenderer>().material.mainTexture = labelTexture;
        mMain.mLabelling.mLabelGo = label;
        mIsPrinted = true;
        mMain.mCompleting.mLabelPrinted = true;
        mMain.mStateMachine.SwitchState(mMain.mCompleting);
    }
}
