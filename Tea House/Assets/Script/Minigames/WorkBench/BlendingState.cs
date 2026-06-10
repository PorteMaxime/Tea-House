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
        throw new System.NotImplementedException();
    }
    public void OnExit()
    {
        throw new System.NotImplementedException();
    }
}
