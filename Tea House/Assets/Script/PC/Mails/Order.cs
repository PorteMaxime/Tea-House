using SQLite;
using System;
using System.Collections.Generic;

public class Order
{
    [PrimaryKey]
    [Column("mailId")]
    public int mailId { get; set; }

    public enum State
    {
        NotSent,

        InBox,

        Accepted,
        Rejected,

        Completed,
        Expired,
    }

    [Column("state")]
    public State mState { get; set; } = State.NotSent;

    public int mLimitHighAmount = 2;

    public List<StatValue> mOrderData = new List<StatValue>();

    public Order()
    {
        foreach (ItemData.Stat stat in (ItemData.Stat[])Enum.GetValues(typeof(ItemData.Stat)))
        {
            StatValue statValue = new StatValue();
            statValue.mStat = stat;
            mOrderData.Add(statValue);
        }
    }

    public void AddStats(List<StatValue> newStats)
    {
        for (int i = 0; i < mOrderData.Count; i++)
        {
            mOrderData[i].mValue += newStats[i].mValue;
        }
    }
}
