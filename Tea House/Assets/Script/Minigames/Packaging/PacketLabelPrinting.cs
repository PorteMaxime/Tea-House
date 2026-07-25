
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UIElements;

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

    private Camera_DepthView mDepthView = null;
    
    public GameObject mPrinterScreenUI = null;
    private Transform mPrinterViewPoint = null;

    private GameObject mLabelTextureGenerator = null;
    private GameObject mLabelGoPrefab = null;
    private GameObject mLabelSpot = null;

    private Dictionary<Mail, GameObject> mOrders = new Dictionary<Mail, GameObject>();

    private Printer_SelectOrder mPrinterScript = null;
    private GameObject mSelectedOrderGo = null;

    public List<ItemData> mMixture = null;

    private void Init()
    {
        Transform printer = mMain.transform.Find("Printer");
        mPrinterViewPoint = printer.transform.Find("ViewPoint");
        mPrinterScreenUI = mPrinterViewPoint.transform.parent.Find("PrinterScreen").gameObject;

        mPrinterScript = printer.GetComponent<Printer_SelectOrder>();

        mLabelGoPrefab = Resources.Load<GameObject>("Printer/Label");
        mLabelSpot = printer.Find("LabelSpot").gameObject;

        mLabelTextureGenerator = GameObject.Find("DynamicTextureGenerator").transform.Find("LabelTexture_Canva").gameObject;
        DynamicTextureManager.AddGenerator<LabelTextureGenerator>(DynamicTextureManager.DynamicTexture.Label, mLabelTextureGenerator);

        mDepthView = mMain.mCameraController.GetControllerMod<Camera_DepthView>();
    }

    public void OnEnter()
    {
        mDepthView.GoTo(mPrinterViewPoint.transform);
        InputManager.Get().mPlayerInteract.Player.Open.Disable();

        List<Mail> toRemove = new List<Mail>();
        foreach (Mail mail in mOrders.Keys)
        {
            if(mail == null) mOrders.Remove(mail);
            if (mail.mOrder.mState != Order.State.Accepted || mail.mIsPrinted == true)
            {
                GameObject.Destroy(mOrders[mail]);
                toRemove.Add(mail);
            }
        }
        foreach(Mail remove in toRemove)
            mOrders.Remove(remove);

        List<Mail> mails =  mMain.mMailSystem.GetMailsOfState(Order.State.Accepted);
        foreach (Mail mail in mails)
        {
            if(mail.mIsPrinted == true)
                continue;
            if (mOrders.ContainsKey(mail) == false)
                mOrders.Add(mail, mPrinterScript.CreateButton(mail));
        }
    }

    public void OnExit()
    {
        InputManager.Get().mPlayerInteract.Player.Open.Enable();
        mPrinterScreenUI.SetActive(false);
        
        if(mSelectedOrderGo != null)
            mSelectedOrderGo.transform.Find("Order_SelectionRect").gameObject.SetActive(false);
        mDepthView.Back();
    }

    public void OnReset()
    {
        mMixture = null;
        mSelectedOrderGo = null;
    }

    public void OnUpdate()
    {
        if (mDepthView.IsMoving(0) == true && mDepthView.IsRotating(0) == true)
            mPrinterScreenUI.SetActive(true);

        if (InputManager.Get().mPlayerInteract.Player.Close.triggered == true)
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
    }  

    #region Printer Button Link
    public void SelectOrder(GameObject newOrderGo)
    {
        if (mSelectedOrderGo != null) mSelectedOrderGo.transform.Find("Order_SelectionRect").gameObject.SetActive(false);

        //newOrderGo.transform.Find("Order_SelectionRect").gameObject.SetActive(true);
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
        
        List<IngredientData> ingredients = new List<IngredientData>();
        BaseData baseData = null;
        foreach(ItemData item in mMixture)
        {
            if(item is BaseData)
                baseData = (BaseData)item;
            else if(item is IngredientData)
                ingredients.Add((IngredientData)item);
        }
        if(baseData == null || ingredients.Count == 0) return null;

        return DynamicTextureManager.Generate(DynamicTextureManager.DynamicTexture.Label,
            targetMail.mPicture,
            targetMail.mName,
            baseData,
            ingredients.Count >= 1 ? ingredients[0] : null,
            ingredients.Count >= 2 ? ingredients[1] : null,            
            ingredients.Count >= 3 ? ingredients[2] : null
            );
    }

    public void LinkMailToOutput()
    {
        if(mMain.mOutput == null) return;

        Mail targetMail = null;
        foreach(Mail mail in mOrders.Keys)
        {
            if (mOrders[mail] == mSelectedOrderGo)
            {
                targetMail = mail;
                break;
            }
        }

        mMain.mOutput.mTargetMail = targetMail;
    }
    #endregion  

    public void CreateLabel()
    {
        if(mMain.mCompleting.mLabelPrinted == true || mMain.mOutput == null) return;
    
        RenderTexture labelTexture = PrintLabel();
        if(labelTexture == null) return;

        GameObject label = GameObject.Instantiate(mLabelGoPrefab);
        label.transform.parent = mPrinterScript.gameObject.transform;
        label.transform.localRotation = Quaternion.identity;
        label.transform.localPosition = new Vector3(0, 0.001f, 0);

        label.GetComponent<MeshRenderer>().material.mainTexture = labelTexture;
        mMain.mLabelling.mLabelGo = label;
        mMain.mCompleting.mLabelPrinted = true;

        LinkMailToOutput();
        // string debug = $"{mMain.mOutput.mTargetMail.mIsPrinted} | ";
        mMain.mOutput.mTargetMail.mIsPrinted = true;
        // debug += $"{mMain.mOutput.mTargetMail.mIsPrinted}";
        // Debug.Log(debug);
        
        
        mMain.StartCoroutine(SlideLabel(label, mLabelSpot.transform.position, mLabelSpot.transform.rotation, 5f));
        mMain.mStateMachine.SwitchState(mMain.mCompleting);
    }

    IEnumerator SlideLabel(GameObject labelGO, Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        float elapsedTime = 0;
        Vector3 positionOrigine = labelGO.transform.position;
        Quaternion rotationOrigine = labelGO.transform.rotation;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            labelGO.transform.position = Vector3.Lerp(positionOrigine, targetPosition, elapsedTime / duration);
            labelGO.transform.rotation = Quaternion.Lerp(rotationOrigine, targetRotation, elapsedTime / duration);
            yield return 0;
        }

        labelGO.transform.position = targetPosition;
        labelGO.transform.rotation = targetRotation;
    }
}
 