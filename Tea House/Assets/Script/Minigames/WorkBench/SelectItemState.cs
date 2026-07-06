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
        if (InputManager.Get().mPlayerInteract.Player.Select.IsPressed() == true)
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
                }
            }
            else
            {
                if (mWorkbench.mDragAndDrop.StartDragging(mWorkbench.currentItemG))
                {
                    if (mWorkbench.cuttingState.canGrabItem == true && InputManager.Get().mPlayerInteract.Player.Select.triggered)
                    {
                        IngredientItem tempItem = mWorkbench.mCurrentItem;
                        mWorkbench.DestroyCurrentIngredient();

                        mWorkbench.mCurrentItem = tempItem;
                        mWorkbench.currentItemG = mWorkbench.InstantiateGameObject(tempItem.mFragGrab, mWorkbench.mCuttingBoard.transform.position);
                        mWorkbench.cuttingState.canGrabItem = false;
                    }
                }
            }
        }
        else
        {
            mWorkbench.mDragAndDrop.StopDragging();

            if (mWorkbench.mCurrentItem != null)
            {
                if (mWorkbench.mMortarBowl.transform.GetChild(0).GetComponent<WorkingPoint>().IsColliding(mWorkbench.currentItemG))
                {
                    if(mWorkbench.mCurrentItem.mType == IngredientItem.TransformType.bruising)
                    {
                        Inventory.msInstance.ChangeItemAmount(mWorkbench.mCurrentItem, -1);

                        mWorkbench.currentItemG.GetComponent<Rigidbody>().useGravity = false;
                        mWorkbench.currentItemG.GetComponent<BoxCollider>().isTrigger = true;

                        mWorkbench.stateMachine.ChangeState(mWorkbench.bruisingState);
                    }

                }
                else if (mWorkbench.mCuttingBoard.transform.GetChild(0).GetComponent<WorkingPoint>().IsColliding(mWorkbench.currentItemG))
                {
                    if (mWorkbench.mCurrentItem.mType == IngredientItem.TransformType.cutting && mWorkbench.cuttingState.isItemCut == false)
                    {
                        mWorkbench.stateMachine.ChangeState(mWorkbench.cuttingState);
                    }
                    else
                    {
                        Debug.Log("Can't cut this !");
                    }
                }
                else if (mWorkbench.mMixtureBowl.transform.GetChild(0).GetComponent<WorkingPoint>().IsColliding(mWorkbench.currentItemG))
                {
                    if (mWorkbench.mCurrentItem.mType == IngredientItem.TransformType.none || mWorkbench.canPutInMixture == true)
                    {
                        mWorkbench.currentItemG.GetComponent<Rigidbody>().useGravity = true;
                        mWorkbench.currentItemG.GetComponent<Rigidbody>().isKinematic = false;

                        mWorkbench.currentItemG.transform.SetParent(mWorkbench.mMixtureBowl.transform);

                        mWorkbench.mCurrentItem = null;
                        mWorkbench.currentItemG = null;

                        mWorkbench.canPutInMixture = false;
                    }
                    else
                    {
                        Debug.Log("Item Need transformation !");
                    }
                }
            }


        }
    }
    public void OnExit()
    {

    }
    void InitIngredient(IngredientItem _ingredient)
    {
        float distance = Vector3.Distance(mWorkbench.mCameraController.transform.position, mWorkbench.mMortarBowl.transform.position) - 0.5f;
        Vector3 ingredientPos = mWorkbench.mCameraRaycast.GetImpact(distance, mWorkbench.mMortarBowl);

        mWorkbench.mCurrentItem = _ingredient;
        mWorkbench.currentItemG = mWorkbench.InstantiateGameObject(_ingredient.mPrefab, ingredientPos);

    }
}
