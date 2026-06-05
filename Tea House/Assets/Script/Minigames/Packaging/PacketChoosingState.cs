using System.Collections.Generic;
using System.Numerics;
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

        List<GameObject> packages = mMain.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Package");
        if (packages.Count > 0)
        {
            GameObject package = mMain.Instantiate(packages[0].transform.parent.gameObject, mWorkingPoint.position, Quaternion.identity);

            //mMain.ShareRender(packages[0], package);
            package.transform.parent = mMain.transform;
            package.transform.localScale = Vector3.one;
            mMain.mOutputGO = package;

            BoxCollider collider = package.AddComponent<BoxCollider>();

            Physics.SyncTransforms();

            GameObject debug1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            debug1.GetComponent<MeshRenderer>().material.color = Color.red;
            debug1.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            debug1.transform.position = collider.bounds.min;
            GameObject debug2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            debug2.GetComponent<MeshRenderer>().material.color = Color.cyan;
            debug2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            debug2.transform.position = collider.bounds.max;
            collider.enabled = false;

            //mMain.mOutput = package.GetComponent<PackageScript>();
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
        }
    }
}
