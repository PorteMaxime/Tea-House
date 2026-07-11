using SQLite;
using System;
using System.Collections.Generic;
using UnityEngine;

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

    public override string ToString()
    {
        string debug = $"=== DEBUG ORDER (ID: {mailId}) ===\n" +
            $"• State: {mState}\n" +        
            $"• LimitHigh: {mLimitHighAmount}\n" +
            $"• Stats:\n"
        ;
        foreach(StatValue stat in mOrderData)
        {
            if(stat.mValue != 0)
                debug += $"    • Item: {stat.mStat} | {stat.mValue}\n";
        }
        debug += "=============================";

        return debug;
    }

    public void AddStats(List<StatValue> newStats)
    {
        string values = "";
        for (int i = 0; i < mOrderData.Count; i++)
        {
            mOrderData[i].mValue += newStats[i].mValue;
            values += newStats[i].mStat + ": " + newStats[i].mValue + " | ";
        }
        //Debug.Log(values);
    }

    /// <Evaluation System>
    /// 1 point par stat dans la meme hauteur que la commande => les deux sont plus haut/bas que la limite ou pas 
    /// 2 points par stat pour la difference entre rendu et commande =>
    ///     2 point si exactement la bonne valeur
    ///     1 point si un point d'ecart
    ///     0 sinon
    /// 2 point pour le prestige si pas de demande dans la commande => valeur max de prestige a 2
    /// 
    public float EvaluateResult(PackageScript suggestion, out float maxObtainable)
    {
        int evaluationLimit = mLimitHighAmount;
        maxObtainable = 0;
        string debug = "";
        foreach(ItemData item in suggestion.mIngredients)
        {
            if(item is IngredientData)
                evaluationLimit++;
            maxObtainable += 3;
        }
        //Debug.Log(debug);
        float totalResult = 0;
        debug = "";
        for(int i = 0; i < mOrderData.Count; i++)
        {
            StatValue suggestionStat = suggestion.mTotal.mStats[i];
            if(suggestionStat.mStat == ItemData.Stat.Prestige) // TODO: Verify if prestige is asked
            {
                int prestige = suggestionStat.mValue;
                if(prestige > 2)
                    prestige = 2;
                totalResult += prestige;
                continue;
            }

            StatValue orderStat = mOrderData[i];
            if(orderStat.mValue == 0) continue;

            //maxObtainable++;
            bool isHigh = orderStat.mValue >= evaluationLimit;
            //debug += orderStat.mStat + ": " + orderStat.mValue + "=" + isHigh + " / " + suggestionStat.mValue + "=" + (suggestionStat.mValue >= evaluationLimit) + " | ";
            
            if((suggestionStat.mValue >= evaluationLimit) == isHigh)
            {
                totalResult++;
                debug += "(suggestionStat.mValue >= evaluationLimit) == isHigh";
            }
            
            int diff = 2 - Math.Abs(suggestionStat.mValue - orderStat.mValue);
            if(diff > 2) 
                diff = 2;
            totalResult += diff;
            debug += diff;
        }
        //Debug.Log(debug);

        return totalResult > maxObtainable ? totalResult = maxObtainable : totalResult;
    }
    public float EvaluateResult_Backup(PackageScript suggestion, out float maxObtainable)
    {
        int evaluationLimit = mLimitHighAmount;
        maxObtainable = 0;
        foreach(ItemData item in suggestion.mIngredients)
        {
            if(item is IngredientData)
                evaluationLimit++;
            //maxObtainable += 3;
        }

        float totalResult = 0;
        string debug = "";
        float prestige = 0;
        for(int i = 0; i < mOrderData.Count; i++)
        {
            StatValue suggestionStat = suggestion.mTotal.mStats[i];
            if(suggestionStat.mStat == ItemData.Stat.Prestige) // TODO: Verify if prestige is asked
            {
                prestige = suggestionStat.mValue;
                // int prestige = suggestionStat.mValue;
                // if(prestige > 2)
                //     prestige = 2;
                // totalResult += prestige;
                continue;
            }

            StatValue orderStat = mOrderData[i];
            if(orderStat.mValue == 0) continue;

            maxObtainable++;
            bool isHigh = orderStat.mValue >= evaluationLimit;
            debug += orderStat.mStat + ": " + orderStat.mValue + "=" + isHigh + " / " + suggestionStat.mValue + "=" + (suggestionStat.mValue >= evaluationLimit) + " | ";
            
            if((suggestionStat.mValue >= evaluationLimit) == isHigh)
                totalResult++;
            
            //totalResult += Math.Abs(suggestionStat.mValue - orderStat.mValue);
        }
        Debug.Log(debug);
        maxObtainable = Mathf.Round((maxObtainable * 5f) / 5f);
        return Mathf.Round((totalResult * 5f + prestige) / 5f);
    }
}
