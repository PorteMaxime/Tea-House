using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatValue
{
    public ItemData.Stat mStat = 0;
    public int mValue = 0;
}

public class ItemData : ScriptableObject
{
    public string mName;
    public GameObject mPrefab;

    public enum Stat
    {
        Warmth,
        Freshness,

        Relaxation,
        Energy,

        Prestige
    }

    public List<StatValue> mStats = new List<StatValue>();

    public int mPrice;

    public ItemData()
    {
        foreach (Stat stat in (Stat[])Enum.GetValues(typeof(Stat)))
        {
            StatValue statValue = new StatValue();
            statValue.mStat = stat;
            mStats.Add(statValue);
        }
    }

    //static public Dictionary<Stat, Stat> oppositeStat = new Dictionary<Stat, Stat>()
    //{
    //    { Stat.Warmth, Stat.Freshness },

    //    { Stat.Relaxation, Stat.Energy },
    //    { Stat.Energy, Stat.Relaxation }
    //};
}
