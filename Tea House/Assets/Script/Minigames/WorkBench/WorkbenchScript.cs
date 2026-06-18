using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorkbenchScript : MonoBehaviour
{
    public CameraController mCameraController;
    public PackagingScript mPackaging;
    public GameObject mMixtureBowlPackagingPos;
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

    //
    float upPan = Screen.height * 0.15f;
    float downPan = Screen.height * 0.85f;
    bool cameraLookUp = false;
    void Awake()
    {
        mBruiseIngrdient = ScriptableObject.CreateInstance<IngredientData>();
        mMixture = new List<IngredientData>();
    }
    void Start()
    {
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
            if (Mouse.current.position.ReadValue().y >= Screen.height - upPan && cameraLookUp == false)
            {
                Quaternion targetRotation = mCameraController.transform.rotation * Quaternion.Euler(-5f, 0f, 0f);
                cameraLookUp = true;
                //moveDownCam = false;
                mCameraController.LerpingCamera(mCameraController.transform.position, targetRotation, 0.3f);
            }
            else if (Mouse.current.position.ReadValue().y <= Screen.height - downPan && cameraLookUp == true)
            {
                Quaternion targetRotation = mCameraController.transform.rotation * Quaternion.Euler(5f, 0f, 0f);

                cameraLookUp = false;
                //moveDownCam = true;
                mCameraController.LerpingCamera(mCameraController.transform.position, targetRotation, 0.3f);
            }

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
    public void RotateGameObject(GameObject _go, Quaternion targetRotation, float duration)
    {
        StartCoroutine(RotateGO(_go, targetRotation, duration));
    }
    IEnumerator RotateGO(GameObject _go, Quaternion targetRotation, float duration)
    {
        float elapsedTime = 0;
        Quaternion rotationOrigine = _go.transform.rotation;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _go.transform.rotation = Quaternion.Lerp(rotationOrigine, targetRotation, elapsedTime / duration);
            yield return 0;
        }

        _go.transform.rotation = targetRotation;
    }
    public void LerpGameObject(GameObject _go, Vector3 targetPosition, float duration)
    {
        StartCoroutine(LerpGO(_go, targetPosition, duration));
    }
    IEnumerator LerpGO(GameObject _go, Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0;
        Vector3 positionOrigine = _go.transform.position;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _go.transform.position = Vector3.Lerp(positionOrigine, targetPosition, elapsedTime / duration);
            yield return 0;
        }

        _go.transform.position = targetPosition;
    }
}
