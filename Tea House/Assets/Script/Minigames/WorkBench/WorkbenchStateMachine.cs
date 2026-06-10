using UnityEngine;

public class WorkbenchStateMachine
{
    IWorkbench mCurrentState;
    public void Update()
    {
        mCurrentState.OnUpdate();
    }
    public void ChangeState(IWorkbench newState)
    {
        mCurrentState?.OnExit();

        mCurrentState = newState;
        mCurrentState.OnEnter();
    }
}
