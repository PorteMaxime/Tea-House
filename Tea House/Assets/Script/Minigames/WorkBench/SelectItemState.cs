using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectItemState : IWorkbench
{
    private WorkbenchScript mWorkbench;
    public SelectItemState(WorkbenchScript system)
    {
        mWorkbench = system;
    }
    float upPan = Screen.height * 0.15f;
    float downPan = Screen.height * 0.85f;
    bool cameraLookUp = false;
    bool moveDownCam = false;
    public void OnEnter()
    {
        //
    }
    public void OnUpdate()
    {
        if (Mouse.current.position.ReadValue().y >= Screen.height - upPan && cameraLookUp == false)
        {
            Quaternion targetRotation = mWorkbench.mCameraController.transform.rotation * Quaternion.Euler(-5f, 0f, 0f);
            cameraLookUp = true;
            moveDownCam = false;
            mWorkbench.mCameraController.LerpingCamera(mWorkbench.mCameraController.transform.position, targetRotation, 0.3f);
        }
        else if (Mouse.current.position.ReadValue().y <= Screen.height - downPan && cameraLookUp == true)
        {
            Quaternion targetRotation = mWorkbench.mCameraController.transform.rotation * Quaternion.Euler(5f, 0f, 0f);

            cameraLookUp = false;
            moveDownCam = true;
            mWorkbench.mCameraController.LerpingCamera(mWorkbench.mCameraController.transform.position, targetRotation, 0.3f);
        }
        if (mWorkbench.mCurrentItem == null)
        {
            List<GameObject> hitDraggables = mWorkbench.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), new string[3] { "Storage", "MixtureBowl", "Spoon" });

            if (hitDraggables.Count > 0)
            {
                if (hitDraggables[0].TryGetComponent(out WorkBenchStorageScript workBenchStorageScript))
                {
                    InitIngredient(workBenchStorageScript.ingredient);
                    workBenchStorageScript.CheckIngredient();
                    mWorkbench.mCameraController.StartDragging(mWorkbench.currentItemG);
                }
                else if (hitDraggables[0] == mWorkbench.mMixtureBowl)
                {
                    mWorkbench.mCameraController.StartDragging(mWorkbench.mMixtureBowl);
                }
                else if (hitDraggables[0] == mWorkbench.mSpoon)
                {
                    mWorkbench.stateMachine.ChangeState(mWorkbench.blendingState);
                }
            }
        }
        else
        {
            List<GameObject> hitDraggables = mWorkbench.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Ingredient");

            if (hitDraggables.Count > 0)
            {
                if (hitDraggables[0] == mWorkbench.currentItemG)
                mWorkbench.mCameraController.StartDragging(mWorkbench.currentItemG);
            }
        }

        if (mWorkbench.mMortarBowl.transform.GetChild(0).GetComponent<WorkingPoint>().IsColliding(mWorkbench.currentItemG) && InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame())
        {
            Inventory.msInstance.RemoveItem(mWorkbench.mCurrentItem, 1);
            mWorkbench.stateMachine.ChangeState(mWorkbench.bruisingState);
        }

    }
    public void OnExit()
    {
        mWorkbench.mCameraController.StopDragging();
    }


    void InitIngredient(IngredientData _ingredient)
    {
        mWorkbench.mCurrentItem = _ingredient;
        mWorkbench.currentItemG = mWorkbench.InstantiateGameObject(_ingredient.mPrefab, mWorkbench.mMortarBowl.transform.position);
    }
}
