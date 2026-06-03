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
        if (mWorkbench.mCurrentItem == null)
        {
            List<GameObject> hitDraggables = mWorkbench.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Storage");

            if (hitDraggables.Count > 0)
            {
                if (hitDraggables[0].TryGetComponent(out WorkBenchStorageScript workBenchStorageScript))
                {
                    InitIngredient(workBenchStorageScript.ingredient);
                    workBenchStorageScript.CheckIngredient();
                    mWorkbench.mCameraController.StartDragging(mWorkbench.currentItemG);
                }
            }
        }
        else
        {
            List<GameObject> hitDraggables = mWorkbench.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Ingredient");

            if (hitDraggables.Count > 0)
            {
                mWorkbench.mCameraController.StartDragging(hitDraggables[0]);
            }
        }

        if (mWorkbench.currentItemG == null)
            return;

        if (mWorkbench.mBowl.transform.GetChild(0).GetComponent<WorkingPoint>().mCollider == mWorkbench.currentItemG)
        {
            mWorkbench.stateMachine.ChangeState(mWorkbench.bruisingState);
        }

    }
    public void OnExit()
    {
        mWorkbench.inventory.RemoveItem(mWorkbench.mCurrentItem, 1);
        mWorkbench.mCameraController.StopDragging();
    }


    void InitIngredient(IngredientData _ingredient)
    {
        mWorkbench.mCurrentItem = _ingredient;
        mWorkbench.currentItemG = mWorkbench.InstantiateGameObject(_ingredient.mModel, mWorkbench.mBowl.transform.position);
    }
}
