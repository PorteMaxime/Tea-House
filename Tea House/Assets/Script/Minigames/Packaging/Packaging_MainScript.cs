using NUnit.Framework;
using System.Collections.Generic;
using TreeEditor;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.InputSystem;

public class Packaging_MainScript : MonoBehaviour
{
    [SerializeField] private GameObject mCamera = null;
    private CameraController mCameraController = null;
    private Input_PlayerInteract mInputPlayerInteract = null;

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

    private void Awake()
    {
        mInputPlayerInteract = InputManager.Get().mPlayerInteract;
    }
    private void Start()
    {
        mCameraController = mCamera.GetComponent<CameraController>();
        mPackagePrefab = Resources.Load<GameObject>("Items/Package");
        mWorkingPoint = transform.Find("WorkingPoint").gameObject;
        mWorkingPointCollider = mWorkingPoint.GetComponent<WorkingPointCollider>();

        mBowlOrigin = transform.Find("MixtureBowl").transform.position;
        mBowlOriginDistance = Vector3.Distance(mBowlOrigin, mWorkingPoint.transform.position);

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

    #region Label
    private bool mIsOnPrinter = false;

    private void SelectLabel()
    {
        // Screen + Drag&drop label + glue
        if (mIsLabelled == true) return;

        List<GameObject> hitDraggables = mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Printer");

        if(hitDraggables.Count > 0 && mIsOnPrinter == false)
        {
            mCameraController.GoTo(hitDraggables[0].transform.Find("ViewPoint").transform);
            mInputPlayerInteract.Player.Open.Disable();
            mIsOnPrinter = true;
        }
        if(mIsOnPrinter == true && mInputPlayerInteract.Player.Close.triggered == true)
        {
            mCameraController.Back();
            mInputPlayerInteract.Player.Open.Enable();
            mIsOnPrinter = false;
        }
    }
    #endregion

    #region Package
    private GameObject mBowl = null;              // Bowl reference
    private Vector3 mBowlOrigin = Vector3.zero;         // Bowl Origin position
    private float mBowlOriginDistance = 0f;             // Bowl Origin distance from WorkingPoint
    private float mTimeToOrigin = 0.5f;                 // Time in which the Bowl need to be at his Origin
    private Vector3 mBowlSpeed = Vector3.zero;          // Bowl Speed during movement to Origin

    private void FillPackage()
    {
        if(mBowl != null)
        {
            float dist = Vector3.Distance(mWorkingPoint.transform.position, mBowl.transform.position);
            float currentAngle = 360f * Mathf.InverseLerp(-mBowlOriginDistance, 0f, -dist);

            mBowl.transform.rotation = Quaternion.Euler(currentAngle, 0f, 0f);
        }
        if (mIsFilled == true) return;

        // Drag&drop bowl
        List<GameObject> hitDraggables = mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Ingredient");

        if (hitDraggables.Count > 0 && mCameraController.mHolding == null)
        {
            mBowl = hitDraggables[0];
            mCameraController.StartDragging(hitDraggables[0]);
        }
        if (mBowl != null && mCameraController.mHolding == null)
            mBowl.transform.position = Vector3.SmoothDamp(mBowl.transform.position, mBowlOrigin, ref mBowlSpeed, mTimeToOrigin);

        if (mWorkingPointCollider.mCollider != null && mWorkingPointCollider.mCollider.tag == "Ingredient")
        {
            mOutput.mIngredient = mData;
            mIsFilled = true;
            //Destroy(mWorkingPointCollider.mCollider);
            mOutput.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }
    #endregion
}
