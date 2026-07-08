using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

public class PacketFilling : IPackaging
{
    #region Main
    private PackagingScript mMain;
    public PacketFilling(PackagingScript main)
    {
        mMain = main;
        Init();
    }
    #endregion

    private Input_PlayerInteract mPlayerInteract = null;
    private CameraDragAndDrop mDragAndDrop = null;

    public GameObject mBowl = null;
    private Vector3 mOldPosition = Vector3.zero;

    public List<ItemData> mMixture = null;

    private readonly float mBowlAngle = 70f;
    private readonly float mRotationSpeed = 5f; 

    private Vector4 mZBorns = Vector4.zero; // (bowlOriginZ, distanceBetweenBowlOriginAndPackage, nearestPackageCornerFromBowlOrigin, packageCenterZ)
    private Vector4 mOppositeZBorns = Vector4.zero; // Mirrored values of the ZBorns => mirroring point is the package center z
    private float mYBorn = 0f;
    private Collider mBowlCollider = null;

    private GameObject mFunnel = null;
    private Coroutine mFillingCoroutine = null;

    private void Init()
    {
        mDragAndDrop = mMain.mCameraController.GetControllerMod<CameraDragAndDrop>();
        mPlayerInteract = InputManager.Get().mPlayerInteract;
        mPlayerInteract.Enable();

        Transform bowlPos = mMain.transform.Find("posMixtureBowl");
        if(bowlPos != null)
        {
            mZBorns.x = bowlPos.position.z;
            mOldPosition = bowlPos.position;
        }
        
        Transform packagePos = mMain.transform.Find("WorkingPoint");
        if(packagePos != null)
            mZBorns.w = packagePos.position.z;
        
        mZBorns.y = mZBorns.w - mZBorns.x;

        mOppositeZBorns.x = mZBorns.w + (mZBorns.w - mZBorns.x);
        mOppositeZBorns.y = mZBorns.w + (mZBorns.w - mZBorns.y);
        mOppositeZBorns.w = mZBorns.w;

        mFunnel = mMain.transform.Find("Funnel").gameObject;
        mFunnel.SetActive(false);
    }
    public void OnEnter()
    {
        if(mDragAndDrop.StartDragging(mBowl) == false) mMain.mStateMachine.SwitchState(mMain.mCompleting);

        mBowl.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        Collider packageCollider = mMain.mOutputGO.transform.GetChild(0).GetComponent<Collider>();
        if(packageCollider != null)
        {
            mZBorns.z = packageCollider.bounds.min.z;
            mOppositeZBorns.y = mZBorns.w + (mZBorns.w - mZBorns.z);
            mYBorn = packageCollider.bounds.max.y;
        }

        mBowlCollider = mBowl.GetComponent<Collider>();
    }

    public void OnExit()
    {
        mBowl.transform.SetPositionAndRotation(mOldPosition, Quaternion.identity);
        mBowl.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }

    public void OnReset()
    {
    }

    public void OnUpdate()
    {
        if(mFillingCoroutine != null) return;

        if(mPlayerInteract.Player.Select.WasReleasedThisFrame() == true)
            mDragAndDrop.StopDragging();

        if (mDragAndDrop.IsDragging() == false)
            mMain.mStateMachine.SwitchState(mMain.mCompleting);

        if(mBowlCollider == null) return;

        if(mBowl.transform.position.z < mZBorns.w && mBowlCollider.bounds.min.y >= mYBorn)
        {
            // B to B
            float percentage = 1 - (mZBorns.w - mBowlCollider.bounds.max.z) / mZBorns.y;
            if(percentage > 1f) percentage = 1f;
            else if(percentage < 0f) percentage = 0f;

            float targetRotation = mBowlAngle * percentage;
            mBowl.transform.rotation = Quaternion.Lerp(mBowl.transform.rotation, Quaternion.Euler(targetRotation, 0f, 0f), mRotationSpeed * Time.deltaTime);

            if(mBowl.transform.position.z > mZBorns.z && mMixture != null)
            {
                mDragAndDrop.StopDragging();
                mFillingCoroutine = mMain.StartCoroutine(FillPackage(mBowl, Quaternion.Euler(130f, 0, 0), 1f));
                mMain.mOutput.AddStat(mMixture);
                mMain.mCompleting.mIsFilled = true;
                mMain.mCompleting.mIsChoosed = true;
            }
        }
        else
            // B to O
            mBowl.transform.rotation = Quaternion.Lerp(mBowl.transform.rotation, Quaternion.identity, mRotationSpeed * Time.deltaTime);
    }

