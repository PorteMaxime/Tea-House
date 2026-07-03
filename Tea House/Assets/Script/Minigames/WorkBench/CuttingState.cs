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
    float cursorSpeed = 3;
    int cursorBorn = 2;
    int nbCutPoint = 0;
    public bool isItemCut;
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
                Debug.Log("ok");
                nbCutPoint++;
                MoveValideZone();
            }
            else
            {
                MoveValideZone();
            }

        }
        if(nbCutPoint == 1)
        {
            mWorkbench.currentItemG.GetComponent<MeshFilter>().mesh = mWorkbench.mCurrentItem.mFragIngredient.GetComponent<MeshFilter>().sharedMesh;
        }
        if(nbCutPoint == 4)
        {
            IngredientItem tempItem = mWorkbench.mCurrentItem;
            mWorkbench.DestroyCurrentIngredient();

            mWorkbench.mCurrentItem = tempItem;
            mWorkbench.currentItemG = mWorkbench.InstantiateGameObject(tempItem.mFragGrab, mWorkbench.mCuttingBoard.transform.position);
            //mWorkbench.currentItemG.GetComponent<MeshFilter>().mesh = mWorkbench.mCurrentItem.mFragGrab.GetComponent<MeshFilter>().sharedMesh;

            nbCutPoint = 0;
            isItemCut = true;
            mWorkbench.canPutInMixture = true;

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
            Debug.Log("cursorPos: " + mWorkbench.mQteCursor.transform.position.y);
            Debug.Log("Valide: " + (mWorkbench.mQteValideZone.transform.position.y - 0.4));
            return true;
        }
        Debug.Log("cursorPos: " + mWorkbench.mQteCursor.transform.position.y);
        Debug.Log("Valide: " + (mWorkbench.mQteValideZone.transform.position.y - 0.4));
        return false;
    }

    void MoveValideZone()
    {
        posValidZone = Random.Range(-175, 175);

        mWorkbench.mQteValideZone.transform.localPosition = 
            new Vector3(mWorkbench.mQteValideZone.transform.localPosition.x, 
            (float)posValidZone / 100, 
            mWorkbench.mQteValideZone.transform.localPosition.z);
    }
}
