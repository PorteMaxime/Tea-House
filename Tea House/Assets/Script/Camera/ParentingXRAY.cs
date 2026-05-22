using UnityEngine;

public class ParentingXRAY : MonoBehaviour
{
    [SerializeField] private GameObject[] mChilds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(bool isShown)
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if(mesh != null) 
            mesh.enabled = isShown;

        foreach (GameObject child in mChilds)
        {
            mesh = child.GetComponent<MeshRenderer>();
            if (mesh == null)
                mesh = child.GetComponentInChildren<MeshRenderer>();
            mesh.enabled = isShown;
        }
    }
}
