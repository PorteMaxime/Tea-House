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

    public void OnEnter()
    {
        //
    }
    public void OnUpdate()
    {
        if (mWorkbench.mCurrentItem == null)
        {
            if(true)//inputPlayer
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
        }
        else
        {
            if (true)//inputPlayer
            {
                List<GameObject> hitDraggables = mWorkbench.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Ingredient");

                if (hitDraggables.Count > 0)
                {
                    if (hitDraggables[0] == mWorkbench.currentItemG)
                        mWorkbench.mCameraController.StartDragging(mWorkbench.currentItemG);
                }

                if (mWorkbench.mMortarBowl.transform.GetChild(0).GetComponent<WorkingPoint>().IsColliding(mWorkbench.currentItemG) && InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame())
                {
                    Inventory.msInstance.RemoveItem(mWorkbench.mCurrentItem, 1);
                    mWorkbench.stateMachine.ChangeState(mWorkbench.bruisingState);
                }
            }
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
