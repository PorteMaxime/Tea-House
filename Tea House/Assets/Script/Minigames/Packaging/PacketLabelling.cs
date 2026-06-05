using System.Collections.Generic;
using Unity.VisualScripting;
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

    public bool mLabelPrinted = false;

    public GameObject mPrinterScreenUI = null;
    private GameObject mPrinterViewPoint = null;

    private GameObject mLabelTextureGenerator = null;
    private GameObject mLabelGoPrefab = null;

    private Dictionary<Mail, GameObject> mOrders = new Dictionary<Mail, GameObject>();

    private Printer_SelectOrder mPrinterScript = null;
    private GameObject mSelectedOrderGo = null;

    private GameObject mLabelGo = null;

    // TODO : Remove Debug
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
        // If label is not printed, go to printer
        if(mLabelPrinted == false)
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
        else
            mMain.mCameraController.StartDragging(mLabelGo);
    }

    public void OnExit()
    {
        mMain.mCameraController.Back();
        InputManager.Get().mPlayerInteract.Player.Open.Enable();
        mPrinterScreenUI.SetActive(false);

        if (mSelectedOrderGo != null)
            mSelectedOrderGo.transform.Find("Order_SelectionRect").gameObject.SetActive(false);
        mSelectedOrderGo = null;
    }

    public void OnUpdate()
    {
        // Activate Printer UI
        if (mMain.mCameraController.IsMoving(0.001f) == false && mMain.mCameraController.IsRotating(0.001f) == false && mLabelPrinted == false)
            mPrinterScreenUI.SetActive(true);

        // Leave if the player press Exit key
        if (InputManager.Get().mPlayerInteract.Player.Close.triggered == true)
            mMain.mStateMachine.SwitchState(mMain.mCompleting);

        // Labelling Logic
        if(mLabelGo == null) CreateLabel();
        if(mLabelGo != null)
        {
            mLabelGo.transform.rotation = mMain.mCameraController.gameObject.transform.rotation * Quaternion.Euler(90, 0, 180);

            if(mMain.mWorkingPoint.mCollider == mLabelGo)
            {
                mLabelGo.transform.parent = mMain.mOutputGO.transform;
                mLabelGo.transform.rotation = Quaternion.Euler(45, 0, 0);
                mMain.mCameraController.StopDragging();
                mMain.mCompleting.mIsLabelled = true;
                mMain.mStateMachine.SwitchState(mMain.mCompleting);
            }
        }
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
        if(mLabelPrinted == true) return;

        RenderTexture labelTexture = PrintLabel();
        if(labelTexture == null) return;

        // TODO : CreateGameObject of Label
        GameObject label = GameObject.Instantiate(mLabelGoPrefab);
        label.transform.parent = mPrinterScript.gameObject.transform;
        label.transform.localRotation = Quaternion.identity;
        label.transform.localPosition = new Vector3(0, 0.001f, 0.003f); // TODO : Find a better way to set the position of the label
        //label.transform.localScale *= 100f;                             // TODO : Find a better way to set the scale of the label

        label.GetComponent<MeshRenderer>().material.mainTexture = labelTexture;
        mLabelGo = label;
        mLabelPrinted = true;
    }
}