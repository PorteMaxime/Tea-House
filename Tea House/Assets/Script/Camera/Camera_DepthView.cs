using UnityEngine;

public class Camera_DepthView : ICameraControllerMod
{
    private CameraController mMain = null;
    public Camera_DepthView(CameraController main)
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
