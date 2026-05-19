using UnityEngine;

public class Order
{
    public enum State
    {
        Waiting,

        Accepted,

        Completed,
        Expired
    }
      
    public State mState = State.Waiting;

    public IngredientData mOrderData = ScriptableObject.CreateInstance<IngredientData>();

    // Objectif de la commande
    [SerializeField] private bool mIsLowerLimitWarmth       = false;
    [SerializeField] private bool mIsLowerLimitFreshness    = false;
    [SerializeField] private bool mIsLowerLimitRelaxation   = false;
    [SerializeField] private bool mIsLowerLimitEnergy       = false;
    [SerializeField] private bool mIsLowerLimitPrestige     = false;
}
