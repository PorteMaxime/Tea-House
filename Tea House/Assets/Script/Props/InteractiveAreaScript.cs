using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class InteractiveAreaScript : MonoBehaviour
{
    private Input_PlayerInteract inputInteract;
    [SerializeField] private bool canIntercat = false;
    [SerializeField] public bool isUsing = false;

    private GameObject mCamera;
    private CameraController mCameraController;
    private void Awake()
    {
        inputInteract = InputManager.Get().mPlayerInteract;
    }
    private void OnEnable()
    {
        inputInteract.Player.Open.Enable();
    }
    private void OnDisable()
    {
        inputInteract.Player.Open.Disable();
    }
    void Start()
    {
        mCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mCameraController = mCamera.GetComponent<CameraController>();
    }
    void Update()
    {
        if (canIntercat)
        {
            if (inputInteract.Player.Open.triggered)
            {
                isUsing = !isUsing && mCameraController.CanInteractWith(gameObject);
                if (isUsing)
                {
                    mCameraController.GoTo(transform.Find("CameraMovePoint").transform);
                    mCameraController.SetState(CameraController.CameraStates.zoom);
                    mCameraController.mInteractionPopup.SetActive(false);
                }
                else if (mCameraController.GetState() == CameraController.CameraStates.zoom)
                {
                    mCameraController.Back();
                    mCameraController.SetState(CameraController.CameraStates.orbitale);
                    mCameraController.StopDragging();
                }
                mCameraController.mPlayer.GetComponent<CharacterInput>().mCanMove = !isUsing;
                mCameraController.mPlayer.GetComponent<MeshRenderer>().enabled = !isUsing;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            mCameraController.AddInteractionTarget(gameObject);
            canIntercat = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        canIntercat = false;
        isUsing = false;
        mCameraController.RemoveInteractionTarget(gameObject);
    }
}
