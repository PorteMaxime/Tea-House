using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem.DualShock.LowLevel;
using System;
using Random = UnityEngine.Random;
using System.Collections;
using System.Linq;

public class MailManager : MonoBehaviour
{
    #region Singleton
    public static MailManager msInstance;
    void Awake()
    {
        msInstance = this;
    }
    #endregion

    public Daylight mDaylight;
    //private float[] time;
    //private int toSend;
    //private float mLast;
    //private float mSpace;


    public List<IngredientData> mIngredients = new List<IngredientData>();
    public (int, int) mTorrefiedProbability = (1, 50);
    public List<IngredientData> mBases = new List<IngredientData>();
    public List<Image> mPictures = new List<Image>();
    public List<string> mNames = new List<string>();
    public (float, float) mDurationSize = (5, 7);

    public List<Mail> mMails = new List<Mail>();
    private List<Mail>[] mCalendar = new List<Mail>[7];
    private int mCurrentDay = 0;
    private bool mHasSend = false;
    private (int, int) mToSendNumber = (5, 8);

    void Start()
    {
        mDaylight = GetComponent<Daylight>();
        for (int i = 0; i < mCalendar.Length; i++)
            mCalendar[i] = new List<Mail>();
    }

    void Update()
    {
        if (mDaylight.mPhase == Daylight.Phase.Morning && mHasSend == false)
            StartDay();
        if(mDaylight.mPhase == Daylight.Phase.Midday && mHasSend == true) mHasSend = false;
    }

    public void CreateMail()
    {
        Order order = CreateOrder();

        Image pic = mPictures[Random.Range(0, mPictures.Count)];
        string name = mNames[Random.Range(0, mNames.Count)];
        float duration = Random.Range(mDurationSize.Item1, mDurationSize.Item2);

        int day = Random.Range(0, 7);
        mCalendar[day].Add(new Mail(order, pic, name, "null", day, duration));
    }

    public Order CreateOrder()
    {
        Order order = new Order();

        IngredientData pBase = mBases[Random.Range(0, mBases.Count)];
        order.mOrderData.mWarmth += pBase.mWarmth;
        order.mOrderData.mFreshness += pBase.mFreshness;
        order.mOrderData.mEnergy += pBase.mEnergy;
        order.mOrderData.mRelaxation += pBase.mRelaxation;
        order.mOrderData.mPrestige += pBase.mPrestige;

        for(int i = 0; i < Random.Range(1, 3); i++)
        {
            IngredientData pIngredient = mIngredients[Random.Range(0, mIngredients.Count)];

            // A VERIFIER
            if(Random.Range(mTorrefiedProbability.Item1, mTorrefiedProbability.Item2) <= 50)
                pIngredient = pIngredient.GetTorrefied();

            order.mOrderData.mWarmth += pBase.mWarmth;
            order.mOrderData.mFreshness += pBase.mFreshness;
            order.mOrderData.mEnergy += pBase.mEnergy;
            order.mOrderData.mRelaxation += pBase.mRelaxation;
            order.mOrderData.mPrestige += pBase.mPrestige;
        }

        order.mIsLowerLimitWarmth = UnityEngine.Random.value < 0.5f;
        order.mIsLowerLimitFreshness = UnityEngine.Random.value < 0.5f;
        order.mIsLowerLimitRelaxation = UnityEngine.Random.value < 0.5f;
        order.mIsLowerLimitEnergy = UnityEngine.Random.value < 0.5f;
        order.mIsLowerLimitPrestige = UnityEngine.Random.value < 0.5f;

        return order;
    }

    void CheckList()
    {
        for(int i = 0; i < mMails.Count; i++)
        {
            if(mMails[i].mSendDay + mMails[i].mDuration < mCurrentDay)
            {
                mMails.RemoveAt(i);
                continue;
            }
        }
    }
    void AddDayList()
    {
        for (int i = 0; i < mCalendar[mCurrentDay % 7].Count; i++)
        {
            mMails.Add(mCalendar[mCurrentDay % 7][i]);
        }
        mCalendar[mCurrentDay % 7].Clear();
    }
    void StartDay()
    {
        // Refresh mails
        mCurrentDay++;
        CheckList();
        AddDayList();

        // For next days
        for (int i = 0; i < Random.Range(mToSendNumber.Item1, mToSendNumber.Item2); i++)
            CreateMail();
        
        mHasSend = true;
    }

    IEnumerator SendMail()
    {
        yield return new WaitForSeconds(3);
    }

    public List<Mail> GetMailsOfState(Order.State[] state)
    {
        List<Mail> returnedMails = new List<Mail>();

        foreach (Mail mail in mMails)
        {
            if (state.Contains(mail.mOrder.mState))
                returnedMails.Add(mail);
        }

        return returnedMails;
    }

    public List<Mail> GetMailsOfState(Order.State state)
    {
        return GetMailsOfState(new Order.State[1] { state });
    }
}
