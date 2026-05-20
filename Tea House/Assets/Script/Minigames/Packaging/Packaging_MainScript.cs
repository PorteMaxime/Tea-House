using TreeEditor;
using UnityEngine;

public class Packaging_MainScript : MonoBehaviour
{
    private enum State
    {
        Idle,

        PacketChoosing,
        PacketFilling,
        PacketLabelling,

        Count
    }
    private State mState = State.Idle;

    private IngredientData mData = null;
    private GameObject[] mPackage = new GameObject[3];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (mData == null) return;
    }

    public void SetIngredientData(IngredientData _ingredientData) => mData = _ingredientData;


}
