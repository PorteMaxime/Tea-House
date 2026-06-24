using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMailPreset", menuName = "Scriptable Objects/MailPreset")]
public class MailPreset : ScriptableObject
{
    [SerializeField] private BaseData mBase;
    [SerializeField] private List<IngredientData> mIngredients = new List<IngredientData>();
    [SerializeField] private PackageData mPackageData;

    [SerializeField] private string mText = "";
    [SerializeField] private int mDuration = 0;

    public Mail CreateMail()
    {
        Mail mail = new Mail();

        mail.mText = mText;
        mail.mDuration = mDuration;

        CreateOrder(mail);

        return mail;
    }

    private void CreateOrder(Mail mail)
    {
        Order order = new Order();

        if (mBase != null)
            order.AddStats(mBase.mStats);

        foreach (IngredientData ingredient in mIngredients)
        {
            order.AddStats(ingredient.mStats);
        }

        if (mPackageData != null)
            order.AddStats(mBase.mStats);

        mail.mOrder = order;
    }
}