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

    public bool mIsUnlocked;

    public enum Stat
    {
        Warmth,
        Freshness,

        Relaxation,
        Energy,

        Prestige
    }

    public List<StatValue> mStats = new List<StatValue>();

    public static Dictionary<Stat, Stat> opposites = new Dictionary<Stat, Stat>
    {
        {Stat.Warmth, Stat.Freshness},
        {Stat.Freshness, Stat.Warmth},
        {Stat.Relaxation, Stat.Energy},
        {Stat.Energy, Stat.Relaxation},
    };

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

    public static int GetOppositeIndex(Stat stat)
    {
        if (!opposites.ContainsKey(stat))
            return -1;

        return (int)opposites[stat];
    }
}
