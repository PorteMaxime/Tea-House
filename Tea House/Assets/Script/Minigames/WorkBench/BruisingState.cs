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

        initBowlPos = mWorkbench.mMortarBowl.transform.parent.position;
        initBowlRot = mWorkbench.mMortarBowl.transform.parent.rotation;

        initMortarPos = mWorkbench.mMortarPilon.transform.position;
        initMortarRot = mWorkbench.mMortarPilon.transform.rotation;

        Debug.Log(initMortarPos);
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

    float timerChangeState = 0f;
    bool timerStart = false;

    public void OnEnter()
    {
        mWorkbench.currentItemG.transform.position = mWorkbench.mMortarBowl.transform.position + ingredientPosOffset;
        isEmpty = false;
    }
    public void OnUpdate()
    {
        if (isEmpty == false && mItemBruised == false)
        {
            if (InputManager.Get().mPlayerInteract.Player.Select.triggered == true)
            {
                mWorkbench.mCameraRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "MortarPilon", out List<RaycastHit> hitDraggables);

                if (hitDraggables.Count > 0)
                {
                    mWorkbench.mDragAndDrop.StartDragging(mWorkbench.mMortarPilon);
                    mWorkbench.mMortarPilon.transform.rotation = Quaternion.identity;
                }
            }
            else if (InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame() == true && mWorkbench.mDragAndDrop.IsDragging() == true)
                mWorkbench.mDragAndDrop.StopDragging();

            if (mWorkbench.mMortarPilon.transform.position.z <= mWorkbench.mMortarRightBorn.transform.position.z
                && mWorkbench.mMortarPilon.transform.position.z >= mWorkbench.mMortarLeftBorn.transform.position.z)
            {
                if (mWorkbench.mMortarPilon.transform.position.y >= mWorkbench.mMortarTopBorn.transform.position.y)
                {
                    if (bottomMortar > topMortar)
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

                mWorkbench.mDragAndDrop.StopDragging();
                mWorkbench.mMortarPilon.transform.position = initMortarPos;
                mWorkbench.mMortarPilon.transform.rotation = initMortarRot;
                bottomMortar = 0;
                topMortar = 0;

                Debug.Log(initMortarPos);
                Debug.Log(mWorkbench.mMortarPilon.transform.position);

            }

        }
        if(canMoveBowl == false && InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame() && mItemBruised == true)
        {
            canMoveBowl = true;
        }
        if (isEmpty == false && mItemBruised == true && canMoveBowl == true)
        {
            if(InputManager.Get().mPlayerInteract.Player.Select.triggered == true)
            {
                mWorkbench.mCameraRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "MortarBowl", out List<RaycastHit> hitDraggables);

                if (hitDraggables.Count > 0)
                {
                    mWorkbench.mDragAndDrop.StartDragging(mWorkbench.mMortarBowl.transform.parent.gameObject);
                }
            }

            if (mWorkbench.mMixtureBowl.transform.GetChild(0).GetComponent<WorkingPoint>().IsColliding(mWorkbench.mMortarBowl) && InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame())
            {
                mWorkbench.blendingState.isEmpty = false;
                //mWorkbench.mMortarBowl.transform.parent.transform.position = mWorkbench.mMixtureBowl.transform.position + new Vector3(0, 0.75f, 0);
                mWorkbench.mMortarBowl.transform.parent.gameObject.transform.position = Vector3.zero;


                fragIngredient.transform.SetParent(mWorkbench.mMixtureBowl.transform);
                fragIngredient.transform.localPosition = new Vector3(0.001f, 0.001f, 0);
                fragIngredient.transform.localScale = new Vector3(0.0005f, 0.0005f, 0.0005f);

                //mWorkbench.mMortarBowl.transform.position = mWorkbench.mMixtureBowl.transform.position + new Vector3(0,1,0);

                //mWorkbench.LerpGameObject(mWorkbench.mMortarBotBorn.transform, fragIngredient);
                mWorkbench.RotateGameObject(mWorkbench.mMortarBowl.transform.parent.gameObject, Quaternion.Euler(-180, 0, 0), 0.5f);

                timerStart = true;
            }
        }

        if(timerStart == true)
        {
            mWorkbench.mDragAndDrop.StopDragging();
            timerChangeState += Time.deltaTime;

            if (timerChangeState > 0.5f)
            {

            }
            if (timerChangeState > 1f)
            {
                timerChangeState = 0;
                timerStart = false;

                mWorkbench.stateMachine.ChangeState(mWorkbench.selectItemState);
            }
        }
    }

    public void OnExit()
    {
        mWorkbench.mDragAndDrop.StopDragging();

        mWorkbench.mMortarBowl.transform.parent.position = initBowlPos;
        mWorkbench.mMortarBowl.transform.parent.rotation = initBowlRot;


        isEmpty = true;
        mItemBruised = false;
        canMoveBowl = false;

        mWorkbench.AddItemToMixture(mWorkbench.mBruiseIngrdient);
        mWorkbench.mBruiseIngrdient = null;
    }
}
