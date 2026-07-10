using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PackageInventory_UIManager : MonoBehaviour
{
    [System.Serializable]
    public class UI_Slot
    {
        #region Usage Icon
        static public Color usedColor = Color.gray;
        static public Color unusedColor = Color.white;
        [SerializeField]public Image mUsedIcon = null;
        #endregion

        #region Usage Text
        static public string unusedText = "Unused";
        [SerializeField]public TextMeshProUGUI mOrderName = null;
        #endregion

        #region To Computer Button
        [SerializeField]public UnityAction mToComputerListener = null;
        [SerializeField]public Button mToComputerButton = null;
        #endregion

        #region Throw Button
        [SerializeField]public UnityAction mThrowListener = null;
        [SerializeField]public Button mThrowButton = null;
        #endregion

        public UI_Slot(Transform slotTransform)
        {
            mUsedIcon = slotTransform.GetChild(0).GetComponent<Image>();
            mOrderName = slotTransform.GetChild(1).GetComponent<TextMeshProUGUI>();
            mThrowButton = slotTransform.GetChild(2).GetComponent<Button>();
            mToComputerButton = slotTransform.GetChild(3).GetComponent<Button>();
        }

        public void Reset()
        {
            mUsedIcon.color = UI_Slot.unusedColor;
            mOrderName.text = UI_Slot.unusedText;
                
            if(mThrowListener != null)
                mThrowButton.onClick.RemoveListener(mThrowListener);
            mThrowListener = null;
            if(mToComputerListener != null)
                mToComputerButton.onClick.RemoveListener(mToComputerListener);
            mToComputerListener = null;
        }
    }

    public List<UI_Slot> mSlots = new List<UI_Slot>();
    public List<PackageScript> mPackages = null;

    private InteractiveAreaScript mIArea = null;
    private InteractiveAreaScript mComputerIArea = null;
    private ComputerScript mComputerScript = null;
    private MailCanvasScript mMailCanva = null;


    void Start()
    {
        Transform slotsUI = transform.Find("Dynamic/Slots");
        foreach(Transform slot in slotsUI)
            mSlots.Add(new UI_Slot(slot));
        mIArea = transform.parent.Find("InteractiveArea").GetComponent<InteractiveAreaScript>();

        Transform computerSpace = GameObject.Find("Computer_Space").transform;
        mComputerIArea = computerSpace.Find("InteractiveArea").GetComponent<InteractiveAreaScript>();
        mComputerScript = computerSpace.Find("ComputerCanvas").GetComponent<ComputerScript>();
        mMailCanva = GameObject.FindGameObjectWithTag("MailManager").GetComponent<MailCanvasScript>();
    }

    public void LinkPackageList(List<PackageScript> packageList) => mPackages = packageList;

    public void RefreshPackage()
    {
        for(int i = mSlots.Count - 1; i >= 0; i--)
        {
            mSlots[i].Reset();
            if(i < mPackages.Count && mPackages[i] != null)
            {
                UI_Slot slot = mSlots[i];
                slot.mUsedIcon.color = UI_Slot.usedColor;
                slot.mOrderName.text = mPackages[i].mTargetMail.mName;

                int toCall = i;
                slot.mThrowListener = () => ThrowPackage(toCall);
                slot.mThrowButton.onClick.AddListener(slot.mThrowListener);
                slot.mToComputerListener = () => PackageToComputer(toCall);
                slot.mToComputerButton.onClick.AddListener(slot.mToComputerListener);
            }
            else
            {
                //mPackages.RemoveAt(i);
            }
        }
    }

    public void ThrowPackage(int packageIndex)
    {
        if(packageIndex < mPackages.Count)
        {
            PackageScript package = mPackages[packageIndex];
            mPackages[packageIndex] = null;
            package.mTargetMail.mIsPrinted = false;
            Destroy(package.gameObject);
        }
        RefreshPackage();
    }

    public void PackageToComputer(int packageIndex) => StartCoroutine(ToComputer(packageIndex));

    IEnumerator ToComputer(int packageIndex)
    {
        Coroutine coroutine = mIArea.GoTo(mComputerIArea);
        if(coroutine != null)
            yield return coroutine;
        while(mComputerIArea.isUsing == false)
            yield return 0;
        while(mComputerScript.mActiveCanvas != mComputerScript.mCanvas["CanvasHome"])
            yield return 0;
        mComputerScript.ChangeInterface("CanvasOrders");
        mMailCanva.SelectOrder(mPackages[packageIndex].mTargetMail);
    }
}
