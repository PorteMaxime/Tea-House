using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PackageInventory_UIManager : MonoBehaviour
{
    private class UI_Slot
    {
        #region Usage Icon
        static public Color usedColor = Color.gray;
        static public Color unusedColor = Color.white;
        public Image mUsedIcon = null;
        #endregion

        #region Usage Text
        static public string unusedText = "Unused";
        public TextMeshProUGUI mOrderName = null;
        #endregion

        #region To Computer Button
        public UnityAction mToComputerListener = null;
        public Button mToComputerButton = null;
        #endregion

        #region Throw Button
        public UnityAction mThrowListener = null;
        public Button mThrowButton = null;
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

    private List<UI_Slot> mSlots = new List<UI_Slot>();
    private List<PackageScript> mPackages = null;

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

    public void AddPackage(PackageScript package)
    {
        int index = mPackages.IndexOf(package);
        if(index == -1)
        {
            Debug.Log("Error AddPackage in UI");
            return;
        }
        UI_Slot slot = mSlots[index];
        slot.Reset();
        slot.mUsedIcon.color = UI_Slot.usedColor;
        slot.mOrderName.text = package.mTargetMail.mName;

        slot.mThrowListener = () => ThrowPackage(index);
        slot.mThrowButton.onClick.AddListener(slot.mThrowListener);
        slot.mToComputerListener = () => PackageToComputer(index);
        slot.mToComputerButton.onClick.AddListener(slot.mToComputerListener);
        RefreshPackage();
    }

    public void RefreshPackage()
    {
        for(int i = mPackages.Count - 1; i >= 0; i--)
        {
            int index = i;
            if(mPackages[index] != null)
            {
                UI_Slot slot = mSlots[index];
                slot.Reset();
                slot.mUsedIcon.color = UI_Slot.usedColor;
                slot.mOrderName.text = mPackages[index].mTargetMail.mName;

                slot.mThrowListener = () => ThrowPackage(index);
                slot.mThrowButton.onClick.AddListener(slot.mThrowListener);
                slot.mToComputerListener = () => PackageToComputer(index);
                slot.mToComputerButton.onClick.AddListener(slot.mToComputerListener);
            }
            else
            {
                UI_Slot slot = mSlots[index];
                slot.Reset();
                mPackages.RemoveAt(index);
            }
        }
        for(int i = mPackages.Count; i < mSlots.Count; i++)
        {
                UI_Slot slot = mSlots[i];
                slot.Reset();
        }
    }

    public void ThrowPackage(int packageIndex)
    {
        if(packageIndex < mPackages.Count)
        {
            GameObject package = mPackages[packageIndex].gameObject;
            mPackages[packageIndex] = null;
            Destroy(package);
        }
        RefreshPackage();
    }

    public void PackageToComputer(int packageIndex)
    {
        // TODO: Send to computer => get result + send
        StartCoroutine(ToComputer(packageIndex));
    }

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
