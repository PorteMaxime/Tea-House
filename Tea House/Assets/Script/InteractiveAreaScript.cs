using UnityEngine;

public class InteractiveAreaScript : MonoBehaviour
{
    private Input_PlayerInteract inputInteract;
    [SerializeField] private bool canIntercat = false;
    [SerializeField] private bool isUsing = false;

    private GameObject mCamera;
    private CameraController mCameraController;
    private void Awake()
    {
        inputInteract = new Input_PlayerInteract();
    }
    private void OnEnable()
    {
        inputInteract.Enable();
    }
    private void OnDisable()
    {
        inputInteract.Disable();
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
            if (inputInteract.Player.Interact.triggered)
            {
                isUsing = !isUsing;
            }
        }
        if (isUsing)
        {
            mCameraController.SetCameraZoomState(gameObject);
        }
        else
        {
            mCameraController.SetCameraOrbitaleState();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Press E to interact");
            canIntercat = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        canIntercat = false;
        isUsing = false;
    }
}
