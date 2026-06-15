using System.Collections.Generic;
using UnityEngine;

public class WorkbenchScript : MonoBehaviour
{
    public CameraController mCameraController;
    public Camera_Raycast mCameraRaycast;
    public CameraDragAndDrop mDragAndDrop;
    public Camera_DepthView mCameraDepth;
    public PackagingScript mPackaging;
    public InteractiveAreaScript mInteractiveArea;
    public GameObject mCameraPosPackaging;

    public GameObject mMortarBowl;
    public GameObject mMortarPilon;
    public GameObject mMixtureBowl;
    public IngredientData mCurrentItem = null;
    public GameObject currentItemG = null;
    public List<IngredientData> mMixture;
    public IngredientData mBruiseIngrdient;

    public GameObject mMortarTopBorn;
    public GameObject mMortarBotBorn;
    public GameObject mMortarRightBorn;
    public GameObject mMortarLeftBorn;

    public GameObject mMixtureRightBorn;
    public GameObject mMixtureLeftBorn;

    public GameObject mSpoon;

    public WorkbenchStateMachine stateMachine;

    public SelectItemState selectItemState;
    public BruisingState bruisingState;
    public BlendingState blendingState;
    void Awake()
    {
        mBruiseIngrdient = ScriptableObject.CreateInstance<IngredientData>();
        mMixture = new List<IngredientData>();
    }
    void Start()
    {
        mCameraRaycast = mCameraController.GetControllerMod<Camera_Raycast>();
        mDragAndDrop = mCameraController.GetControllerMod<CameraDragAndDrop>();
        mCameraDepth = mCameraController.GetControllerMod<Camera_DepthView>();
        
        stateMachine = new WorkbenchStateMachine();

        selectItemState = new SelectItemState(this);
        bruisingState = new BruisingState(this);
        blendingState = new BlendingState(this);

        stateMachine.ChangeState(selectItemState);
    }

    void Update()
    {
        if (mInteractiveArea.isUsing)
        {
            stateMachine.Update();
        }

    }

    public void AddItemToMixture(IngredientData item)
    {
        mMixture.Add(item);
    }

    public GameObject InstantiateGameObject(GameObject _gameObject, Vector3 pos)
    {
        return Instantiate(_gameObject, pos, _gameObject.transform.rotation);
    }
    public GameObject InstantiateGameObject(GameObject _gameObject, Vector3 pos, GameObject _parent)
    {
        return Instantiate(_gameObject, pos, Quaternion.identity, _parent.transform);
    }
    public void DestroyCurrentIngredient()
    {
        mCurrentItem = null;
        Destroy(currentItemG);
    }
}
