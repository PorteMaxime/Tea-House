using System.Collections.Generic;
using SQLite;
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
        Expired,
    }

    // seulement utile pour la BDD
    [PrimaryKey]
    [Column("mailId")]
    public int mailId { get; set; }

    [Column("state")]
    public State mState { get; set; } = State.NotSent;

    public IngredientData mOrderData = ScriptableObject.CreateInstance<IngredientData>();

    public void AddStats(List<StatValue> newStats)
    {
        for (int i = 0; i < mOrderData.mStats.Count; i++)
        {
            mOrderData.mStats[i].mValue += newStats[i].mValue;
        }
    }
}
