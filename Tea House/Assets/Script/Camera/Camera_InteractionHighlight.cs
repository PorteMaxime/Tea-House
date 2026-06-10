using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_InteractionHighlight : ICameraControllerMod
{
    #region Main
    private CameraController mMain = null;
    public Camera_InteractionHighlight(CameraController main)
    {
        mMain = main;
        OnInit();
    }
    #endregion

    private GameObject mPlayerGO = null;
    private CharacterInput mPlayerInputComponent = null;
    private MeshRenderer mPlayerRenderComponent = null;

    private List<InteractiveAreaScript> mInteractionFields = new List<InteractiveAreaScript>();

    public void OnInit()
    {
        mPlayerGO = GameObject.FindGameObjectWithTag("Player");
        mPlayerInputComponent = mPlayerGO.GetComponent<CharacterInput>();
        mPlayerRenderComponent = mPlayerGO.GetComponent<MeshRenderer>();
    }
    public void OnUpdate()
    {
        if(mInteractionFields.Count > 0) HighlightInteraction();
    }

    public void AddInteractionTarget(InteractiveAreaScript target)
    {
        mInteractionFields.Add(target);
    }
    public void RemoveInteractionTarget(InteractiveAreaScript target)
    {
        mInteractionFields.Remove(target);

        if (mInteractionFields.Count == 0)
        {
            // TODO : ZOOMED => TO ORBITAL
            mMain.GetControllerMod<Camera_DepthView>().Back();
            mMain.SetState(CameraController.CameraStates.Orbital);
            //StopDragging();
        }
    }
    public bool CanInteractWith(InteractiveAreaScript target)
    {
        return mInteractionFields[mInteractionFields.Count - 1] == target;
    }
    
    private void HighlightInteraction()
    {
        // TODO : Find a way to highlight the GameObject which is target of interaction
    }
}

