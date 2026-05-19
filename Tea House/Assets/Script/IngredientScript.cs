using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class IngredientScript : MonoBehaviour
{
    public IngredientData mIngredientData;
    public bool mIsTorrefied;
    
    public MeshRenderer mMeshRenderer;
    public MeshFilter mMeshFilter;


    void Start()
    {
        mMeshRenderer = GetComponent<MeshRenderer>();
        mMeshRenderer.material = mIngredientData.mModel.GetComponent<MeshRenderer>().sharedMaterial;
        mMeshFilter = GetComponent<MeshFilter>();
        mMeshFilter.mesh = mIngredientData.mModel.GetComponent<MeshFilter>().sharedMesh;

        transform.localScale = new Vector3(100f, 100f ,100f);

        mIsTorrefied = false;
    }
}
