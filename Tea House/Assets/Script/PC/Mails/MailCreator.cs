using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class MailCreator : MonoBehaviour
{
    [SerializeField] private Daylight mDaylight;

    public List<IngredientData> mBases;
    public List<IngredientData> mIngredients;

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
        for (int i = 0; i < Random.Range(1, 7); i++)
            CreateMail();
    }

    public void CreateMail()
    {
        Mail mail = new Mail();

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

        mail.mText += "\ncréation jour " + mail.mSendDay +
            "\ndurée" + mail.mDuration;

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
        IngredientData baseIngredient = mBases[Random.Range(0, mBases.Count)];
        mail.mText += "\n" + baseIngredient.mName;

        order.AddStats(baseIngredient.mStats);

        // Ingredients
        for (int i = 0; i < Random.Range(mIngredientsAmount.x, mIngredientsAmount.y + 1); i++)
        {
            IngredientData ingredient = mIngredients[Random.Range(0, mIngredients.Count)];
            mail.mText += "\n" + ingredient.mName;

            if (Random.Range(0, 101) <= mTorrefiedProbability)
            {
                mail.mText += " + torrefaction";

                List<StatValue> torrefiedStats = ingredient.GetTorrefiedStats();

                order.AddStats(torrefiedStats);
            }
            else
                order.AddStats(ingredient.mStats);
        }

        mail.mOrder = order;
    }
}
