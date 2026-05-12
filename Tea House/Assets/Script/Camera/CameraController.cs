using UnityEngine;
using UnityEngine.Windows;

public class CameraController : MonoBehaviour
{
    private Vector3 mInitialPos = new Vector3(-7.5f, 7.5f, -7.5f);
    private Vector3 mInitialRot = new Vector3(25f, 45f, 0f);

    private Vector3 mCurrentPos = Vector3.zero;
    private Quaternion mCurrentRot = Quaternion.identity;

    private GameObject mPlayer;

    private Input_PlayerMovement mInputPlayerMovement;
    private Input_PlayerInteract mInputPlayerInteract;

    private Vector3 zoomOffset = new Vector3(0f, 2f, 0f);
    [SerializeField] private float mRotationSpeed;

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

    private enum CameraStates
    {
        orbitale,
        zoom
    }
    private CameraStates mState = CameraStates.orbitale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = mInitialPos;
        transform.rotation = Quaternion.Euler(mInitialRot);

        mCurrentPos = mInitialPos;
        mCurrentRot = Quaternion.Euler(mInitialRot);

        mPlayer = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (mState == CameraStates.zoom && mInputPlayerInteract.Player.Interact.triggered)
        {
            return;//
        }
        if (mState == CameraStates.orbitale && mInputPlayerMovement.Camera.Rotation.triggered)
        {
            float axis = mInputPlayerMovement.Camera.Rotation.ReadValue<float>();
            float rotationAmount = axis * mRotationSpeed;
            transform.parent.Rotate(0, rotationAmount, 0);

            mCurrentPos = transform.position;
            mCurrentRot = transform.rotation;
        }

    }
    public void SetCameraZoomState(GameObject target)
    {
        transform.position = mPlayer.transform.position + zoomOffset;
        transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        mState = CameraStates.zoom;
    }
    public void SetCameraOrbitaleState()
    {
        transform.position = mCurrentPos;
        transform.rotation = mCurrentRot;
        mState = CameraStates.orbitale;
    }
}
