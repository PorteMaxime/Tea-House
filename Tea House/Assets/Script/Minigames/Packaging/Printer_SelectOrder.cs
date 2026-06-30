using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Printer_SelectOrder : MonoBehaviour
{
    private PacketLabelPrinting mPacketLabelPrinting = null;
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
        mPacketLabelPrinting = transform.parent.GetComponent<PackagingScript>().mPrinting;

        if (mPacketLabelPrinting == null) return;

        mPrinterScreenUI = mPacketLabelPrinting.mPrinterScreenUI;
        mScrollUI = mPrinterScreenUI.transform.Find("Background/Printer_Scroll/Viewport/Content").gameObject;
    }

    public void SelectOrder(GameObject orderGo) => mPacketLabelPrinting.SelectOrder(orderGo);
    public void Print()
    {
        if (mPacketLabelPrinting == null) Init();
        mPacketLabelPrinting.CreateLabel();
    }

    public GameObject CreateButton(Mail mail)
    {
        if (mPacketLabelPrinting == null) Init();

        GameObject mailGo = GameObject.Instantiate(mOrderGoPrefab, mScrollUI.transform);
        Vector3 currentMailPos = mailGo.transform.position;
        currentMailPos.z = 0f;
        mailGo.transform.position = currentMailPos;

        mailGo.transform.Find("Order_Picture").GetComponent<Image>().sprite = mail.mPicture;
        string name = mail.mName;
        if(name.Length > 11)
            name = name.Substring(0, 11) + "...";
        mailGo.transform.Find("Order_Number").GetComponent<TextMeshProUGUI>().text = name;
        string desc = mail.mText.Replace("\n", " ").Replace("\r", " ");
        if(desc.Length > 30)
            desc = desc.Substring(0, 30) + "...";
        mailGo.transform.Find("Order_Description").GetComponent<TextMeshProUGUI>().text = desc;

        mailGo.transform.Find("Order_Button").GetComponent<Button>().onClick.AddListener(() => SelectOrder(mailGo));

        return mailGo;
    }
}
