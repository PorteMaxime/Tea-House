using System.Collections.Generic;
using UnityEngine;

public class PackageInventory : MonoBehaviour
{
    private List<Transform> mSlots = new List<Transform>();
    private List<PackageScript> mPackages = null;


    void Start()
    {
        foreach(Transform slot in transform)
            mSlots.Add(slot);
    }

    public void LinkPackageList(List<PackageScript> packageList) => mPackages = packageList;

    public void AddPackage(PackageScript package)
    {
        package.transform.parent = transform;
        RefreshPackage();
    }
    public void RefreshPackage()
    {
        for(int i = 0; i < mPackages.Count; i++)
        {
            if(mPackages[i] != null)
                mPackages[i].transform.SetPositionAndRotation(mSlots[i].position, mSlots[i].rotation);
        }
    }
}