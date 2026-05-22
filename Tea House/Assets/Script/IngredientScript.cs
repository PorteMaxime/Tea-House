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

    private void Update()
    {
        if (input.Debug.GetFragmentIngredient.triggered)
        {
            mMeshRenderer.enabled = false;
            Instantiate(mIngredientData.mFragIngredient, transform.position + new Vector3(0.1f, 0f, 0f), Quaternion.identity);
            Instantiate(mIngredientData.mFragIngredient, transform.position + new Vector3(-0.1f, 0f, 0f), Quaternion.identity);
            Instantiate(mIngredientData.mFragIngredient, transform.position, Quaternion.identity);
            Instantiate(mIngredientData.mFragIngredient, transform.position + new Vector3(0f, 0f, 0.1f), Quaternion.identity);
            Instantiate(mIngredientData.mFragIngredient, transform.position + new Vector3(0f, 0f, -0.1f), Quaternion.identity);
        }
    }
}
