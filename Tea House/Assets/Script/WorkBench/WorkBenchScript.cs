using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class WorkBenchScript : MonoBehaviour
{
    private Packaging_MainScript mPackaging;
    public CameraController mCameraController;

    public Inventory inventory;

    public List<GameObject> mJars = new List<GameObject>();
    public IngredientData currentItem = null;

    private GameObject currentItemG = null;
    private GameObject mBowl;

    private Vector3 posOffset = new Vector3(0f,0.1f,0f);
    private IngredientData mTotalIngredient;
    private void Awake()
    {
        mTotalIngredient = ScriptableObject.CreateInstance<IngredientData>();
        mBowl = transform.Find("SM_Mortar_bowl").gameObject;
    }

    private void Update()
    {
        if(currentItem == null)
        {
            List<GameObject> hitDraggables = mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Storage");

            if (hitDraggables.Count > 0)
            {
                if (hitDraggables[0].TryGetComponent(out WorkBenchStorageScript workBenchStorageScript))
                {
                    GameObject gO = SelectItem(workBenchStorageScript.ingredient);
                    inventory.RemoveItem(currentItem, 1);
                    workBenchStorageScript.CheckIngredient();

                    mCameraController.StartDragging(gO);
                }
            }
        }
        else
        {
            List<GameObject> hitDraggables = mCameraController.GetRaycastHit(LayerMask.GetMask("Interactibles"), "Ingredient");

            if (hitDraggables.Count > 0)
            {
                GameObject gO = hitDraggables[0];
                mCameraController.StartDragging(gO);
            }
        }
    }

    public GameObject SelectItem(IngredientData ingredient)
    {
        Debug.Log("Mint");
        currentItem = ingredient;
        currentItemG = ingredient.mModel;

        return Instantiate(currentItemG, mBowl.transform.position, Quaternion.identity, mBowl.transform);
    }
    public void AddIngredient()
    {
        if (currentItem == null)
        {
            Debug.LogWarning("Player.currentItem == null when using !");
        }

        mTotalIngredient.mWarmth += currentItem.mWarmth;
        mTotalIngredient.mFreshness += currentItem.mFreshness;
        mTotalIngredient.mEnergy += currentItem.mEnergy;
        mTotalIngredient.mRelaxation += currentItem.mRelaxation;
        mTotalIngredient.mPrestige += currentItem.mPrestige;

        currentItem = null;
    }
    public void Clear()
    {
        mTotalIngredient.mWarmth = 0;
        mTotalIngredient.mFreshness = 0;
        mTotalIngredient.mEnergy = 0;
        mTotalIngredient.mRelaxation = 0;
        mTotalIngredient.mPrestige = 0;
    }
    public void SetIngredientData()
    {
        mPackaging.SetIngredientData(mTotalIngredient);
    }
}