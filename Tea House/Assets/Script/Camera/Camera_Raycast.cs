using UnityEngine;

public class Camera_Raycast : ICameraControllerMod
{
    private CameraController mMain = null;
    public Camera_Raycast(CameraController main)
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
