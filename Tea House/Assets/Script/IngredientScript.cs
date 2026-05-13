using UnityEngine;

public class IngredientScript : MonoBehaviour
{
    public IngredientData ingredientData;
    public MeshRenderer mr;
    public MeshFilter mf;
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mr.material = ingredientData.Model.GetComponent<MeshRenderer>().sharedMaterial;
        mf = GetComponent<MeshFilter>();
        mf.mesh = ingredientData.Model.GetComponent<MeshFilter>().sharedMesh;

        transform.localScale = new Vector3(100f, 100f ,100f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
