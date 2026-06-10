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

        if (IsMoving(0.00001f) == false)
            mMain.transform.position = mCurrentTarget.position;
        else
            mMain.transform.position = Vector3.Slerp(mMain.transform.position, mCurrentTarget.position, mMoveSpeed * Time.deltaTime);

        if (IsRotating(0.00001f) == false)
            mMain.transform.rotation = mCurrentTarget.rotation;
        else
            mMain.transform.rotation = Quaternion.Slerp(mMain.transform.rotation, mCurrentTarget.rotation, mRotationSpeed * Time.deltaTime);
    }

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
    }
    public void Back()
    {
        if (mDepth.Count <= 1) return;
        
        mDepth.RemoveAt(mDepth.Count - 1);
        mCurrentTarget = mDepth[mDepth.Count - 1];
    }

    private IEnumerator ReturnToOrbital()
    {
        yield return 0f;
    }
}
