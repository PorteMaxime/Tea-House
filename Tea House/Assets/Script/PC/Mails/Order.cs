using UnityEngine;

public class Order
{
    public enum State
    {
        NotSent,

        InBox,

        Accepted,
        Rejected,

        Completed,
        Expired
    }

    public State mState = State.NotSent;

    public IngredientData mOrderData = ScriptableObject.CreateInstance<IngredientData>();

    // Objectif de la commande
    [SerializeField] public bool mIsLowerLimitWarmth = false;
    [SerializeField] public bool mIsLowerLimitFreshness = false;
    [SerializeField] public bool mIsLowerLimitRelaxation = false;
    [SerializeField] public bool mIsLowerLimitEnergy = false;
    [SerializeField] public bool mIsLowerLimitPrestige = false;
}
