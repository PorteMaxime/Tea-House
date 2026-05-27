using UnityEngine;
using UnityEngine.Windows;

public class MortarBowlScript : MonoBehaviour
{
    public WorkingPointCollider mWorkingPointCollider;

    private WorkBenchScript mWorkBenchScript;
    private Vector3 posOffset = new Vector3(0f, 0.1f, 0f);
    void Start()
    {
        mWorkBenchScript = transform.parent.GetComponent<WorkBenchScript>();
        mWorkingPointCollider = transform.Find("WorkingPoint").gameObject.GetComponent<WorkingPointCollider>();
    }


    void Update()
    {
        if (mWorkingPointCollider.mCollider != null && mWorkingPointCollider.mCollider.tag == "Ingredient")
        {
            if(mWorkingPointCollider.mCollider.TryGetComponent(out IngredientScript ingredient))
            {
                GameObject g0 = Instantiate(ingredient.mIngredientData.mFragIngredient, transform.position + posOffset, Quaternion.identity, transform);
                g0.transform.localScale = new Vector3(1000, 1000, 1000);
                Destroy(mWorkingPointCollider.mCollider);
                mWorkBenchScript.currentItem = null;
            }

        }
    }
}
