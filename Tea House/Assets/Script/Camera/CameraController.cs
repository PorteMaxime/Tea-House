using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    //#region Singleton
    //private static CameraController mInstance = null;
    //public static CameraController Get()
    //{
    //    if(mInstance == null)
    //        mInstance = new CameraController();
    //    return mInstance;
    //}
    //#endregion

    private Input_PlayerMovement mInputPlayerMovement;
    private Input_PlayerInteract mInputPlayerInteract;
    private Camera mCamera;

    public GameObject mPlayer;
    public GameObject mInteractionPopup;

    private List<GameObject> zoomTargetOrder = new List<GameObject>();
    private Vector3 zoomOffset = new Vector3(0f, 2f, 0f);

    // Camera Slerp
    private List<Transform> mDepths = new List<Transform>();
    private float mMoveSpeed = 5f;
    private float mRotationSpeed = 5f;
    private float mTargetRotation = 0f;
    private float mRotationAmount = 90f;

    // Camera State
    public enum CameraStates
    {
        orbitale,
        zoom
    }
    public CameraStates mState = CameraStates.orbitale;

    #region DragAndDrop
    // Drag & Drop
    public GameObject mHolding = null;              // GameObject holded
    private bool mHoldingWasKinematic = false;      // Before dragging, mHolding was kinematic ?
    public bool mHasReleasedDrag = false;           // Gameobject is released last frame ?
    private float mCameraDistance;                  // Distance between camera and gameobject

    public void StartDragging(GameObject target)
    {
        if (target == null) return;

        mHolding = target;

        mCameraDistance = mCamera.WorldToScreenPoint(mHolding.transform.position).z;

        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            mHoldingWasKinematic = rb.useGravity;
            rb.isKinematic = false;
        }
    }
    public void StopDragging()
    {
        if (mHolding == null) return;

        Rigidbody rb = mHolding.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = mHoldingWasKinematic;

        mHolding = null;
        mHasReleasedDrag = false;
    }
    public bool IsDragging()
    {
        return mHolding != null;
    }

    #endregion

    #region Raycast
    // Interaction Raycast => For minigames
    private List<GameObject> mHits = new List<GameObject>();
    private RaycastHit[] mRaycastHits = new RaycastHit[50];

    public List<GameObject> GetRaycastHit(LayerMask[] layers, string[] tags)
    {
        if(IsDragging() == true ) return new List<GameObject>(); // Can't raycast if dragging an object

        List<GameObject> gOs = new List<GameObject>();
        int totalLayers = 0;

        if (layers != null)
        {
            foreach (LayerMask layer in layers)
                totalLayers |= layer.value;
        }

        bool hasLayerFilter = layers != null && layers.Length > 0;
        bool hasTagFilter = tags != null && tags.Length > 0;

        foreach (GameObject go in mHits)
        {
            if (!hasLayerFilter || ((1 << go.layer) & totalLayers) != 0)
            {
                if (hasTagFilter == false)
                {
                    gOs.Add(go);
                }
                else
                {
                    for (int i = 0; i < tags.Length; i++)
                    {
                        if (go.CompareTag(tags[i]))
                        {
                            gOs.Add(go);
                            break;
                        }
                    }
                }
            }
        }

        return gOs;
    }
    public List<GameObject> GetRaycastHit(LayerMask layer, string tag)
    {
        return GetRaycastHit(new LayerMask[1] { layer }, new string[1] { tag });
    }
    public List<GameObject> GetRaycastHit(LayerMask layer, string[] tags)
    {
        return GetRaycastHit(new LayerMask[1] { layer }, tags);
    }
    public List<GameObject> GetRaycastHit(LayerMask[] layers, string tag)
    {
        return GetRaycastHit(layers, new string[1] { tag });
    }
    #endregion

    #region Trash

    public void AddInteractionTarget(GameObject target)
    {
        zoomTargetOrder.Add(target);
    }
    public void RemoveInteractionTarget(GameObject target)
    {
        zoomTargetOrder.Remove(target);

        if (zoomTargetOrder.Count <= 0)
        {
            Back();
            SetState(CameraController.CameraStates.orbitale);
            StopDragging();
            mPlayer.GetComponent<CharacterInput>().mCanMove = true;
            mPlayer.GetComponent<MeshRenderer>().enabled = true;
        }
    }
    public bool CanInteractWith(GameObject target)
    {
        return zoomTargetOrder[zoomTargetOrder.Count - 1] == target;
    }
    public void SetInteractionImagePosition(GameObject target)
    {
        mInteractionPopup.SetActive(true);
        mInteractionPopup.transform.position = Camera.main.WorldToScreenPoint(target.transform.parent.position) + zoomOffset * 30;
    }

    #endregion


    void Awake()
    {
        mInputPlayerMovement = InputManager.Get().mPlayerMovement;
        mInputPlayerInteract = InputManager.Get().mPlayerInteract;
        mCamera = GetComponent<Camera>();
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

        mPlayer = GameObject.FindGameObjectWithTag("Player");

        mInteractionPopup.SetActive(true);
        mInteractionPopup = GameObject.FindGameObjectWithTag("InteractionImage");
        mInteractionPopup.SetActive(false);
    }
    void Update()
    {
        if (mDepths.Count > 0)
        {
            if (IsMoving(0.00001f) == false)
                transform.position = mDepths[mDepths.Count - 1].position;
            else
                transform.position = Vector3.Slerp(transform.position, mDepths[mDepths.Count - 1].position, mMoveSpeed * Time.deltaTime);

            if (IsRotating(0.00001f) == false)
                transform.rotation = mDepths[mDepths.Count - 1].rotation;
            else
                transform.rotation = Quaternion.Slerp(transform.rotation, mDepths[mDepths.Count - 1].rotation, mRotationSpeed * Time.deltaTime);
        }

        if (mState == CameraStates.zoom) ZoomedUpdate();
        else if (mState == CameraStates.orbitale) OrbitalUpdate();
    }

    private void ZoomedUpdate()
    {
        mHits.Clear();

        if (mInputPlayerInteract.Player.Select.IsPressed())
        {
            Ray ray = mCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            int hitCount = Physics.RaycastNonAlloc(ray, mRaycastHits, Mathf.Infinity, Physics.AllLayers);

            for (int i = 0; i < hitCount; i++)
                mHits.Add(mRaycastHits[i].collider.gameObject);
        }

        if (mHolding != null)
        {
            if (mHasReleasedDrag == true)
                StopDragging();
            if (mInputPlayerInteract.Player.Select.IsPressed() == false && mHolding != null)
                mHasReleasedDrag = true;
            else
            {
                if (mHolding != null)
                {
                    Vector3 mousePos = Mouse.current.position.ReadValue();
                    mousePos.z = mCameraDistance;
                    Vector3 worldPos = mCamera.ScreenToWorldPoint(mousePos);
                    worldPos.x = mHolding.transform.position.x;
                    mHolding.transform.position = worldPos;
                }
            }
        }
    }
    void OrbitalUpdate()
    {
        if (zoomTargetOrder.Count > 0)
        {
            SetInteractionImagePosition(zoomTargetOrder[zoomTargetOrder.Count - 1]);
        }
        else
        {
            mInteractionPopup.SetActive(false);
        }

        if (mInputPlayerMovement.Camera.Rotation.triggered)
        {
            float axis = -mInputPlayerMovement.Camera.Rotation.ReadValue<float>();
            mTargetRotation += axis * mRotationAmount;

            if (mDepths.Count == 1)
            {
                Destroy(mDepths[mDepths.Count - 1].gameObject);
                mDepths.RemoveAt(mDepths.Count - 1);
            }
        }
        transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, Quaternion.Euler(0, mTargetRotation, 0), mRotationSpeed * Time.deltaTime);
    }

    public void GoTo(Transform otherTransform)
    {
        if (mDepths.Count == 0)
        {
            GameObject rootSave = new GameObject("save of " + transform.name);
            rootSave.transform.SetPositionAndRotation(transform.position, transform.rotation);
            mDepths.Add(rootSave.transform);
        }

        GameObject save = new GameObject("save of " + otherTransform.name);
        save.transform.SetPositionAndRotation(otherTransform.position, otherTransform.rotation);
        mDepths.Add(save.transform);
    }
    public void Back()
    {
        if (mDepths.Count <= 1)
            return;
        Destroy(mDepths[mDepths.Count - 1].gameObject);
        mDepths.RemoveAt(mDepths.Count - 1);
    }
    public bool IsMoving(float precision)
    {
        return mDepths.Count > 0 ? Vector3.Distance(transform.position, mDepths[mDepths.Count - 1].position) > precision : false;
    }
    public bool IsRotating(float precision)
    {
        return mDepths.Count > 0 ? Quaternion.Angle(transform.rotation, mDepths[mDepths.Count - 1].rotation) > precision : false;
    }

    public void SetState(CameraStates state) => mState = state;
    public CameraStates GetState()
    {
        return mState;
    }
}