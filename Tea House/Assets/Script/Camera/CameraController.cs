using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    private Input_PlayerMovement mInputPlayerMovement;
    private Input_PlayerInteract mInputPlayerInteract;
    private Camera mCamera;

    private Vector3 mCurrentPos = Vector3.zero;
    private Quaternion mCurrentRot = Quaternion.identity;

    private GameObject mPlayer;

    private GameObject mInteractionPopup;

    private List<GameObject> zoomTargetOrder = new List<GameObject>();
    private Vector3 zoomOffset = new Vector3(0f, 2f, 0f);
    [SerializeField] private float mRotationAmount;
    [SerializeField] private float mRotationSpeed;
    private float mTargetRotation = 0f;

    // Interaction Raycast => For minigames
    private List<GameObject> mHits = new List<GameObject>();
    private RaycastHit[] mRaycastHits = new RaycastHit[50];

    // Drag & Drop
    public GameObject mHolding = null;      // GameObject holded
    private float mCameraDistance;          // Distance between camera and gameobject


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

        mCurrentPos = transform.position;
        mCurrentRot = transform.rotation;

        mPlayer = GameObject.FindGameObjectWithTag("Player");

        mInteractionPopup = GameObject.FindGameObjectWithTag("InteractionImage");
        mInteractionPopup.SetActive(false);
    }
    void Update()
    {
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
            if (!mInputPlayerInteract.Player.Select.IsPressed())
                StopDragging();
            else
            {
                Vector3 mousePos = Mouse.current.position.ReadValue();
                mousePos.z = mCameraDistance;
                Vector3 worldPos = mCamera.ScreenToWorldPoint(mousePos);

                mHolding.transform.position = worldPos;                   
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
            float axis = mInputPlayerMovement.Camera.Rotation.ReadValue<float>();
            mTargetRotation += axis * mRotationAmount;
        }
        transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, Quaternion.Euler(0, mTargetRotation, 0), mRotationSpeed * Time.deltaTime);
    }

    public void StartDragging(GameObject target)
    {
        if (target == null) return;

        mHolding = target;

        mCameraDistance = mCamera.WorldToScreenPoint(mHolding.transform.position).z;

        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null) rb.useGravity = false;
    }
    public void StopDragging()
    {
        Rigidbody rb = mHolding.GetComponent<Rigidbody>();
        if (rb != null) rb.useGravity = true;

        mHolding = null;
    }

    public void SetCameraZoomState(GameObject targetPosition, GameObject lookDirection)
    {
        mCurrentPos = transform.position;
        mCurrentRot = transform.rotation;

        transform.position = targetPosition.transform.position;
        transform.rotation = Quaternion.LookRotation(lookDirection.transform.position - transform.position);

        mState = CameraStates.zoom;

        mInteractionPopup.SetActive(false);

        mPlayer.GetComponent<CharacterInput>().mCanMove = false;
        mPlayer.GetComponent<MeshRenderer>().enabled = false;
    }

    public void AddInteractionTarget(GameObject target)
    {
        zoomTargetOrder.Add(target);
    }
    public void RemoveInteractionTarget(GameObject target)
    {
        zoomTargetOrder.Remove(target);

        if (zoomTargetOrder.Count <= 0)
            SetCameraOrbitaleState();
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

    public void SetCameraOrbitaleState()
    {
        // Sécurité : On lâche l'objet si on quitte le mode zoom
        StopDragging();

        transform.position = mCurrentPos;
        transform.rotation = mCurrentRot;

        mState = CameraStates.orbitale;

        mPlayer.GetComponent<CharacterInput>().mCanMove = true;
        mPlayer.GetComponent<MeshRenderer>().enabled = true;
    }
    public CameraStates GetState()
    {
        return mState;
    }

    public List<GameObject> GetRaycastHit(LayerMask[] layers, string[] tags)
    {
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
    public List<GameObject> GetRaycastHit(LayerMask[] layers, string tag = "")
    {
        return GetRaycastHit(layers, new string[1] { tag });
    }
}