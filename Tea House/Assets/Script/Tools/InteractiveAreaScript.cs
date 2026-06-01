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
                    Enter();
                }
                else if (mCameraController.GetState() == CameraController.CameraStates.zoom)
                {
                    Quit();
                }
            }
        }

    }

    public void Enter()
    {
        mCameraController.GoTo(transform.Find("CameraMovePoint").transform);
        mCameraController.SetState(CameraController.CameraStates.zoom);
        mCameraController.mInteractionPopup.SetActive(false);

        mCameraController.mPlayer.GetComponent<CharacterInput>().mCanMove = false;
        mCameraController.mPlayer.GetComponent<MeshRenderer>().enabled = false;
        isUsing = true;
    }

    public void Quit()
    {
        mCameraController.Back();
        mCameraController.SetState(CameraController.CameraStates.orbitale);
        mCameraController.StopDragging();

        mCameraController.mPlayer.GetComponent<CharacterInput>().mCanMove = true;
        mCameraController.mPlayer.GetComponent<MeshRenderer>().enabled = true;
        isUsing = false;
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
