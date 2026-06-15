using System.Collections.Generic;
using UnityEngine;

public class SelectItemState : IWorkbench
{
    private WorkbenchScript mWorkbench;
    public SelectItemState(WorkbenchScript system)
    {
        mWorkbench = system;
    }

    public void OnEnter()
    {
        //
    }
    public void OnUpdate()
    {
<<<<<<< Updated upstream
        if (mWorkbench.mCurrentItem == null)
        {
            // TODO : GetRaycastHit
            // List<GameObject> hitDraggables = mWorkbench.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), new string[2] { "Storage", "MixtureBowl"});

            // if (hitDraggables.Count > 0)
            // {
            //     if (hitDraggables[0].TryGetComponent(out WorkBenchStorageScript workBenchStorageScript))
            //     {
            //         InitIngredient(workBenchStorageScript.ingredient);
            //         workBenchStorageScript.CheckIngredient();
            //         mWorkbench.mCameraController.StartDragging(mWorkbench.currentItemG);
            //     }
            //     else if (hitDraggables[0] = mWorkbench.mMixtureBowl)
            //     {
            //         mWorkbench.mCameraController.StartDragging(mWorkbench.mMixtureBowl);
            //     }
            // }
        }
        else
        {
            // TODO : GetRaycastHit
            // List<GameObject> hitDraggables = mWorkbench.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Ingredient");

            // if (hitDraggables.Count > 0)
            // {
            //     if (hitDraggables[0] == mWorkbench.currentItemG)
            //     mWorkbench.mCameraController.StartDragging(mWorkbench.currentItemG);
            // }
=======
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
>>>>>>> Stashed changes
        }

        if (mWorkbench.currentItemG == null)
            return;

        if (mWorkbench.mBowl.transform.GetChild(0).GetComponent<WorkingPoint>().IsColliding(mWorkbench.currentItemG) && InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame())
        {
            mWorkbench.stateMachine.ChangeState(mWorkbench.bruisingState);
        }

    }
    public void OnExit()
    {
<<<<<<< Updated upstream
        mWorkbench.inventory.RemoveItem(mWorkbench.mCurrentItem, 1);
        // TODO : StopDragging
        //mWorkbench.mCameraController.StopDragging();
=======
        mWorkbench.mDragAndDrop.StopDragging();
>>>>>>> Stashed changes
    }

    void InitIngredient(IngredientData _ingredient)
    {
        mWorkbench.mCurrentItem = _ingredient;
<<<<<<< Updated upstream
        mWorkbench.currentItemG = mWorkbench.InstantiateGameObject(_ingredient.mPrefab, mWorkbench.mBowl.transform.position);
=======


        Vector3 mousePos = Mouse.current.position.ReadValue();      // Coordonnées X / Y de la souris sur l'écran
        mousePos.z = Vector3.Distance(mWorkbench.mCameraController.transform.position, mWorkbench.mMortarBowl.transform.position);
        Vector3 worldPos = mWorkbench.mCameraController.gameObject.GetComponent<Camera>().ScreenToWorldPoint(mousePos);

        Vector3 dir = (worldPos - mWorkbench.mCameraController.gameObject.transform.position).normalized;
        float dist = Vector3.Distance(mWorkbench.mCameraController.gameObject.transform.position, new Vector3(mWorkbench.mMortarBowl.transform.position.x, worldPos.y, worldPos.z));
        worldPos = mWorkbench.mCameraController.gameObject.transform.position + dir * dist;


        mWorkbench.currentItemG = mWorkbench.InstantiateGameObject(_ingredient.mPrefab, worldPos);
>>>>>>> Stashed changes
    }
}
