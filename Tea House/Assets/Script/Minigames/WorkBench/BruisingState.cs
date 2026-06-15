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
<<<<<<< Updated upstream
            // TODO : GetRaycastHit
            // List<GameObject> hitDraggables = mWorkbench.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Mortar");
=======
            mWorkbench.mCameraRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "MortarPilon", out List<GameObject> hitDraggables);
>>>>>>> Stashed changes

            // if (hitDraggables.Count > 0)
            // {
            //     mWorkbench.mCameraController.StartDragging(mWorkbench.mMortar);
            //     mWorkbench.mMortar.transform.rotation = Quaternion.identity;
            // }
            if (mWorkbench.mMortar.transform.localPosition.y <= mWorkbench.mBowl.transform.localPosition.y + posOffset.y 
                && mWorkbench.mMortar.transform.localPosition.x <= mWorkbench.mBowl.transform.localPosition.x + posOffset.x
                && mWorkbench.mMortar.transform.localPosition.x >= mWorkbench.mBowl.transform.localPosition.x - posOffset.x
                && mItemBruised == false
                && InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame())
            {
<<<<<<< Updated upstream
                fragIngredient = mWorkbench.InstantiateGameObject(mWorkbench.mCurrentItem.mFragIngredient, mWorkbench.mBowl.transform.position + ingredientPosOffset, mWorkbench.mBowl);
=======
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
>>>>>>> Stashed changes
                mWorkbench.mBruiseIngrdient = mWorkbench.mCurrentItem;
                mWorkbench.DestroyCurrentIngredient();

                // TODO : StopDragging
                //mWorkbench.mCameraController.StopDragging();
                mWorkbench.mMortar.transform.position = initMortarPos;
                mWorkbench.mMortar.transform.rotation = initMortarRot;
                mItemBruised = true;
<<<<<<< Updated upstream
=======

                mWorkbench.mDragAndDrop.StopDragging();
                mWorkbench.mMortarPilon.transform.position = initMortarPos;
                mWorkbench.mMortarPilon.transform.rotation = initMortarRot;
                bottomMortar = 0;
                topMortar = 0;

>>>>>>> Stashed changes
            }
        }
<<<<<<< Updated upstream

        if (isEmpty == false && mItemBruised == true)
        {
            // TODO : GetRaycastHit
            // List<GameObject> hitDraggables = mWorkbench.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Bowl");
=======
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
>>>>>>> Stashed changes

            // if (hitDraggables.Count > 0)
            // {
            //     mWorkbench.mCameraController.StartDragging(mWorkbench.mBowl);
            // }

            if (mWorkbench.mMixtureBowl.transform.GetChild(0).GetComponent<WorkingPoint>().IsColliding(mWorkbench.mBowl) && InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame())
            {
                mWorkbench.mBowl.transform.position = mWorkbench.mMixtureBowl.transform.position + new Vector3(0, 0.5f, 0);
                mWorkbench.stateMachine.ChangeState(mWorkbench.selectItemState);
            }

        }
    }

    public void OnExit()
    {
<<<<<<< Updated upstream
        // TODO : StopDraggging
        //mWorkbench.mCameraController.StopDragging();
=======
        mWorkbench.mDragAndDrop.StopDragging();
>>>>>>> Stashed changes

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
