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
        if(Mouse.current.rightButton.IsPressed() == true)
        {
            mWorkbench.blendingState.canSwap = true;
            mWorkbench.blendingState.mIsBlended = true;
        }

        if (mWorkbench.mCurrentItem == null)
        {
            if(InputManager.Get().mPlayerInteract.Player.Select.IsPressed() == true)//inputPlayer
            {
                mWorkbench.mCameraRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), new string[3] { "Storage", "MixtureBowl", "Spoon" }, out List<RaycastHit> hitDraggables);

                if (hitDraggables.Count > 0)
                {
                    GameObject hitGo = hitDraggables[0].collider.gameObject;
                    if (hitGo.TryGetComponent(out WorkBenchStorageScript workBenchStorageScript))
                    {
                        InitIngredient(workBenchStorageScript.ingredient);
                        workBenchStorageScript.CheckIngredient();
                        mWorkbench.mDragAndDrop.StartDragging(mWorkbench.currentItemG);
                    }
                    else if (hitGo == mWorkbench.mMixtureBowl)
                    {
                        mWorkbench.mDragAndDrop.StartDragging(mWorkbench.mMixtureBowl);
                    }
                    else if (hitGo == mWorkbench.mSpoon)
                    {
                        mWorkbench.stateMachine.ChangeState(mWorkbench.blendingState);
                    }
                }
            }
        }
        else
        {
            if (InputManager.Get().mPlayerInteract.Player.Select.IsPressed() == true)//inputPlayer
            {
                mWorkbench.mCameraRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "Ingredient", out List<RaycastHit> hitDraggables);

                if (hitDraggables.Count > 0)
                {
                    GameObject hitGo = hitDraggables[0].collider.gameObject;
                    if (hitGo == mWorkbench.currentItemG)
                        mWorkbench.mDragAndDrop.StartDragging(mWorkbench.currentItemG);
                }
            }
            if (mWorkbench.mMortarBowl.transform.GetChild(0).GetComponent<WorkingPoint>().IsColliding(mWorkbench.currentItemG) && InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame())
            {
                Inventory.msInstance.ChangeItemAmount(mWorkbench.mCurrentItem, -1);
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
        mWorkbench.currentItemG = mWorkbench.InstantiateGameObject(_ingredient.mPrefab, Vector3.zero);

        MeshRenderer mesh = mWorkbench.mMortarBowl.GetComponent<MeshRenderer>();
        float distance = Vector3.Distance(mWorkbench.mCameraController.transform.position, mWorkbench.mMortarBowl.transform.position);

        mWorkbench.currentItemG.transform.position = mWorkbench.mCameraRaycast.GetImpact(distance, mWorkbench.mMortarBowl);
    }
}
