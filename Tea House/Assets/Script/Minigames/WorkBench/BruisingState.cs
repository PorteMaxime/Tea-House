using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BruisingState : IWorkbench
{
    WorkbenchScript mWorkbench;
    Vector3 initBowlPos = Vector3.zero;
    Quaternion initBowlRot = Quaternion.identity;
    Vector3 initMortarPos = Vector3.zero;
    Quaternion initMortarRot = Quaternion.identity;
    Vector3 posOffset = new Vector3(0.1f, 0.1f, 0);
    Vector3 ingredientPosOffset = new Vector3(0, 0.1f, 0);

    GameObject fragIngredient;

    bool isEmpty = true;
    bool mItemBruised = false;
    public BruisingState(WorkbenchScript system)
    {
        mWorkbench = system;
    }

    public void OnEnter()
    {
        initBowlPos = mWorkbench.mBowl.transform.position;
        initBowlRot = mWorkbench.mBowl.transform.rotation;

        initMortarPos = mWorkbench.mMortar.transform.position;
        initMortarRot = mWorkbench.mMortar.transform.rotation;

        mWorkbench.currentItemG.transform.position = mWorkbench.mBowl.transform.position + ingredientPosOffset;
        isEmpty = false;
    }
    public void OnUpdate()
    {
        if (isEmpty == false && mItemBruised == false)
        {
            List<GameObject> hitDraggables = mWorkbench.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Mortar");

            if (hitDraggables.Count > 0)
            {
                mWorkbench.mCameraController.StartDragging(mWorkbench.mMortar);
                mWorkbench.mMortar.transform.rotation = Quaternion.identity;
            }
            if (mWorkbench.mMortar.transform.localPosition.y <= mWorkbench.mBowl.transform.localPosition.y + posOffset.y 
                && mWorkbench.mMortar.transform.localPosition.x <= mWorkbench.mBowl.transform.localPosition.x + posOffset.x
                && mWorkbench.mMortar.transform.localPosition.x >= mWorkbench.mBowl.transform.localPosition.x - posOffset.x
                && mItemBruised == false
                && InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame())
            {
                fragIngredient = mWorkbench.InstantiateGameObject(mWorkbench.mCurrentItem.mFragIngredient, mWorkbench.mBowl.transform.position + ingredientPosOffset, mWorkbench.mBowl);
                mWorkbench.mBruiseIngrdient = mWorkbench.mCurrentItem;
                mWorkbench.DestroyCurrentIngredient();

                mWorkbench.mCameraController.StopDragging();
                mWorkbench.mMortar.transform.position = initMortarPos;
                mWorkbench.mMortar.transform.rotation = initMortarRot;
                mItemBruised = true;
            }
        }

        if (isEmpty == false && mItemBruised == true)
        {

            List<GameObject> hitDraggables = mWorkbench.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Bowl");

            if (hitDraggables.Count > 0)
            {
                mWorkbench.mCameraController.StartDragging(mWorkbench.mBowl);
            }

            if (mWorkbench.mMixtureBowl.transform.GetChild(0).GetComponent<WorkingPoint>().mCollider == mWorkbench.mBowl && InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame())
            {
                mWorkbench.mBowl.transform.position = mWorkbench.mMixtureBowl.transform.position + new Vector3(0, 0.5f, 0);
                mWorkbench.stateMachine.ChangeState(mWorkbench.selectItemState);
            }

        }
    }

    public void OnExit()
    {
        mWorkbench.mCameraController.StopDragging();

        mWorkbench.mBowl.transform.rotation = Quaternion.Euler(0, 0, 180);

        fragIngredient.GetComponent<Rigidbody>().useGravity = true;
        fragIngredient.GetComponent<Rigidbody>().isKinematic = false;
        fragIngredient.transform.SetParent(null);
        fragIngredient.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

        mWorkbench.mBowl.transform.position = initBowlPos;
        mWorkbench.mBowl.transform.rotation = initBowlRot;

        isEmpty = true;
        mItemBruised = false;

        mWorkbench.AddItemToMixture(mWorkbench.mBruiseIngrdient);
        mWorkbench.mBruiseIngrdient = null;

        //mWorkbench.mPackaging.SetIngredientData(mWorkbench.mMixture);
    }
}
