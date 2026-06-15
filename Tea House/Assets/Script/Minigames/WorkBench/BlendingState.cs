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
    bool mIsBlended = false;
    public void OnEnter()
    {
        initSpoonPos = mWorkbench.mSpoon.transform.position;
        initSpoonRot = mWorkbench.mSpoon.transform.rotation;
    }

    public void OnUpdate()
    {
        if(isEmpty == false && mIsBlended == false){
            List<GameObject> hitDraggables = mWorkbench.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Spoon");

            if (hitDraggables.Count > 0)
            {
                mWorkbench.mCameraController.StartDragging(mWorkbench.mSpoon);
                mWorkbench.mSpoon.transform.rotation = Quaternion.Euler(90, 180, 60);
            }

            if (mWorkbench.mSpoon.transform.position.z <= mWorkbench.mMixtureLeftBorn.transform.position.z)
            {
                if (leftMixtureBowl == rightMixtureBowl)
                {
                    leftMixtureBowl++;
                }

            }
            else if (mWorkbench.mSpoon.transform.position.z >= mWorkbench.mMixtureRightBorn.transform.position.z)
            {
                if (leftMixtureBowl > rightMixtureBowl)
                {
                    rightMixtureBowl++;
                }

            }

            if (rightMixtureBowl == 4)
            {
                mIsBlended = true;

                leftMixtureBowl = 0;
                rightMixtureBowl = 0;

                mWorkbench.mCameraController.StopDragging();
                mWorkbench.mSpoon.transform.position = initSpoonPos;
                mWorkbench.mSpoon.transform.rotation = initSpoonRot;

            }
        }

        if(mIsBlended == true && Mouse.current.position.ReadValue().x > Screen.width * 0.8)
        {
            if(Mouse.current.leftButton.isPressed)
            {
                Debug.Log("Swap");

                mWorkbench.mCameraController.LerpingCamera(mWorkbench.mCameraPosPackaging.transform.position, mWorkbench.mCameraPosPackaging.transform.rotation, 0.5f);
                mWorkbench.stateMachine.ChangeState(mWorkbench.selectItemState);
            }
        }


        //if (InputManager.Get().mPlayerInteract.Player.Select.WasReleasedThisFrame() && mIsBlended == true)
        //{
        //    mWorkbench.stateMachine.ChangeState(mWorkbench.selectItemState);
        //}

    }
    public void OnExit()
    {

    }
}
