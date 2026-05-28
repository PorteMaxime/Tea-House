using UnityEngine;

public class PackagingStateMachine
{
    private IPackaging mCurrentState;


     public void Update()
    {
        mCurrentState.OnUpdate();
    }

    public void SwitchState(IPackaging newState)
    {
        if(mCurrentState != null) mCurrentState.OnExit();

        mCurrentState = newState;
        newState.OnEnter();
    }
}
