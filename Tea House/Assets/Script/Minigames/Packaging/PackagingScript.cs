using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PackagingScript : MonoBehaviour
{
    #region StateMachine
    public PackagingStateMachine mStateMachine;

    public PacketChoosingState mChoosing = null;
    public PacketOpening mOpening = null;
    public PacketCompleting mCompleting = null;
    public PacketFilling mFilling = null;
    public PacketLabelling mLabelling = null;
    public PacketLabelPrinting mPrinting = null;
    #endregion

    private InteractiveAreaScript mIArea = null;
    public CameraController mCameraController = null;

    private Dictionary<PackageData, List<GameObject>> mVisualPackages = new Dictionary<PackageData, List<GameObject>>();
    public Inventory mInventory = null;

    public PackageScript mOutput = null;
    public GameObject mOutputGO = null;

    public List<ItemData> mDebugList = new List<ItemData>();

    public MailSystem mMailSystem = null;

    void Start()
    {
        mCameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        mMailSystem = GameObject.FindGameObjectWithTag("MailManager").GetComponent<MailSystem>();
        mIArea = transform.Find("InteractiveArea").GetComponent<InteractiveAreaScript>();
        
        mInventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        Transform visualGeneralPackage = transform.Find("Packages");
        foreach(Transform packageType in visualGeneralPackage)
        {
            List<GameObject> packagesGOs = new List<GameObject>();
            foreach(Transform package in packageType)
                packagesGOs.Add(package.gameObject);
            Associated_SO packageSO = packageType.GetComponent<Associated_SO>();
            if(packageSO != null && packageSO.mItem != null && packageSO.mItem is PackageData)
                mVisualPackages[(PackageData)packageSO.mItem] = packagesGOs;
        }
        
        mStateMachine = new PackagingStateMachine();
        
        mChoosing = new PacketChoosingState(this);
        mOpening = new PacketOpening(this);
        mCompleting = new PacketCompleting(this);
        mFilling = new PacketFilling(this);
        mLabelling = new PacketLabelling(this);
        mPrinting = new PacketLabelPrinting(this);

        mStateMachine.SwitchState(mCompleting);

        mPrinting.mMixture = mDebugList;
        mFilling.mMixture = mDebugList;
    }
    void Update()
    {
        foreach(PackageData item in mVisualPackages.Keys)
        {
            int amount = mInventory.GetAmount(item);
            for(int i = 0; i < amount; i++)
            {
                if(i >= mVisualPackages[item].Count)
                    break;
                mVisualPackages[item][i].SetActive(true);
            }
            for(int i = amount; i < mVisualPackages[item].Count; i++)
                mVisualPackages[item][i].SetActive(false);
        }

        if(mIArea.isUsing == true)
            mStateMachine.Update();
    }

    public void SetIngredientData(List<ItemData> data)
    {
        // TODO : Take the ingredient data from Workbench
        GameObject.FindGameObjectWithTag("MixtureBowl").transform.parent = transform;
        mPrinting.mMixture = data;
        mFilling.mMixture = data;
    }

    public void ResetFSM()
    {
        mChoosing.OnReset();
        mOpening.OnReset();
        mCompleting.OnReset();
        mFilling.OnReset();
        mLabelling.OnReset();
        mPrinting.OnReset();

        mStateMachine.SwitchState(mCompleting);
    }

    public void ThrowLabel() => mCompleting.ThrowLabel();

    public void ExportPackage() => mCompleting.ExportPackage();
}
