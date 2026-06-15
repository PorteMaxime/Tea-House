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
    //bool moveDownCam = false;
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
            //moveDownCam = false;
            mWorkbench.mCameraDepth.LerpingCamera(mWorkbench.mCameraController.transform.position, targetRotation, 0.3f);
        }
        else if (Mouse.current.position.ReadValue().y <= Screen.height - downPan && cameraLookUp == true)
        {
            Quaternion targetRotation = mWorkbench.mCameraController.transform.rotation * Quaternion.Euler(5f, 0f, 0f);

            cameraLookUp = false;
            //moveDownCam = true;
            mWorkbench.mCameraDepth.LerpingCamera(mWorkbench.mCameraController.transform.position, targetRotation, 0.3f);
        }
        if (mWorkbench.mCurrentItem == null && InputManager.Get().mPlayerInteract.Player.Select.IsPressed())
        {
            mWorkbench.mCameraRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), new string[3] { "Storage", "MixtureBowl", "Spoon" }, out List<GameObject> hitDraggables);

            if (hitDraggables.Count > 0)
            {
                if (hitDraggables[0].TryGetComponent(out WorkBenchStorageScript workBenchStorageScript))
                {
                    InitIngredient(workBenchStorageScript.ingredient);
                    workBenchStorageScript.CheckIngredient();
                    mWorkbench.mDragAndDrop.StartDragging(mWorkbench.currentItemG);
                }
                else if (hitDraggables[0] == mWorkbench.mMixtureBowl)
                {
                    mWorkbench.mDragAndDrop.StartDragging(mWorkbench.mMixtureBowl);
                }
                else if (hitDraggables[0] == mWorkbench.mSpoon)
                {
                    mWorkbench.stateMachine.ChangeState(mWorkbench.blendingState);
                }
            }
        }
        else
        {
            mWorkbench.mCameraRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "Ingredient", out List<GameObject> hitDraggables);

            if (hitDraggables.Count > 0)
            {
                if (hitDraggables[0] == mWorkbench.currentItemG)
                    mWorkbench.mDragAndDrop.StartDragging(mWorkbench.currentItemG);
            }

            if (mWorkbench.mMortarBowl.transform.GetChild(0).GetComponent<WorkingPoint>().IsColliding(mWorkbench.currentItemG) && InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame())
            {
                Inventory.msInstance.RemoveItem(mWorkbench.mCurrentItem, 1);
                mWorkbench.stateMachine.ChangeState(mWorkbench.bruisingState);
            }
        }
    }

    public void OnExit()
    {
        mWorkbench.mDragAndDrop.StopDragging();
    }

    void InitIngredient(IngredientData _ingredient)
    {
        mWorkbench.mCurrentItem = _ingredient;

        Vector3 mousePos = Mouse.current.position.ReadValue();      // Coordonn�es X / Y de la souris sur l'�cran
        mousePos.z = Vector3.Distance(mWorkbench.mCameraController.transform.position, mWorkbench.mMortarBowl.transform.position);
        Vector3 worldPos = mWorkbench.mCameraController.gameObject.GetComponent<Camera>().ScreenToWorldPoint(mousePos);

        Vector3 dir = (worldPos - mWorkbench.mCameraController.gameObject.transform.position).normalized;
        float dist = Vector3.Distance(mWorkbench.mCameraController.gameObject.transform.position, new Vector3(mWorkbench.mMortarBowl.transform.position.x, worldPos.y, worldPos.z));
        worldPos = mWorkbench.mCameraController.gameObject.transform.position + dir * dist;


        mWorkbench.currentItemG = mWorkbench.InstantiateGameObject(_ingredient.mPrefab, worldPos);
    }
}
