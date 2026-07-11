using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private Dictionary<CameraControllerMod, ICameraControllerMod>  mCameraControllerMods = new Dictionary<CameraControllerMod, ICameraControllerMod>();
    public enum CameraControllerMod
    {
        InteractiveHightlight,
        DepthView,
        Rotation,
        Raycast,
        DragAndDrop
    };

    private Dictionary<CameraStates, List<CameraControllerMod>> mStatesMod = new Dictionary<CameraStates, List<CameraControllerMod>>();
    private CameraStates mState = CameraStates.Orbital;
    public enum CameraStates
    {
        Zoom,
        Orbital
    }

    private CharacterInput mPlayerInputComponent = null;
    private SkinnedMeshRenderer mPlayerSkinnedRenderComponent = null;

    public Camera mCamera = null;


    #region Basics Methods
    void Awake()
    {
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        mPlayerInputComponent = playerGO.GetComponent<CharacterInput>();
        mPlayerSkinnedRenderComponent = playerGO.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        mCamera = GetComponent<Camera>();

        mCameraControllerMods.Add(CameraControllerMod.InteractiveHightlight, new Camera_InteractionHighlight(this));
        mCameraControllerMods.Add(CameraControllerMod.DepthView, new Camera_DepthView(this));
        mCameraControllerMods.Add(CameraControllerMod.Rotation, new Camera_Rotation(this));

        mCameraControllerMods.Add(CameraControllerMod.Raycast, new Camera_Raycast(this));
        mCameraControllerMods.Add(CameraControllerMod.DragAndDrop, new CameraDragAndDrop(this));

        mStatesMod.Add(CameraStates.Zoom,
            new List<CameraControllerMod>()
            {
                CameraControllerMod.DepthView,
                CameraControllerMod.Raycast,
                CameraControllerMod.DragAndDrop
            }
        );
        mStatesMod.Add(CameraStates.Orbital,
            new List<CameraControllerMod>()
            {
                CameraControllerMod.InteractiveHightlight,
                CameraControllerMod.DepthView,
                CameraControllerMod.Rotation,
                CameraControllerMod.Raycast
            }
        );
    }

    void Update()
    {
        if(mStatesMod.ContainsKey(mState) == false) return;

        foreach(CameraControllerMod modID in mStatesMod[mState])
        {
            if(mCameraControllerMods.ContainsKey(modID) == true)
                mCameraControllerMods[modID].OnUpdate();
        }
    }

    void FixedUpdate()
    {
        if(mStatesMod.ContainsKey(mState) == false) return;

        foreach(CameraControllerMod modID in mStatesMod[mState])
        {
            if(mCameraControllerMods.ContainsKey(modID) == true)
                mCameraControllerMods[modID].OnFixedUpdate();
        }
    }
    #endregion

    #region State
    public void SetState(CameraStates newState)
    {
        if(mState == newState) return;

        mState = newState;

        switch(mState)
        {
        case CameraStates.Orbital:
            mPlayerInputComponent.mCanMove = true;
            mPlayerSkinnedRenderComponent.enabled = true;
            break;
        case CameraStates.Zoom:
            mPlayerInputComponent.mCanMove = false;
            mPlayerSkinnedRenderComponent.enabled = false;
            break;
        }
    }

    public bool IsState(CameraStates state)
    {
        return mState == state;
    }
    #endregion

    public ModType GetControllerMod<ModType>() where ModType : ICameraControllerMod
    {
        foreach(ICameraControllerMod mod in mCameraControllerMods.Values)
        {
            if(mod is ModType castedMod)
                return castedMod;
        }
        return default;
    }
}