    private IEnumerator FillPackage(GameObject bowl, Quaternion targetRotation, float duration)
    {
        mFunnel.SetActive(true);
        mFunnel.transform.position = new Vector3(mMain.mOutputGO.transform.position.x, mMain.mOutputGO.transform.Find("LabelPlace").transform.position.y, mMain.mOutputGO.transform.position.z);
        
        bowl.transform.position = new Vector3(
            bowl.transform.position.x,
            mMain.mOutputGO.transform.position.y + (mFunnel.GetComponent<MeshCollider>().bounds.extents.y + bowl.GetComponent<MeshCollider>().bounds.extents.y * 1.25f) * 2f,
            bowl.transform.position.z);
        Tuple<BoxCollider, MeshCollider> packageColliders = new Tuple<BoxCollider, MeshCollider>(mMain.mOutputGO.transform.GetChild(0).GetComponent<BoxCollider>(), mMain.mOutputGO.transform.GetChild(0).GetComponent<MeshCollider>());
        packageColliders.Item1.enabled = false;
        packageColliders.Item2.enabled = true;

        SkinnedMeshRenderer skinnedMeshRenderer = bowl.GetComponent<SkinnedMeshRenderer>();
        if(skinnedMeshRenderer != null)
        {
            Mesh bakedMesh = new Mesh();
            skinnedMeshRenderer.BakeMesh(bakedMesh);
        
            // 2. On l'assigne au MeshCollider
            packageColliders.Item2.sharedMesh = null;
            packageColliders.Item2.sharedMesh = bakedMesh;
        }
        
        // Rotate Bowl
        float elapsedTime = 0;
        Quaternion rotationOrigine = bowl.transform.rotation;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            bowl.transform.rotation = Quaternion.Lerp(rotationOrigine, targetRotation, elapsedTime / duration);
            yield return 0;
        }

        // Enable Physic
        BoxCollider bowlBox = bowl.GetComponent<BoxCollider>();
        bowlBox.enabled = false;
        bowl.GetComponent<MeshCollider>().enabled = true;

        List<Rigidbody> childRBs = new List<Rigidbody>();
        foreach(Transform child in bowl.transform)
        {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if(child.CompareTag("WorkingPoint") == true || rb == null) continue;

            childRBs.Add(rb);
            rb.isKinematic = false;
            rb.useGravity = true;
            Collider childCollider = child.gameObject.GetComponent<Collider>();
            childCollider.isTrigger = false;
            childCollider.enabled = true;
        }
        foreach(Rigidbody rb in childRBs)
            rb.transform.parent = null;

        yield return mMain.StartCoroutine(DeleteExcessFragment(childRBs, 5f));

        // Disable Physic
        bowl.transform.rotation = targetRotation;
        bowlBox.enabled = true;

        // Destroy elements
        foreach(Rigidbody rb in childRBs) GameObject.Destroy(rb.gameObject);
        mFunnel.SetActive(false);
        packageColliders.Item1.enabled = true;
        packageColliders.Item2.enabled = false;

        mFillingCoroutine = null;
        mMain.mStateMachine.SwitchState(mMain.mCompleting);
    }

    IEnumerator DeleteExcessFragment(List<Rigidbody> fragmentRBs, float duration)
    {
        float currentTime = 0f;
        WorkingPoint wp = mFunnel.transform.Find("WorkingPoint").GetComponent<WorkingPoint>();

        while(currentTime < duration)
        {
            foreach(Rigidbody rb in fragmentRBs)
            {
                if(wp.IsColliding(rb.gameObject))
                    rb.gameObject.SetActive(false);
            }
            yield return 0f;
        }
    }
}