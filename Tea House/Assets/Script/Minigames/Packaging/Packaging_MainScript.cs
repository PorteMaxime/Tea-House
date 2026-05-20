using NUnit.Framework;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Packaging_MainScript : MonoBehaviour
{
    [SerializeField] private GameObject mCamera = null;

    private enum State
    {
        Idle,

        PacketChoosing,
        Completing,
        Finish,

        Count
    }
    private State mState = State.Idle;

    private CameraController mCameraController = null;

    private IngredientData mData = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        mCameraController = mCamera.GetComponent<CameraController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (mData == null || mCamera == null) return;
        if (mState == State.PacketChoosing) SelectPackage();
    }

    public void SetIngredientData(IngredientData _ingredientData)
    {
        mData = _ingredientData;
        mState = State.PacketChoosing;
    }

    private void SelectPackage()
    {
        List<GameObject> packages = mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Package");
        if(packages.Count > 0)
        {
            // CREATE GAMEOBJECT OF PACKAGE
        }
    }
}
