using System.Collections.Generic;
using Unity.VisualScripting;
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
        if (InputManager.Get().mPlayerInteract.Player.Select.IsPressed() == true)//inputPlayer
        {
            if (mWorkbench.mCurrentItem == null && mWorkbench.mDragAndDrop.IsDragging() == false)
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
                    else if (hitGo == mWorkbench.mCuttingBoard)
                    {
                        mWorkbench.stateMachine.ChangeState(mWorkbench.blendingState);
                    }
                }
            }
            else
            {
                mWorkbench.mDragAndDrop.StartDragging(mWorkbench.currentItemG);
            }
        }
        else
        {
            mWorkbench.mDragAndDrop.StopDragging();

            if (mWorkbench.mCurrentItem != null)
            {
                if (mWorkbench.mMortarBowl.transform.GetChild(0).GetComponent<WorkingPoint>().IsColliding(mWorkbench.currentItemG))
                {
                    Inventory.msInstance.ChangeItemAmount(mWorkbench.mCurrentItem, -1);

                    mWorkbench.currentItemG.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                    mWorkbench.currentItemG.GetComponent<Rigidbody>().useGravity = false;
                    mWorkbench.currentItemG.GetComponent<BoxCollider>().isTrigger = true;

                    mWorkbench.stateMachine.ChangeState(mWorkbench.bruisingState);
                }
            }

        }
    }
    public void OnExit()
    {

    }
    void InitIngredient(IngredientData _ingredient)
    {
        mWorkbench.mCurrentItem = _ingredient;
        mWorkbench.currentItemG = mWorkbench.InstantiateGameObject(_ingredient.mPrefab, Vector3.zero);

        float distance = Vector3.Distance(mWorkbench.mCameraController.transform.position, mWorkbench.mMortarBowl.transform.position) - 0.5f;

        mWorkbench.currentItemG.transform.position = mWorkbench.mCameraRaycast.GetImpact(distance, mWorkbench.mMortarBowl);

    }
}
