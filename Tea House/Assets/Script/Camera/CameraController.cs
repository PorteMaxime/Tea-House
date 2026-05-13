using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CameraController : MonoBehaviour
{
    private Vector3 mCurrentPos = Vector3.zero;
    private Quaternion mCurrentRot = Quaternion.identity;

    private GameObject mPlayer;

    private Input_PlayerMovement mInputPlayerMovement;
    private Input_PlayerInteract mInputPlayerInteract;

    private GameObject mInteractionPopup;

    private List<GameObject> zoomTargetOrder = new List<GameObject>();
    private Vector3 zoomOffset = new Vector3(0f, 2f, 0f);
    [SerializeField] private float mRotationAmount;
    [SerializeField] private float mRotationSpeed;
    private float mTargetRotation = 0f;

    public enum CameraStates
    {
        orbitale,
        zoom
    }
    public CameraStates mState = CameraStates.orbitale;

    void Awake()
    {
        mInputPlayerMovement = new Input_PlayerMovement();
        mInputPlayerInteract = new Input_PlayerInteract();
    }
    void OnEnable()
    {
        mInputPlayerMovement.Enable();
        mInputPlayerInteract.Enable();
    }
    void OnDisable()
    {
        mInputPlayerMovement.Disable();
        mInputPlayerInteract.Disable();
    }
    void Start()
    {
        mTargetRotation = transform.parent.rotation.eulerAngles.y;

        mCurrentPos = transform.position;
        mCurrentRot = transform.rotation;

        mPlayer = GameObject.FindGameObjectWithTag("Player");

        mInteractionPopup = GameObject.FindGameObjectWithTag("InteractionImage");
        mInteractionPopup.SetActive(false);
    }
    void Update()
    {
        if (mState == CameraStates.zoom)
        {
            return;//
        }
        else if (mState == CameraStates.orbitale)
        {
            if (zoomTargetOrder.Count > 0)
            {
                SetInteractionImagePosition(zoomTargetOrder[zoomTargetOrder.Count - 1]);
            }
            else
            {
                mInteractionPopup.SetActive(false);
            }
        }


            transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, Quaternion.Euler(0, mTargetRotation, 0), mRotationSpeed * Time.deltaTime);

        if (mInputPlayerMovement.Camera.Rotation.triggered)
        {
            float axis = mInputPlayerMovement.Camera.Rotation.ReadValue<float>();
            mTargetRotation += axis * mRotationAmount;
            //Debug.Log("TargetRot: " + mTargetRotation);
            //mCurrentPos = transform.position;
            //mCurrentRot = transform.rotation;
        }
    }

    public void SetCameraZoomState(GameObject target)
    {
        transform.position = mPlayer.transform.position + zoomOffset;
        transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        mState = CameraStates.zoom;

        mInteractionPopup.SetActive(false);
    }

    public void AddInteractionTarget(GameObject target)
    {
        zoomTargetOrder.Add(target);

    }
    public void RemoveInteractionTarget(GameObject target)
    {
        zoomTargetOrder.Remove(target);
    }
    public bool CanInteractWith(GameObject target)
    {
        return zoomTargetOrder[zoomTargetOrder.Count - 1] == target;
    }
    public void SetInteractionImagePosition(GameObject target)
    {
        mInteractionPopup.SetActive(true);
        mInteractionPopup.transform.position = Camera.main.WorldToScreenPoint(target.transform.position) + zoomOffset * 30;
    }

    public void SetCameraOrbitaleState()
    {
        transform.position = mCurrentPos;
        transform.rotation = mCurrentRot;
        mState = CameraStates.orbitale;
    }
    public CameraStates GetState()
    {
        return mState;
    }
}