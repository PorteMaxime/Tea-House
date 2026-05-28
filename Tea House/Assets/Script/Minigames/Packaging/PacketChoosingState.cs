using System.Collections.Generic;
using UnityEngine;

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


    private GameObject mPackagePrefab = null;
    private Transform mWorkingPoint = null;

    private void Init()
    {
        mPackagePrefab = Resources.Load<GameObject>("Items/Package");
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
        List<GameObject> packages = mMain.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Package");
        if (packages.Count > 0)
        {
            GameObject package = mMain.Instantiate(mPackagePrefab, mWorkingPoint.position, Quaternion.identity);

            mMain.ShareRender(packages[0], package);

            //mMain.mOutput = package.GetComponent<PackageScript>();
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
        }
    }
}
