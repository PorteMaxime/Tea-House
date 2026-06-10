using System.Collections.Generic;
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

    private Transform mWorkingPoint = null;

    private void Init()
    {
        mWorkingPoint = mMain.transform.Find("WorkingPoint").transform;

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

        // TODO : GetRaycastHit
        // List<GameObject> packages = mMain.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Package");
        // if (packages.Count > 0)
        // {
        //     GameObject package = mMain.Instantiate(packages[0].transform.parent.gameObject, mWorkingPoint.position, Quaternion.identity);

        //     //mMain.ShareRender(packages[0], package);
        //     package.transform.parent = mMain.transform;
        //     package.transform.localScale = Vector3.one;
        //     mMain.mOutputGO = package;

        //     GameObject packageLid = package.transform.Find("Lid").gameObject;
        //     if(packageLid != null && packageLid.GetComponent<Rigidbody>() == null)
        //     {
        //         Rigidbody rb = packageLid.AddComponent<Rigidbody>();
        //         rb.useGravity = true;
        //         rb.isKinematic = true;
        //     }

        //     if (packageLid != null && packageLid.GetComponent<WorkingPoint>() == null)
        //         packageLid.AddComponent<WorkingPoint>();

        //     //mMain.mOutput = package.GetComponent<PackageScript>();
        //     mMain.mStateMachine.SwitchState(mMain.mCompleting);
        // }
    }
}
