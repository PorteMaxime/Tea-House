using System.Collections.Generic;
using UnityEngine;

public class PackageInventory : MonoBehaviour
{
    private List<Transform> mSlots = new List<Transform>();
    private List<PackageScript> mPackages = new List<PackageScript>();


    void Start()
    {
        foreach(Transform slot in transform)
            mSlots.Add(slot);
    }

    public void AddPackage(PackageScript package)
    {
        mPackages.Add(package);
        package.transform.parent = transform;
        RefreshPackage();
    }
    private void RefreshPackage()
    {
        for(int i = 0; i < mPackages.Count; i++)
        {
            if(mPackages[i] == null)
                mPackages.RemoveAt(i);
            else
                mPackages[i].transform.SetPositionAndRotation(mSlots[i].position, mSlots[i].rotation);
        }
    }
}
