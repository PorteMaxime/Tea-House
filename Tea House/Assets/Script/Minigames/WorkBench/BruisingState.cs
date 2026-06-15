using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BruisingState : IWorkbench
{
    WorkbenchScript mWorkbench;
    public BruisingState(WorkbenchScript system)
    {
        mWorkbench = system;
    }

    Vector3 initBowlPos = Vector3.zero;
    Quaternion initBowlRot = Quaternion.identity;
    Vector3 initMortarPos = Vector3.zero;
    Quaternion initMortarRot = Quaternion.identity;
    Vector3 ingredientPosOffset = new Vector3(0, 0.1f, 0);

    GameObject fragIngredient;

    bool isEmpty = true;
    bool mItemBruised = false;
    bool canMoveBowl = false;
    int topMortar = 0;
    int bottomMortar = 0;



    public void OnEnter()
    {
        initBowlPos = mWorkbench.mMortarBowl.transform.position;
        initBowlRot = mWorkbench.mMortarBowl.transform.rotation;

        initMortarPos = mWorkbench.mMortarPilon.transform.position;
        initMortarRot = mWorkbench.mMortarPilon.transform.rotation;

        mWorkbench.currentItemG.transform.position = mWorkbench.mMortarBowl.transform.position + ingredientPosOffset;
        isEmpty = false;
    }
    public void OnUpdate()
    {
        if (isEmpty == false && mItemBruised == false)
        {
            mWorkbench.mCameraRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "MortarPilon", out List<GameObject> hitDraggables);

            if (hitDraggables.Count > 0)
            {
                fragIngredient = mWorkbench.InstantiateGameObject(mWorkbench.mCurrentItem.mFragIngredient, mWorkbench.mBowl.transform.position + ingredientPosOffset, mWorkbench.mBowl);
                mWorkbench.mDragAndDrop.StartDragging(mWorkbench.mMortarPilon);
                mWorkbench.mMortarPilon.transform.rotation = Quaternion.identity;
            }
            if (mWorkbench.mMortarPilon.transform.position.z <= mWorkbench.mMortarRightBorn.transform.position.z
                && mWorkbench.mMortarPilon.transform.position.z >= mWorkbench.mMortarLeftBorn.transform.position.z)
            {
                if(mWorkbench.mMortarPilon.transform.position.y >= mWorkbench.mMortarTopBorn.transform.position.y)
                {
                    if(bottomMortar > topMortar)
                    {
                        topMortar++;
                    }

                }
                else if (mWorkbench.mMortarPilon.transform.position.y <= mWorkbench.mMortarBotBorn.transform.position.y)
                {
                    if (bottomMortar == topMortar)
                    {
                        bottomMortar++;
                    }

                }
            }
            if (bottomMortar == 4)
            {
                fragIngredient = mWorkbench.InstantiateGameObject(mWorkbench.mCurrentItem.mFragIngredient, mWorkbench.mMortarBowl.transform.position + ingredientPosOffset, mWorkbench.mMortarBowl);
                mWorkbench.mBruiseIngrdient = mWorkbench.mCurrentItem;
                mWorkbench.DestroyCurrentIngredient();
                mItemBruised = true;
                mWorkbench.mMortarPilon.transform.position = initMortarPos;
                mWorkbench.mMortarPilon.transform.rotation = initMortarRot;
                bottomMortar = 0;
                topMortar = 0;
            }

        }
        if(canMoveBowl == false && InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame() && mItemBruised)
        {
            Debug.Log("canMoveBowl");
            canMoveBowl = true;
        }
        if (isEmpty == false && mItemBruised == true && canMoveBowl == true)
        {
            mWorkbench.mCameraRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "MortarBowl", out List<GameObject> hitDraggables);

            if (hitDraggables.Count > 0)
            {
                mWorkbench.mDragAndDrop.StartDragging(mWorkbench.mMortarBowl);
            }
            List<GameObject> hitDraggables = mWorkbench.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "MortarBowl");

            if (hitDraggables.Count > 0)
            {
                mWorkbench.mCameraController.StartDragging(mWorkbench.mMortarBowl);
            }

            if (mWorkbench.mMixtureBowl.transform.GetChild(0).GetComponent<WorkingPoint>().IsColliding(mWorkbench.mMortarBowl) && InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame())
            {
                mWorkbench.blendingState.isEmpty = false;
                mWorkbench.mMortarBowl.transform.position = mWorkbench.mMixtureBowl.transform.position + new Vector3(0, 0.5f, 0);
                fragIngredient.transform.SetParent(mWorkbench.mMixtureBowl.transform);
                fragIngredient.transform.localPosition = new Vector3(0, 0.001f, 0);
                fragIngredient.transform.localScale = new Vector3(0.0005f, 0.0005f, 0.0005f);
                mWorkbench.stateMachine.ChangeState(mWorkbench.selectItemState);
            }
        }
    }

    public void OnExit()
    {
        mWorkbench.mDragAndDrop.StopDragging();

        mWorkbench.mMortarBowl.transform.rotation = Quaternion.Euler(0, 0, 180);

        mWorkbench.mMortarBowl.transform.position = initBowlPos;
        mWorkbench.mMortarBowl.transform.rotation = initBowlRot;

        isEmpty = true;
        mItemBruised = false;
        canMoveBowl = false;

        mWorkbench.AddItemToMixture(mWorkbench.mBruiseIngrdient);
        mWorkbench.mBruiseIngrdient = null;

        //mWorkbench.mPackaging.SetIngredientData(mWorkbench.mMixture);
    }
}
