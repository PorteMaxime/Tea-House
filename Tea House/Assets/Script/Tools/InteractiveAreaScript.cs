using UnityEngine;

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
                // TODO : CanInteractWith
                //isUsing = !isUsing; //&& mCameraController.CanInteractWith(gameObject);
                if (isUsing)
                {
                    //Enter();
                }
                // TODO : GetCameraState
                // else if (mCameraController.GetState() == CameraController.CameraStates.zoom)
                // {
                //     Quit();
                // }
            }
        }
    }

    public void Enter()
    {
        // TODO : GoTo
        //mCameraController.GoTo(transform.Find("CameraMovePoint").transform);
        // TODO : SetState
        //mCameraController.SetState(CameraController.CameraStates.zoom);
        // TODO : Disable InteractionPopup
        //mCameraController.mInteractionPopup.SetActive(false);

        // TODO : Block Player
        // mCameraController.mPlayer.GetComponent<CharacterInput>().mCanMove = false;
        // mCameraController.mPlayer.GetComponent<MeshRenderer>().enabled = false;
        isUsing = true;
    }

    public void Quit()
    {
        // TODO : Back
        //mCameraController.Back();
        // TODO : SetState
        //mCameraController.SetState(CameraController.CameraStates.orbitale);
        // TODO : StopDragging
        //mCameraController.StopDragging();

        // TODO : Allow Player to Move
        // mCameraController.mPlayer.GetComponent<CharacterInput>().mCanMove = true;
        // mCameraController.mPlayer.GetComponent<MeshRenderer>().enabled = true;
        isUsing = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // TODO : AddInteractionTarget
            //mCameraController.AddInteractionTarget(gameObject);
            canIntercat = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        canIntercat = false;
        isUsing = false;
        // TODO : RemoveInteractionTarget
        //mCameraController.RemoveInteractionTarget(gameObject);
    }
}
