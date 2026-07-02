using UnityEngine;
using System.Collections.Generic;

public class Inventory_Station : MonoBehaviour
{
    private Camera_DepthView mCameraDepth = null;
    private InteractiveAreaScript mIArea = null;

    private PackageInventory mPackInv = null;
    private PackageInventory_UIManager mUI = null;

    private List<PackageScript> mPackages = new List<PackageScript>();
    private int mLastPackageNumber = 0;



    void Start()
    {
        mCameraDepth = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().GetControllerMod<Camera_DepthView>();
        mIArea = transform.Find("InteractiveArea").GetComponent<InteractiveAreaScript>();

        mPackInv = transform.Find("SM_Tray").GetComponent<PackageInventory>();
        mUI = transform.Find("ManagerUI").GetComponent<PackageInventory_UIManager>();

        mPackInv.LinkPackageList(mPackages);
        mUI.LinkPackageList(mPackages);
    }

    void Update()
    {
        mUI.gameObject.SetActive(mIArea.isUsing && (mCameraDepth.IsMoving(0.00001f) == false));
        if(mLastPackageNumber != mPackages.Count) RefreshPackage();
    }

    public void AddPackage(PackageScript package)
    {
        mPackages.Add(package);
        mPackInv.AddPackage(package);
        mUI.AddPackage(package);
    }

    private void RefreshPackage()
    {
        mPackInv.RefreshPackage();
        mUI.RefreshPackage();
        mLastPackageNumber = mPackages.Count;
    }
}
