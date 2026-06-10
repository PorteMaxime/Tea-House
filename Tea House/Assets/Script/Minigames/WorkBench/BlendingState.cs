using System.Collections.Generic;
using UnityEngine;

public class BlendingState : IWorkbench
{
    WorkbenchScript mWorkbench;
    public BlendingState(WorkbenchScript system)
    {
        mWorkbench = system;
    }


    Vector3 initSpoonPos = Vector3.zero;
    Quaternion initSpoonRot = Quaternion.identity;


    public void OnEnter()
    {
        initSpoonPos = mWorkbench.mSpoon.transform.position;
        initSpoonRot = mWorkbench.mSpoon.transform.rotation;
    }

    public void OnUpdate()
    {
        List<GameObject> hitDraggables = mWorkbench.mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Spoon");

        if (hitDraggables.Count > 0)
        {
            mWorkbench.mCameraController.StartDragging(mWorkbench.mSpoon);
            mWorkbench.mSpoon.transform.rotation = Quaternion.Euler(90,0,60);
        }
    }
    public void OnExit()
    {
        throw new System.NotImplementedException();
    }
}
