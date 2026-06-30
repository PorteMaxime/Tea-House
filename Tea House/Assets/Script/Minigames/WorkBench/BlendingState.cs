using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlendingState : IWorkbench
{
    WorkbenchScript mWorkbench;
    public BlendingState(WorkbenchScript system)
    {
        mWorkbench = system;
    }


    Vector3 initSpoonPos = Vector3.zero;
    Quaternion initSpoonRot = Quaternion.identity;

    int rightMixtureBowl= 0;
    int leftMixtureBowl = 0;
    public bool isEmpty = true;
    public bool mIsBlended = false;
    public bool canSwap = false;
    public void OnEnter()
    {
        initSpoonPos = mWorkbench.mSpoon.transform.position;
        initSpoonRot = mWorkbench.mSpoon.transform.rotation;

    }

    public void OnUpdate()
    {
        if(isEmpty == false && mIsBlended == false)
        {
            if(InputManager.Get().mPlayerInteract.Player.Select.IsPressed() == true)
            {
                mWorkbench.mCameraRaycast.MouseRaycast(Mathf.Infinity, LayerMask.GetMask("Interactibles"), "Spoon", out List<RaycastHit> hitDraggables);

                if (hitDraggables.Count > 0)
                {
                    mWorkbench.mDragAndDrop.StartDragging(mWorkbench.mSpoon);
                    mWorkbench.mSpoon.transform.rotation = Quaternion.Euler(90, 180, 60);
                }
            }

            if (mWorkbench.mSpoon.transform.position.z <= mWorkbench.mMixtureLeftBorn.transform.position.z)
            {
                if (leftMixtureBowl == rightMixtureBowl)
                {
                    leftMixtureBowl++;
                    mWorkbench.RotateGameObject(mWorkbench.mMixtureBowl, mWorkbench.mMixtureBowl.transform.rotation * Quaternion.Euler(0, 180, 0), 1.0f);
                }

            }
            else if (mWorkbench.mSpoon.transform.position.z >= mWorkbench.mMixtureRightBorn.transform.position.z)
            {
                if (leftMixtureBowl > rightMixtureBowl)
                {
                    rightMixtureBowl++;
                    mWorkbench.RotateGameObject(mWorkbench.mMixtureBowl, mWorkbench.mMixtureBowl.transform.rotation * Quaternion.Euler(0, 180, 0), 1.0f);
                }

            }

            if (rightMixtureBowl == 4)
            {
                mIsBlended = true;
                mWorkbench.mPanelSwap.SetActive(true);
                leftMixtureBowl = 0;
                rightMixtureBowl = 0;

                mWorkbench.mDragAndDrop.StopDragging();
                mWorkbench.mSpoon.transform.position = initSpoonPos;
                mWorkbench.mSpoon.transform.rotation = initSpoonRot;

            }
        }
        if(isEmpty == true)
        {
            mWorkbench.stateMachine.ChangeState(mWorkbench.selectItemState);
        }
        if(canSwap == false && mIsBlended == true && InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame())
        {
            Debug.Log("canSwap");
            canSwap = true;
        }

        if(mIsBlended == true && canSwap == true && Mouse.current.position.ReadValue().x > Screen.width * 0.8)
        {
            if(Mouse.current.leftButton.isPressed)
            {
                mWorkbench.mCameraDepth.LerpingCamera(mWorkbench.mCameraPosPackaging.transform.position, mWorkbench.mCameraPosPackaging.transform.rotation, 0.5f);
                mWorkbench.LerpGameObject(mWorkbench.mMixtureBowl, mWorkbench.mMixtureBowlPackagingPos.transform.position, 0.5f);
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.transform.position = new Vector3(mWorkbench.mCameraPosPackaging.transform.position.x, player.transform.position.y, player.transform.position.z);
                mWorkbench.stateMachine.ChangeState(mWorkbench.selectItemState);
            }
        }
    }
    public void OnExit()
    {
        mWorkbench.mPanelSwap.SetActive(false);
    }
}
