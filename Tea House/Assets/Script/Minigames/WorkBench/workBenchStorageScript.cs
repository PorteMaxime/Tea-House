using System.Collections;
using UnityEngine;

public class WorkBenchStorageScript : MonoBehaviour
{
    public IngredientItem ingredient;

    public GameObject ingredientAmountG;
    public int amount;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private DatabaseManager databaseManager;

    void Start()
    {
        meshFilter = ingredientAmountG.GetComponent<MeshFilter>();
        meshRenderer = ingredientAmountG.GetComponent<MeshRenderer>();

        databaseManager = GameObject.FindGameObjectWithTag("DataBaseManager").GetComponent<DatabaseManager>();

        StartCoroutine(DBLoad());
    }

    public void CheckIngredient()
    {
        amount = Inventory.msInstance.GetAmount(ingredient);


        UpdateAmount();
    }

    void UpdateAmount()
    {
        if (ingredient.mIsUnlocked == true)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
            return;
        }

        if (amount > 10)
        {
            ShareIngredientRender(ingredient.mAmountJar1);
        }
        else if(amount > 5)
        {
            ShareIngredientRender(ingredient.mAmountJar2);
        }
        else if (amount > 0)
        {
            ShareIngredientRender(ingredient.mAmountJar3);
        }
        else
        {
            meshFilter.mesh = null;
            meshRenderer = null;
        }
    }

    void ShareIngredientRender(GameObject _gameObject)
    {
        meshFilter.mesh = _gameObject.GetComponent<MeshFilter>().sharedMesh;
        meshRenderer.material = _gameObject.GetComponent<MeshRenderer>().sharedMaterial;
    }

    IEnumerator DBLoad()
    {
        while(databaseManager.isLoaded == false)
        {
            yield return null;
        }
        CheckIngredient();
    }
}
