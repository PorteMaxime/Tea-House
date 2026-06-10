using UnityEngine;

public class InteractiveAreaScript : MonoBehaviour
{
    private Input_PlayerInteract inputInteract;
    [SerializeField] private bool canIntercat = false;
    [SerializeField] public bool isUsing = false;

    private CameraController mCameraController = null;
    private Camera_InteractionHighlight mCameraInteractionMod = null;
    private Camera_DepthView mCameraDepthMod = null;
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
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        mCameraController = camera.GetComponent<CameraController>();
        mCameraInteractionMod = mCameraController.GetControllerMod<Camera_InteractionHighlight>();
        mCameraDepthMod = mCameraController.GetControllerMod<Camera_DepthView>();
    }
    void Update()
    {
        if (canIntercat)
        {
            if (inputInteract.Player.Open.triggered)
            {
                isUsing = !isUsing && mCameraInteractionMod.CanInteractWith(this);
                if (isUsing)
                    Enter();
                else if (mCameraController.IsState(CameraController.CameraStates.Zoom) == true)
                    Quit();
            }
        }
    }

    public void Enter()
    {
        mCameraDepthMod.GoTo(transform.Find("CameraMovePoint").transform);
        mCameraController.SetState(CameraController.CameraStates.Zoom);
        isUsing = true;
    }

    public void Quit()
    {
        // TODO : StopDragging
        //mCameraController.StopDragging();
        mCameraDepthMod.Back();
        mCameraController.SetState(CameraController.CameraStates.Orbital);
        isUsing = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            mCameraInteractionMod.AddInteractionTarget(this);
            canIntercat = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            canIntercat = false;
            isUsing = false;
            mCameraInteractionMod.RemoveInteractionTarget(this);
        }
    }
}
