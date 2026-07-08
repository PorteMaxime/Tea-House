using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.InputSystem;

public class CuttingState : IWorkbench
{
    WorkbenchScript mWorkbench;

    public CuttingState(WorkbenchScript script)
    {
        mWorkbench = script;

        initialPosKnife = mWorkbench.mKnife.transform.position;
        initialRotKnife = mWorkbench.mKnife.transform.rotation;
    }

    Vector3 initialPosCam;
    Quaternion initialRotCam;
    public Vector3 initialPosKnife;
    public Quaternion initialRotKnife;

    int posValidZone;
    int dir = -1;
    float cursorSpeed = 3;
    float cursorBorn = 2f;
    int nbCutPoint = 0;
    public bool isItemCut;
    int cuttingPointIndex = 0;
    public bool canGrabItem = false;
    public void OnEnter()
    {
        initialPosCam = mWorkbench.mCameraController.transform.position;
        initialRotCam = mWorkbench.mCameraController.transform.rotation;

        mWorkbench.mCuttingPanel.SetActive(true);

        mWorkbench.mCameraDepth.LerpingCamera(mWorkbench.mCameraPosCuttingBoard.transform.position, mWorkbench.mCameraPosCuttingBoard.transform.rotation, 0.5f);

        mWorkbench.currentItemG.GetComponent<MeshFilter>().mesh = mWorkbench.mCurrentItem.mCuttingBoardIngredient.GetComponent<MeshFilter>().sharedMesh;
        //mWorkbench.currentItemG.GetComponent<BoxCollider>().isTrigger = true;
        
        //mWorkbench.currentItemG.transform.position = mWorkbench.posIngredientGrab.position;
        mWorkbench.currentItemG.transform.position = mWorkbench.mCuttingBoard.transform.position;
    }
    public void OnUpdate()
    {

        if(dir == 1)
        {
            if(mWorkbench.mQteCursor.transform.localPosition.y >= cursorBorn)
            {
                dir = -1;
            }
        }
        else if(dir == -1)
        {
            if (mWorkbench.mQteCursor.transform.localPosition.y <= -cursorBorn)
            {
                dir = 1;
            }
        }
        mWorkbench.mQteCursor.transform.localPosition += new Vector3(0, cursorSpeed * dir * Time.deltaTime, 0);

        if (InputManager.Get().mPlayerInteract.Player.Select.triggered == true)
        {
            if(CursorOk() == true)
            {
                nbCutPoint++;
                MoveValideZone();
                mWorkbench.Cut(cuttingPointIndex);
                cuttingPointIndex++;
            }
            else
            {
                MoveValideZone();
            }

        }

        if(nbCutPoint == 4)
        {
            mWorkbench.currentItemG.GetComponent<MeshFilter>().mesh = mWorkbench.mCurrentItem.mFragIngredient.GetComponent<MeshFilter>().sharedMesh;
            nbCutPoint = 0;
            isItemCut = true;
            mWorkbench.canPutInMixture = true;
            cuttingPointIndex = 0;
            canGrabItem = true;

            mWorkbench.mKnife.transform.localPosition = initialPosKnife;
            mWorkbench.mKnife.transform.rotation = initialRotKnife;

            mWorkbench.stateMachine.ChangeState(mWorkbench.selectItemState);
        }

    }    
    public void OnExit()
    {
        mWorkbench.mCameraDepth.LerpingCamera(initialPosCam, initialRotCam, 0.5f);
        mWorkbench.mCuttingPanel.SetActive(false);
    }
    bool CursorOk()
    {
        if(mWorkbench.mQteCursor.transform.localPosition.y <= mWorkbench.mQteValideZone.transform.localPosition.y + 0.2
            && mWorkbench.mQteCursor.transform.localPosition.y >= mWorkbench.mQteValideZone.transform.localPosition.y - 0.4)
        {

            return true;
        }
        return false;
    }

    void MoveValideZone()
    {
        posValidZone = Random.Range(-150, 150);

        mWorkbench.mQteValideZone.transform.localPosition = 
            new Vector3(mWorkbench.mQteValideZone.transform.localPosition.x, 
            (float)posValidZone / 100,
            mWorkbench.mQteValideZone.transform.localPosition.z);
    }
}
