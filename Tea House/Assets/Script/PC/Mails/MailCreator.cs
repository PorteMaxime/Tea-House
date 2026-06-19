using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class MailCreator : MonoBehaviour
{
    [SerializeField] private Daylight mDaylight;

    [SerializeField] private List<MailPreset> mMailsPresets = new List<MailPreset>();

    private Vector2Int mIngredientsAmount = new Vector2Int(1 , 3);
    private Vector2Int mDurationRange = new Vector2Int(2 , 7);

    private int mPresetProbability = 8;
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
        for (int i = 0; i < Random.Range(1, 7); i++)
            CreateMail();
    }

    public void CreateMail()
    {
        Mail mail;

        // Si preset mail
        if (Random.Range(0, 101) <= mPresetProbability && mMailsPresets.Count > 0)
        {
            mail = mMailsPresets[Random.Range(0, mMailsPresets.Count)].CreateMail();

            CreateMailData(mail);
        }
        else
        {
            mail = new Mail();

            Mail.mailID += 1;

            CreateMailData(mail);

            CreateOrder(mail);

            CreateMailDescription(mail);
        }

        mCalendar[mail.mSendDay % 7].Add(mail);
    }

    public void CreateMailData(Mail mail)
    {
        mail.mPicture = mImages[Random.Range(0, mImages.Count)];
        mail.mName = mFirstNames[Random.Range(0, mFirstNames.Count)] + " " + mLastNames[Random.Range(0, mLastNames.Count)];
        mail.mSendDay = mDaylight.mCurrentDay + 1 + Random.Range(0, mCalendar.Length);
        
        if (mail.mDuration <= 0)
            mail.mDuration = Random.Range(mDurationRange.x, mDurationRange.y);
    }
    public void CreateMailDescription(Mail mail)
    {
        // Pour tests
        /*
        if (mail.mOrder == null)
        {
            mail.mText = "-> Order is not set";
            return;
        }

        mail.mText += "\nCreated day " + mail.mSendDay + " | Lasting " + mail.mDuration + " days | Expiring on day " + (mail.mSendDay + mail.mDuration);

        foreach (StatValue stat in mail.mOrder.mOrderData.mStats)
            mail.mText += "\n" + stat.mStat.ToString() + " : " + stat.mValue;
        /**/




    }

    public void CreateOrder(Mail mail)
    {
        Order order = new Order();

        // Base
        List<BaseData> bases = Inventory.msInstance.GetItemsOfType<BaseData>();

        BaseData baseIngredient = bases[Random.Range(0, bases.Count)];
        //mail.mText = baseIngredient.mName; // Pour tests

        order.AddStats(baseIngredient.mStats);

        // Ingredients
        //mail.mText += "\n";
        List<IngredientData> ingredients = Inventory.msInstance.GetItemsOfType<IngredientData>();
        for (int i = 0; i < Random.Range(mIngredientsAmount.x, mIngredientsAmount.y + 1); i++)
        {
            IngredientData ingredient = ingredients[Random.Range(0, ingredients.Count)];
            //mail.mText += " " + ingredient.mName; // Pour tests

            if (Inventory.msInstance.isTorrefactionUnlocked == true && Random.Range(0, 101) <= mTorrefiedProbability)
            {
                //mail.mText += " torrefied"; // Pour tests

                List<StatValue> torrefiedStats = ingredient.GetTorrefiedStats();

                order.AddStats(torrefiedStats);
            }
            else
                order.AddStats(ingredient.mStats);
            //mail.mText += " ; "; // Pour tests
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
