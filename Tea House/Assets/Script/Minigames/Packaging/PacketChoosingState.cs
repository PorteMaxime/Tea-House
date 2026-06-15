using System.Collections.Generic;
using System.ComponentModel;
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
        
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
        if (mInitialized == false) Init();

        if (mPlayerInteract.Player.Select.triggered == false) return;

        GameObject currentPackage = null;
        mRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "Package", out currentPackage);

        if (currentPackage != null)
        {
            GameObject package = mMain.Instantiate(currentPackage.transform.parent.gameObject, mWorkingPoint.position, Quaternion.identity);

            //mMain.ShareRender(packages[0], package);
            package.transform.parent = mMain.transform;
            package.transform.localScale = Vector3.one;
            mMain.mOutputGO = package;

            GameObject packageLid = package.transform.Find("Lid").gameObject;
            if(packageLid != null && packageLid.GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = packageLid.AddComponent<Rigidbody>();
                rb.useGravity = true;
                rb.isKinematic = true;
            }

            if (packageLid != null && packageLid.GetComponent<WorkingPoint>() == null)
                packageLid.AddComponent<WorkingPoint>();

            //mMain.mOutput = package.GetComponent<PackageScript>();
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
        }
    }
}
