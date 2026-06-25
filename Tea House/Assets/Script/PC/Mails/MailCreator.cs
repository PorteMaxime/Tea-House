using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class MailCreator : MonoBehaviour
{
    [SerializeField] private Daylight mDaylight;

    private Vector2Int mMailsPerDayRange = new Vector2Int(1, 7);

    private Vector2Int mIngredientsAmount = new Vector2Int(1 , 3);
    private Vector2Int mDurationRange = new Vector2Int(2 , 7);

    private int mPresetProbability = 10;
    private int mTorrefiedProbability = 25;

    [SerializeField] private List<Sprite> mImages;
    [SerializeField] private List<string> mFirstNames;
    [SerializeField] private List<string> mLastNames;

    public List<Mail>[] mCalendar = new List<Mail>[7];

    public MailCreator()
    {
        for (int i = 0; i < mCalendar.Length; i++)
            mCalendar[i] = new List<Mail>();
    }

    public void CreateDailyMails()
    {
        for (int i = 0; i < Random.Range(10, 70); i++)
            CreateMail();
    }

    public void CreateMail()
    {
        Mail mail = new Mail();

        Mail.mailID += 1;

        CreateMailData(mail);

        CreateOrder(mail);

        CreateMailDescription(mail);

        mCalendar[mail.mSendDay % 7].Add(mail);
    }

    public void CreateMailData(Mail mail)
    {
        mail.mPicture = mImages[Random.Range(0, mImages.Count)];
        mail.mName = mFirstNames[Random.Range(0, mFirstNames.Count)] + " " + mLastNames[Random.Range(0, mLastNames.Count)];
        mail.mSendDay = mDaylight.mCurrentDay + 1 + Random.Range(0, mCalendar.Length);
        mail.mDuration = Random.Range(mDurationRange.x, mDurationRange.y);
    }
    public void CreateMailDescription(Mail mail)
    {
        if (mail.mOrder == null)
        {
            mail.mText += "\norder non existant";
            return;
        }

        mail.mText += "\ncréation jour " + mail.mSendDay + " | durée " + mail.mDuration + " | expiration jour " + (mail.mSendDay + mail.mDuration);

        if (mail.mOrder.mOrderData.mStats.Count == 0)
        {
            mail.mText += "\n Aucune stat dans la commande";
            return;
        }

        foreach (StatValue stat in mail.mOrder.mOrderData.mStats)
            mail.mText += "\n" + stat.mStat.ToString() + " : " + stat.mValue;
    }

    public void CreateOrder(Mail mail)
    {
        if (Random.Range(0, 101) <= mPresetProbability)
        {
            // Si preset mail
            mail.mText += "\npreset mail";
            //return;
        }

        Order order = new Order();

        // Base
        List<BaseData> bases = Inventory.msInstance.GetItemsOfType<BaseData>();
        BaseData baseIngredient = bases[Random.Range(0, bases.Count)];
        mail.mText += "\n" + baseIngredient.mName;

        order.AddStats(baseIngredient.mStats);

        // Ingredients
        List<IngredientData> ingredients = Inventory.msInstance.GetItemsOfType<IngredientData>();
        for (int i = 0; i < Random.Range(mIngredientsAmount.x, mIngredientsAmount.y + 1); i++)
        {
            IngredientData ingredient = ingredients[Random.Range(0, ingredients.Count)];
            mail.mText += "\n" + ingredient.mName;

            if (Inventory.msInstance.isTorrefactionUnlocked == true && Random.Range(0, 101) <= mTorrefiedProbability)
            {
                mail.mText += " + torrefaction";

                List<StatValue> torrefiedStats = ingredient.GetTorrefiedStats();

                order.AddStats(torrefiedStats);
            }
            else
                order.AddStats(ingredient.mStats);
        }

        for (int i = 0; i < order.mOrderData.mStats.Count; i++)
        {
            if (order.mOrderData.mStats[i].mValue != 0)
            {
                int oppositeStat = ItemData.GetOppositeIndex(order.mOrderData.mStats[i].mStat);

                if (oppositeStat >= 0 && order.mOrderData.mStats[oppositeStat].mValue != 0)
                {
                    int rand = Random.Range(0, 2);

                    if (rand == 0)
                        order.mOrderData.mStats[i].mValue = 0;
                    else
                        order.mOrderData.mStats[oppositeStat].mValue = 0;
                }
            }
        }

        mail.mOrder = order;
    }
}
