using UnityEngine;
using UnityEngine.UI;

public class Printer_SelectOrder : MonoBehaviour
{
    private PacketLabelling mLabellingScript = null;
    private GameObject mScrollUI = null;
    private GameObject mPrinterScreenUI = null;

    private GameObject mOrderGoPrefab = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mOrderGoPrefab = Resources.Load<GameObject>("Printer/Printer_OrderPrefab");

        Init();
    }

    private void Init()
    {
        mLabellingScript = transform.parent.GetComponent<PackagingScript>().mLabelling;

        if (mLabellingScript == null) return;

        mPrinterScreenUI = mLabellingScript.mPrinterScreenUI;
        mScrollUI = mPrinterScreenUI.transform.Find("Background/Printer_Scroll/Viewport/Content").gameObject;
    }

    public void SelectOrder(GameObject orderGo) => mLabellingScript.SelectOrder(orderGo);
    public void Print()
    {
        if (mLabellingScript == null) Init();
        mLabellingScript.CreateLabel();
    }

    public GameObject CreateButton()
    {
        if (mLabellingScript == null) Init();

        GameObject mailGo = GameObject.Instantiate(mOrderGoPrefab, mScrollUI.transform);
        Vector3 currentMailPos = mailGo.transform.position;
        currentMailPos.z = 0f;
        mailGo.transform.position = currentMailPos;

        mailGo.transform.Find("Order_Button").GetComponent<Button>().onClick.AddListener(() => SelectOrder(mailGo));

        return mailGo;
    }
}
