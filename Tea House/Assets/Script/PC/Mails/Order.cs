using System.Collections.Generic;
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

    public void AddStats(List<StatValue> newStats)
    {
        for (int i = 0; i < mOrderData.mStats.Count; i++)
        {
            mOrderData.mStats[i].mValue += newStats[i].mValue;
        }
    }
}
