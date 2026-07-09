using System;
using System.Collections.Generic;
using SQLite;
using UnityEngine;

[Table("Stat")]
[System.Serializable]
public class StatValue
{
    // Id
    private int mMailIdVar;
    [Column("mailId")]
    public int mMailId { get => mMailIdVar; set => mMailIdVar = value; }

    // Stat
    [SerializeField] private ItemData.Stat mStatVar;
    [Column("stat")]
    public ItemData.Stat mStat { get => mStatVar; set => mStatVar = value; }

    // Valeur
    [SerializeField] private int mValueVar;
    [Column("value")]
    public int mValue { get => mValueVar; set => mValueVar = value; }
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

        Sweetness,
        Tanginess,

        Prestige
    }

    public List<StatValue> mStats = new List<StatValue>();

    public static Dictionary<Stat, Stat> opposites = new Dictionary<Stat, Stat>
    {
        {Stat.Warmth, Stat.Freshness},
        {Stat.Freshness, Stat.Warmth},
        {Stat.Relaxation, Stat.Energy},
        {Stat.Energy, Stat.Relaxation},
        {Stat.Sweetness, Stat.Tanginess},
        {Stat.Tanginess, Stat.Sweetness},
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

    public GameObject mInfosPage;
}
