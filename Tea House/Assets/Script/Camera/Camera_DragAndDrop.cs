using UnityEngine;

public class CameraDragAndDrop : ICameraControllerMod
{
    private CameraController mMain = null;
    public CameraDragAndDrop(CameraController main)
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
