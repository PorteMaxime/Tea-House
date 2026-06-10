using UnityEngine;

public class Camera_XRAY : ICameraControllerMod
{
    private CameraController mMain = null;
    public Camera_XRAY(CameraController main)
    {
        mMain = main;
        OnInit();
    }

    public void OnInit()
    {
        
    }

    public void OnUpdate()
    {
        
    }
}
