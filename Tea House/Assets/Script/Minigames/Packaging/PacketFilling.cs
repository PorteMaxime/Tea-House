using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

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
    bool isFilled = false;

    public GameObject mBowl = null;
    private Vector3 mOldPosition = Vector3.zero;

    private float mBowlAngle = 70f;
    private float mRotationSpeed = 5f;
    // private Vector3 mRotationStartPos = Vector3.zero;
    // private Vector3 mRotationEndPos = Vector3.zero;

    private Input_PlayerInteract mPlayerInteract = null;
    private CameraDragAndDrop mDragAndDrop = null;

    private Vector4 mZBorns = Vector4.zero; // (bowlOriginZ, distanceBetweenBowlOriginAndPackage, nearestPackageCornerFromBowlOrigin, packageCenterZ)
    private Vector4 mOppositeZBorns = Vector4.zero; // Mirrored values of the ZBorns => mirroring point is the package center z
    private float mYBorn = 0f;
    private Collider mBowlCollider = null;

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
    }
    public void OnEnter()
    {
        if(mDragAndDrop.StartDragging(mBowl) == false) mMain.mStateMachine.SwitchState(mMain.mCompleting);

        mBowl.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        Collider packageCollider = mMain.mOutputGO.GetComponent<Collider>();
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
        mMain.mCompleting.mIsFilled = isFilled;
        mBowl.transform.rotation = Quaternion.identity;
        mBowl.transform.position = mOldPosition;
        mBowl.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }

    public void OnUpdate()
    {
        if(mPlayerInteract.Player.Select.WasReleasedThisFrame() == true)
            mDragAndDrop.StopDragging();

        if (mDragAndDrop.IsDragging() == false)
            mMain.mStateMachine.SwitchState(mMain.mCompleting);

        if(mBowlCollider == null) return;

        if(mBowlCollider.bounds.min.y < mYBorn) return;
        if(mBowlCollider.bounds.max.z < mZBorns.w)
        {
            // B to B
            float percentage = 1 - (mZBorns.w - mBowlCollider.bounds.max.z) / mZBorns.y;
            if(percentage > 1f) percentage = 1f;
            else if(percentage < 0f) percentage = 0f;

            float targetRotation = mBowlAngle * percentage;
            mBowl.transform.rotation = Quaternion.Lerp(mBowl.transform.rotation, Quaternion.Euler(targetRotation, 0f, 0f), mRotationSpeed * Time.deltaTime);
        }
        // else if(mBowlCollider.bounds.min.z > mOppositeZBorns.w)
        // {
        //     // O to O
        //     float percentage = 1 - (mBowlCollider.bounds.min.z - mOppositeZBorns.w) / mOppositeZBorns.y;Debug.Log(percentage);
        //     if(percentage > 1f) percentage = 1f;
        //     else if(percentage < 0f) percentage = 0f;

        //     float targetRotation = -mBowlAngle * percentage;
        //     mBowl.transform.rotation = Quaternion.Lerp(mBowl.transform.rotation, Quaternion.Euler(targetRotation, 0f, 0f), mRotationSpeed * Time.deltaTime);
        // }
        else
        {
            // B to O
            mBowl.transform.rotation = Quaternion.Lerp(mBowl.transform.rotation, Quaternion.identity, mRotationSpeed * Time.deltaTime);
        }
    }

    private IEnumerator HomeSweetHome() // Bring back Bowl
    {
        // TODO : Bring back the bowl to its original position and rotation
        yield return 0f;
    }
}