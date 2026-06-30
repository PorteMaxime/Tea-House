using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PacketChoosingState : IPackaging
{
    #region Main
    private PackagingScript mMain;
    public PacketChoosingState(PackagingScript main)
    {
        mMain = main;
        Init();
    }
    #endregion

    private Input_PlayerInteract mPlayerInteract = null;

    private Transform mWorkingPoint = null;

    public GameObject mChoosedPackage = null;

    private void Init()
    {
        mWorkingPoint = mMain.transform.Find("WorkingPoint").transform;

        mPlayerInteract = InputManager.Get().mPlayerInteract;
        mPlayerInteract.Enable();
    }

    public void OnEnter()
    {
        if (mChoosedPackage != null)
        {
            if(mMain.mOutputGO != null)
            {
                mMain.mInventory.ChangeItemAmount(mMain.mOutputGO.GetComponent<Associated_SO>().mItem, 1);
                GameObject.Destroy(mMain.mOutputGO);
            }

            GameObject package = GameObject.Instantiate(mChoosedPackage.transform.parent.gameObject, mWorkingPoint.position, Quaternion.identity);
            mMain.mInventory.ChangeItemAmount(mChoosedPackage.transform.parent.GetComponent<Associated_SO>().mItem, -1);

            package.transform.parent = mMain.transform;
            package.transform.localScale *= 100f;
            mMain.mOutputGO = package;
            mMain.mOutput = package.GetComponent<PackageScript>();

            Transform lid = package.transform.Find("Lid");
            if(lid != null)
            {    
                GameObject packageLid = lid.gameObject;
                if(packageLid != null)
                {
                    Rigidbody rb = packageLid.GetComponent<Rigidbody>();
                    if(rb == null) 
                        rb = packageLid.AddComponent<Rigidbody>();
                    rb.useGravity = true;
                    rb.isKinematic = true;
                }

                if (packageLid != null && packageLid.GetComponent<WorkingPoint>() == null)
                    packageLid.AddComponent<WorkingPoint>();
            }
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
        }
    }

    public void OnExit()
    {
        mChoosedPackage = null;
    }

    public void OnReset()
    {
    }

    public void OnUpdate()
    {
        
    }
}
