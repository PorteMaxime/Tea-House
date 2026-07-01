using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.InputSystem;

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

        GameObject.FindGameObjectWithTag("MainPanel").SetActive(false);

        mWorkbench.mCameraDepth.LerpingCamera(mWorkbench.mCameraPosCuttingBoard.transform.position, mWorkbench.mCameraPosCuttingBoard.transform.rotation, 0.5f);

        mWorkbench.currentItemG.transform.position = mWorkbench.mCuttingBoard.transform.position;
    }
    public void OnUpdate()
    {
        if (Mouse.current.leftButton.IsPressed())
        {
            int test = Random.Range(5, 76);

            mWorkbench.mQteValideZone.transform.localPosition = new Vector3(mWorkbench.mQteValideZone.transform.localPosition.x, test, mWorkbench.mQteValideZone.transform.localPosition.z);
        }
    }    
    public void OnExit()
    {
        mWorkbench.mCameraDepth.LerpingCamera(initialPosCam, initialRotCam, 0.5f);
        GameObject.FindGameObjectWithTag("MainPanel").SetActive(true);
    }

}
