using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class IngredientScript : MonoBehaviour
{
    Input_PlayerInteract input;

    public IngredientData mIngredientData;
    public bool mIsTorrefied;
    
    public MeshRenderer mMeshRenderer;
    public MeshFilter mMeshFilter;

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
        mMeshRenderer = GetComponent<MeshRenderer>();
        mMeshRenderer.material = mIngredientData.mModel.GetComponent<MeshRenderer>().sharedMaterial;
        mMeshFilter = GetComponent<MeshFilter>();
        mMeshFilter.mesh = mIngredientData.mModel.GetComponent<MeshFilter>().sharedMesh;

        transform.localScale = new Vector3(100f, 100f ,100f);

        mIsTorrefied = false;
    }

    private void Update()
    {
        if (input.Debug.GetFragmentIngredient.triggered)
        {
            
        }
    }
}
