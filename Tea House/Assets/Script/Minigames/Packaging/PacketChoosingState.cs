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

    private bool mInitialized = false;
    #endregion

    private Input_PlayerInteract mPlayerInteract = null;

    private Transform mWorkingPoint = null;

    private Camera_Raycast mRaycast = null;

    private GameObject mPackageSlot = null;
    private List<GameObject> mPackages = new List<GameObject>();

    public GameObject mChoosedPackage = null;

    private void Init()
    {
        mWorkingPoint = mMain.transform.Find("WorkingPoint").transform;

        mPlayerInteract = InputManager.Get().mPlayerInteract;
        mPlayerInteract.Enable();

        mRaycast = mMain.mCameraController.GetControllerMod<Camera_Raycast>();
        mPackageSlot = mMain.transform.Find("Packages").gameObject;

        foreach (Transform package in mPackageSlot.transform)
            mPackages.Add(package.gameObject);

        mInitialized = true;
    }

    public void OnEnter()
    {
        if (mChoosedPackage != null)
        {
            GameObject package = mMain.Instantiate(mChoosedPackage.transform.parent.gameObject, mWorkingPoint.position, Quaternion.identity);

            //mMain.ShareRender(packages[0], package);
            package.transform.parent = mMain.transform;
            package.transform.localScale *= 100f;
            mMain.mOutputGO = package;

            Transform lid = package.transform.Find("Lid");
            if(lid != null)
            {    
                GameObject packageLid = lid.gameObject;
                if(packageLid != null && packageLid.GetComponent<Rigidbody>() != null)
                {
                    Rigidbody rb = packageLid.AddComponent<Rigidbody>();
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

    public void OnUpdate()
    {
        
    }
}
