using UnityEngine;
using UnityEngine.Windows;

public class CameraController : MonoBehaviour
{
    private Vector3 mCurrentPos = Vector3.zero;
    private Quaternion mCurrentRot = Quaternion.identity;

    private GameObject mPlayer;

    private Input_PlayerMovement mInputPlayerMovement;
    private Input_PlayerInteract mInputPlayerInteract;

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
    }
    void Update()
    {
        InputHandler();
    }
    void InputHandler()
    {
        if (mState == CameraStates.zoom)
        {
            return;//
        }
        else if (mState == CameraStates.orbitale)
        {
            if (mInputPlayerMovement.Camera.Rotation.triggered)//A,E
            {
                float axis = mInputPlayerMovement.Camera.Rotation.ReadValue<float>();
                mTargetRotation += axis * mRotationAmount;
            }

            transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, Quaternion.Euler(0, mTargetRotation, 0), mRotationSpeed * Time.deltaTime);
        }
    }
    public void SetCameraZoomState(GameObject target)
    {
        mCurrentPos = transform.position;
        mCurrentRot = transform.rotation;

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
    public CameraStates GetState()
    {
        return mState;
    }
}