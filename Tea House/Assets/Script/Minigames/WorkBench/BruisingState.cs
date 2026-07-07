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
        if(InputManager.Get().mPlayerInteract.Player.Select.triggered == true)
        {
            mWorkbench.mCameraRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), new string[2] { "MortarPilon", "MortarBowl" }, out List<RaycastHit> hitDraggables);

            if (hitDraggables.Count > 0)
            {
                GameObject hitGo = hitDraggables[0].collider.gameObject;
                if (hitGo == mWorkbench.mMortarPilon)
                {
                    if (isEmpty == false && mItemBruised == false)
                    {
                        mWorkbench.mDragAndDrop.StartDragging(mWorkbench.mMortarPilon);
                        mWorkbench.mMortarPilon.transform.rotation = Quaternion.identity;
                    }
                }
                else if (hitGo == mWorkbench.mMortarBowl)
                {
                    if (isEmpty == false && mItemBruised == true && canMoveBowl == true)
                    {
                        mWorkbench.mDragAndDrop.StartDragging(mWorkbench.mMortarBowl.transform.parent.gameObject);
                    }
                }
            }

        }
        else if(mWorkbench.mDragAndDrop.mDragging == mWorkbench.mMortarPilon)
        {
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
                fragIngredient.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                mWorkbench.mBruiseIngrdient = mWorkbench.mCurrentItem;
                mWorkbench.DestroyCurrentIngredient();
                mItemBruised = true;

                mWorkbench.mDragAndDrop.StopDragging();
                mWorkbench.mMortarPilon.transform.position = initMortarPos;
                mWorkbench.mMortarPilon.transform.rotation = initMortarRot;
                bottomMortar = 0;
                topMortar = 0;
            }
        }
        else if(InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame())
        {
            if (canMoveBowl == false && mItemBruised == true)
            {
                canMoveBowl = true;
            }
            mWorkbench.mDragAndDrop.StopDragging();
        }

        if (isEmpty == false && mItemBruised == true && canMoveBowl == true)
        {
            if (mWorkbench.mMixtureBowl.transform.GetChild(0).GetComponent<WorkingPoint>().IsColliding(mWorkbench.mMortarBowl) && InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame())
            {
                mWorkbench.blendingState.isEmpty = false;

                mWorkbench.LerpGameObject(mWorkbench.mMortarBowl.transform.parent.gameObject, mWorkbench.mMixtureBowl.transform.position + new Vector3(0, 0.5f, 0), 0.5f);
                mWorkbench.RotateGameObject(mWorkbench.mMortarBowl.transform.parent.gameObject, Quaternion.Euler(-180, 0, 0), 0.5f);

                timerStart = true;
            }
        }

        if (timerStart == true)
        {
            mWorkbench.mDragAndDrop.StopDragging();
            timerChangeState += Time.deltaTime;

            if (timerChangeState > 0.5f)
            {
                fragIngredient.GetComponent<Rigidbody>().useGravity = true;
                fragIngredient.GetComponent<Rigidbody>().isKinematic = false;

                fragIngredient.transform.SetParent(mWorkbench.mMixtureBowl.transform);
                fragIngredient.transform.localScale = new Vector3(0.0005f, 0.0005f, 0.0005f);
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

        mWorkbench.mMortarPilon.transform.position = initMortarPos;
        mWorkbench.mMortarPilon.transform.rotation = initMortarRot;

        isEmpty = true;
        mItemBruised = false;
        canMoveBowl = false;

        mWorkbench.AddItemToMixture(mWorkbench.mBruiseIngrdient);
        mWorkbench.mBruiseIngrdient = null;
    }
}
