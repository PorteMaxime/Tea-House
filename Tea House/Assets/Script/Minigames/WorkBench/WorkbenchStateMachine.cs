public class WorkbenchStateMachine
{
    IWorkbench currentState;
    public void Update()
    {
        currentState.OnUpdate();
    }
    public void ChangeState(IWorkbench newState)
    {
        currentState?.OnExit();

        currentState = newState;

        currentState.OnEnter();
    }
}
