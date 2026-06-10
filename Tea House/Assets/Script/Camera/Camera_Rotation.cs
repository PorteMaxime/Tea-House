using UnityEngine;

public class Camera_Rotation : ICameraControllerMod
{
    #region Main
    private CameraController mMain = null;
    public Camera_Rotation(CameraController main)
    {
        mMain = main;
        OnInit();
    }
    #endregion

    private Input_PlayerMovement mInput = null;

    private float mRotationSpeed = 5f;
    private float mTargetRotation = 0f;
    private Quaternion mTargetQuaternion = Quaternion.identity;

    public void OnInit()
    {
        mInput = InputManager.Get().mPlayerMovement;
        mInput.Enable();
        
        mTargetRotation = mMain.transform.parent.rotation.eulerAngles.y;
        mTargetQuaternion = Quaternion.Euler(0, mTargetRotation, 0);
    }

    public void OnUpdate()
    {
        if(mInput.Camera.Rotation.triggered)
        {
            float axis = -mInput.Camera.Rotation.ReadValue<float>();
            mTargetRotation += axis * 90f;
            mTargetQuaternion = Quaternion.Euler(0, mTargetRotation, 0);
        }

        if(IsRotating(0.00001f) == true)
            mMain.transform.parent.rotation = Quaternion.Slerp(mMain.transform.parent.rotation, mTargetQuaternion, mRotationSpeed * Time.deltaTime);
    }

    private bool IsRotating(float precision)
    {
        return Quaternion.Angle(mMain.transform.parent.rotation, mTargetQuaternion) >= precision;
    }
}
