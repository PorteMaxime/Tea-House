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
    private Vector3 mRotationStartPos = Vector3.zero;
    private Vector3 mRotationEndPos = Vector3.zero;

    private Input_PlayerInteract mPlayerInteract = null;
    private CameraDragAndDrop mDragAndDrop = null;

    private void Init()
    {
        mDragAndDrop = mMain.mCameraController.GetControllerMod<CameraDragAndDrop>();
        mPlayerInteract = InputManager.Get().mPlayerInteract;
        mPlayerInteract.Enable();
    }
    public void OnEnter()
    {
        mOldPosition = mBowl.transform.localPosition;
        if(mDragAndDrop.StartDragging(mBowl) == false) mMain.mStateMachine.SwitchState(mMain.mCompleting);

        BoxCollider packageCollider = mMain.mOutputGO.transform.Find("Base").GetComponent<BoxCollider>();
        Vector3 minBound = packageCollider.bounds.min;
        Vector3 maxBound = packageCollider.bounds.max;

        mBowl.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        mRotationStartPos = new Vector3(0, maxBound.y, maxBound.z + mBowl.transform.position.z);
        mRotationEndPos = new Vector3(0, 0, minBound.z - mBowl.transform.position.z);
    }

    public void OnExit()
    {
        mMain.mCompleting.mIsFilled = isFilled;
        mBowl.transform.localPosition = mOldPosition;
        mBowl.transform.rotation = Quaternion.identity;
    }

    public void OnUpdate()
    {
        if(mPlayerInteract.Player.Select.WasReleasedThisFrame() == true)
            mDragAndDrop.StopDragging();

        if (mDragAndDrop.IsDragging() == false)
            mMain.mStateMachine.SwitchState(mMain.mCompleting);
            
        if (mBowl.transform.position.z <= mRotationStartPos.z && mBowl.transform.position.z >= mRotationEndPos.z &&
            mBowl.transform.position.y >= mRotationStartPos.y)
        {
            float percentage = mBowl.transform.position.z >= mMain.mOutputGO.transform.position.z ? /*1 - (mBowl.transform.position.z / mRotationEndPos.z)*/0 : 1 - (mBowl.transform.position.z / mRotationEndPos.z);
            float targetRotation = -mBowlAngle * percentage;

            mBowl.transform.rotation = Quaternion.Lerp(mBowl.transform.rotation, Quaternion.Euler(targetRotation, 0f, 0f), mRotationSpeed * Time.deltaTime);
        }

        else if (mBowl.transform.position.y < mRotationStartPos.y || mBowl.transform.position.y > mRotationStartPos.y || mBowl.transform.position.z < mRotationEndPos.z)
            mBowl.transform.rotation = Quaternion.Lerp(mBowl.transform.rotation, Quaternion.identity, mRotationSpeed * Time.deltaTime);
    }

    private IEnumerator HomeSweetHome() // Bring back Bowl
    {
        // TODO : Bring back the bowl to its original position and rotation
        yield return 0f;
    }
}