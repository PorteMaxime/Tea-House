using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_DepthView : ICameraControllerMod
{
    #region  Main
    private CameraController mMain = null;
    public Camera_DepthView(CameraController main)
    {
        mMain = main;
        OnInit();
    }
    #endregion

    private List<Transform> mDepth = new List<Transform>();
    private Transform mCurrentTarget;

    private float mMoveSpeed = 5f;
    private float mRotationSpeed = 5f;

    public void OnInit()
    {
        GoTo(mMain.transform.parent.Find("RootTransformOfMainCamera"));
    }

    public void OnUpdate()
    {
        if (mDepth.Count == 0) return;

        // if (IsMoving(0.00001f) == false)
        //     mMain.transform.position = mCurrentTarget.position;
        // else
        //     mMain.transform.position = Vector3.Slerp(mMain.transform.position, mCurrentTarget.position, mMoveSpeed * Time.deltaTime);

        // if (IsRotating(0.00001f) == false)
        //     mMain.transform.rotation = mCurrentTarget.rotation;
        // else
        //     mMain.transform.rotation = Quaternion.Slerp(mMain.transform.rotation, mCurrentTarget.rotation, mRotationSpeed * Time.deltaTime);
    }

    public void OnFixedUpdate() {}

    public bool IsMoving(float precision)
    {
        return mDepth.Count > 0 ? Vector3.Distance(mMain.transform.position, mCurrentTarget.position) >= precision : false;
    }
    public bool IsRotating(float precision)
    {
        return Quaternion.Angle(mMain.transform.rotation, mCurrentTarget.rotation) >= precision;
    }

    public void GoTo(Transform otherTransform)
    {
        mDepth.Add(otherTransform);
        mCurrentTarget = mDepth[mDepth.Count - 1];
        LerpingCamera(otherTransform.position, otherTransform.rotation, 0.5f);
    }
    public void Back()
    {
        if (mDepth.Count <= 1) return;
        
        mDepth.RemoveAt(mDepth.Count - 1);
        mCurrentTarget = mDepth[mDepth.Count - 1];
        LerpingCamera(mCurrentTarget.position, mCurrentTarget.rotation, 0.5f);
    }

    private IEnumerator ReturnToOrbital()
    {
        yield return 0f;
    }

    IEnumerator mcMoveTo(Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        float elapsedTime = 0;
        Vector3 positionOrigine = mMain.transform.position;
        Quaternion rotationOrigine = mMain.transform.rotation;
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            mMain.transform.position = Vector3.Lerp(positionOrigine, targetPosition, elapsedTime / duration);
            mMain.transform.rotation = Quaternion.Lerp(rotationOrigine, targetRotation, elapsedTime / duration);
            yield return 0;
        }

        mMain.transform.position = targetPosition;
        mMain.transform.rotation = targetRotation;
    }
    public void LerpingCamera(Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        mMain.StartCoroutine(mcMoveTo(targetPosition, targetRotation, duration));
    }
}
