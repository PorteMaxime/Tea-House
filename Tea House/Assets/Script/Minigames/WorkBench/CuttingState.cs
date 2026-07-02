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

    int posValidZone;
    int dir = -1;
    float cursorSpeed = 100;
    public void OnEnter()
    {
        initialPosCam = mWorkbench.mCameraController.transform.position;
        initialRotCam = mWorkbench.mCameraController.transform.rotation;

        mWorkbench.mCuttingPanel.SetActive(true);

        mWorkbench.mCameraDepth.LerpingCamera(mWorkbench.mCameraPosCuttingBoard.transform.position, mWorkbench.mCameraPosCuttingBoard.transform.rotation, 0.5f);

        mWorkbench.currentItemG.transform.position = mWorkbench.mCuttingBoard.transform.position;
    }
    public void OnUpdate()
    {

        if(dir == 1)
        {
            if(mWorkbench.mQteCursor.transform.localPosition.y >= 45)
            {
                dir = -1;
            }
        }
        else if(dir == -1)
        {
            if (mWorkbench.mQteCursor.transform.localPosition.y <= -45)
            {
                dir = 1;
            }
        }
        mWorkbench.mQteCursor.transform.localPosition += new Vector3(0, cursorSpeed * dir * Time.deltaTime, 0);
        Debug.Log(mWorkbench.mQteCursor.transform.localPosition);

        if (InputManager.Get().mPlayerInteract.Player.Select.triggered && CursorOk() == false)
        {
            posValidZone = Random.Range(-40, 40);

            mWorkbench.mQteValideZone.transform.localPosition = new Vector3(mWorkbench.mQteValideZone.transform.localPosition.x, posValidZone, mWorkbench.mQteValideZone.transform.localPosition.z);

        }
        else
        {
            Debug.Log("ok");
        }
    }    
    public void OnExit()
    {
        mWorkbench.mCameraDepth.LerpingCamera(initialPosCam, initialRotCam, 0.5f);
        mWorkbench.mCuttingPanel.SetActive(false);
    }
    bool CursorOk()
    {
        if(mWorkbench.mQteCursor.transform.position.y <= mWorkbench.mQteValideZone.transform.position.y + 5
            && mWorkbench.mQteCursor.transform.position.y >= mWorkbench.mQteValideZone.transform.position.y - 5)
        {
            return true;
        }
        return false;
    }
}
