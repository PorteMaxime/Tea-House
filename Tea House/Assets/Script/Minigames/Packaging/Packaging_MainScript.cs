using NUnit.Framework;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Packaging_MainScript : MonoBehaviour
{
    [SerializeField] private GameObject mCamera = null;
    private CameraController mCameraController = null;

    private enum State
    {
        Idle,

        PacketChoosing,
        Completing,
        Finish,

        Count
    }
    private State mState = State.Idle;
    private bool mIsLabelled = false;
    private bool mIsFilled = false;

    private IngredientData mData = null;

    private GameObject mWorkingPoint = null;
    private WorkingPointCollider mWorkingPointCollider = null;

    // OUTPUT
    private PackageScript mOutput = null;

    // RESOURCES
    private GameObject mPackagePrefab = null;


    private void Start()
    {
        mCameraController = mCamera.GetComponent<CameraController>();
        mPackagePrefab = Resources.Load<GameObject>("Items/Package");
        mWorkingPoint = transform.Find("WorkingPoint").gameObject;
        mWorkingPointCollider = mWorkingPoint.GetComponent<WorkingPointCollider>();

        // Temporary for waiting Workbench
        SetIngredientData(new IngredientData());
    }

    private void Update()
    {
        if (mData == null || mCamera == null) return;
        if (mState == State.PacketChoosing) SelectPackage();
        if (mState == State.Completing) CompletingUpdate();
    }
    
    private void CompletingUpdate()
    {
        if (mIsLabelled == false) SelectLabel();
        if (mIsFilled == false) FillPackage();
    }

    public void SetIngredientData(IngredientData _ingredientData)
    {
        mData = _ingredientData;
        mState = State.PacketChoosing;
    }

    private void SelectPackage()
    {
        List<GameObject> packages = mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Package");
        if(packages.Count > 0)
        {
            GameObject package = Instantiate(mPackagePrefab, mWorkingPoint.transform.position, Quaternion.identity);

            MeshFilter mesh = package.AddComponent<MeshFilter>();
            mesh.sharedMesh = packages[0].GetComponent<MeshFilter>().mesh;
            MeshRenderer meshRenderer = package.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterials = packages[0].GetComponent<MeshRenderer>().materials;

            mOutput = package.GetComponent<PackageScript>();

            mState = State.Completing;
        }
    }
    private void SelectLabel()
    {
        // Screen + Drag&drop label + glue
    }
    private void FillPackage()
    {
        if (mIsFilled == true) return;

        // Drag&drop bowl
        List<GameObject> hitDraggables = mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Ingredient");

        if (hitDraggables.Count > 0 && mCameraController.mHolding == null)
        {
            mCameraController.StartDragging(hitDraggables[0]);
        }

        if(mWorkingPointCollider.mCollider != null && mWorkingPointCollider.mCollider.tag == "Ingredient")
        {
            mOutput.mIngredient = mData;
            mIsFilled = true;
            Destroy(mWorkingPointCollider.mCollider);
            mOutput.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }
}
