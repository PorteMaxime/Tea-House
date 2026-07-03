using UnityEngine;
using UnityEngine.Experimental.AI;

public class CuttingState : IWorkbench
{
    WorkbenchScript mWorkbench;

    public CuttingState(WorkbenchScript script)
    {
        mWorkbench = script;
    }

    Vector3 initialPosCam;
    Quaternion initialRotCam;

    public void OnEnter()
    {
        initialPosCam = mWorkbench.mCameraController.transform.position;
        initialRotCam = mWorkbench.mCameraController.transform.rotation;

        mWorkbench.mCameraDepth.LerpingCamera(mWorkbench.mCameraPosCuttingBoard.transform.position, mWorkbench.mCameraPosCuttingBoard.transform.rotation, 0.5f);
    }
    public void OnUpdate()
    {

    }    
    public void OnExit()
    {
        mWorkbench.mCameraDepth.LerpingCamera(initialPosCam, initialRotCam, 0.5f);
    }

}
