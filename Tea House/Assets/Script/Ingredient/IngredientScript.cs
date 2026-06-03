using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class IngredientScript : MonoBehaviour
{
    Input_PlayerInteract input;

    public MeshRenderer mMeshRenderer;
    public MeshFilter mMeshFilter;

    public IngredientData mIngredientData;
    public bool mIsTorrefied;


    private void Awake()
    {
        input = new Input_PlayerInteract();
    }
    private void OnEnable()
    {
        input.Enable();
    }
    private void OnDisable()
    {
        input.Disable();
    }
    void Start()
    {
        mMeshFilter = GetComponent<MeshFilter>();
        mMeshFilter.mesh = mIngredientData.mModel.GetComponent<MeshFilter>().sharedMesh;

        mMeshRenderer = GetComponent<MeshRenderer>();
        mMeshRenderer.material = mIngredientData.mModel.GetComponent<MeshRenderer>().sharedMaterial;

        mIsTorrefied = false;
    }
}
