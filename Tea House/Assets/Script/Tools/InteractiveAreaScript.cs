using System.Collections;
using UnityEngine;

public class InteractiveAreaScript : MonoBehaviour
{
    private Input_PlayerInteract inputInteract;
    static private GameObject mPlayer = null;
    [SerializeField] public bool canIntercat = false;
    [SerializeField] public bool isUsing = false;
    [SerializeField] public bool isSliding = false;
    [SerializeField] public bool hideMainPanel = false;

    private CameraController mCameraController = null;
    private Camera_InteractionHighlight mCameraInteractionMod = null;
    private CameraDragAndDrop mDragAndDrop = null;
    private Camera_DepthView mCameraDepthMod = null;
    public bool interactWithCamera = true;

    private Transform mCameraMovePoint = null;

    private GameObject mMainPanel;

    private void Awake()
    {
        inputInteract = InputManager.Get().mPlayerInteract;
        mPlayer = GameObject.FindGameObjectWithTag("Player");

        mCameraMovePoint = transform.Find("CameraMovePoint").transform;

        mMainPanel = GameObject.FindGameObjectWithTag("MainPanel");
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
        mDragAndDrop = mCameraController.GetControllerMod<CameraDragAndDrop>();
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
        if(interactWithCamera)
        {
            mCameraDepthMod.GoTo(mCameraMovePoint);
            mCameraController.SetState(CameraController.CameraStates.Zoom);
        }

        isUsing = true;
        if (hideMainPanel == true)
            mMainPanel.SetActive(false);
    }

    public void Quit()
    {
        if (interactWithCamera)
        {
            mCameraDepthMod.Back();
            mCameraController.SetState(CameraController.CameraStates.Orbital);
        }

        mDragAndDrop.StopDragging();

        isUsing = false;
        if (hideMainPanel == true)
            mMainPanel.SetActive(true);
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

    public Coroutine GoTo(InteractiveAreaScript otherIArea)
    {
        if(isUsing == false || otherIArea == null) return null;
        Quit();
        mCameraDepthMod.GoTo(otherIArea.transform.GetChild(0));
        mCameraController.SetState(CameraController.CameraStates.Zoom);
        
        isSliding = true;
        return StartCoroutine(SlideToInteractiveArea(mPlayer, otherIArea, 0.5f));
    }

    IEnumerator SlideToInteractiveArea(GameObject gameObject, InteractiveAreaScript targetIArea, float duration)
    {
        float elapsedTime = 0;
        Vector3 positionOrigine = gameObject.transform.position;
        Vector3 targetPos = new Vector3(targetIArea.transform.position.x, mPlayer.transform.position.y, targetIArea.transform.position.z);
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            gameObject.transform.position = Vector3.Lerp(positionOrigine, targetPos, elapsedTime / duration);
            yield return 0;
        }

        gameObject.transform.position = targetPos;
        isSliding = false;
        mCameraDepthMod.Back();
        targetIArea.Enter();
    }
}
