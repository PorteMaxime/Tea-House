using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MailManager : MonoBehaviour
{
    #region Singleton
    public static MailManager msInstance;

    void Awake()
    {
        msInstance = this;
    }
    #endregion

    public static event Action<Mail> mOnMailSent;
    public static event Action<Mail> mOnMailExpired;

    public Daylight mDaylight;

    [SerializeField] private List<IngredientData> mIngredients = new List<IngredientData>();
    [SerializeField] private List<IngredientData> mBases = new List<IngredientData>();

    [SerializeField] private List<Sprite> mPictures = new List<Sprite>();
    [SerializeField] private List<string> mFirstNames = new List<string>();
    [SerializeField] private List<string> mLastNames = new List<string>();

    [SerializeField] private int mTorrefiedProbability = 50; // X % de chance qu'un ingredient soit considere comme torrefie a la creation du mail
    [SerializeField] private (float, float) mDurationRange = (2, 5); // les mails durent entre X et Y jours
    [SerializeField] private (int, int) mMailsToSendRange = (5, 8); // entre X et Y mails a créer par jour

    [SerializeField] private Order.State[] mExpiringMailsStates = { Order.State.InBox, Order.State.Accepted };

    private bool mDailyMailsCreated = false;
    private int mDailyMailsAmount = 0;

    private Dictionary<Order.State, List<Mail>> mMails = new Dictionary<Order.State, List<Mail>>();
    private List<Mail>[] mCalendar = new List<Mail>[7];

    void Start()
    {
        mDaylight = GetComponent<Daylight>();

        foreach (Order.State state in (Order.State[]) Order.State.GetValues(typeof(Order.State)))
            mMails[state] = new List<Mail>();

        for (int i = 0; i < mCalendar.Length; i++)
            mCalendar[i] = new List<Mail>();

        StartDay();

        StartCoroutine(WatchMails());
    }

    void Update()
    {
        if (mDaylight.mCurrentPhase == Daylight.Phase.Midday && mDailyMailsCreated == true) mDailyMailsCreated = false;
    }

    #region Mails Creation
    public void CreateMail()
    {
        Order order = CreateOrder();
        Sprite picture = mPictures[Random.Range(0, mPictures.Count)];
        string name = mFirstNames[Random.Range(0, mFirstNames.Count)] + " " + mLastNames[Random.Range(0, mLastNames.Count)];
        int sendDay = Random.Range(mDaylight.mCurrentDay, mDaylight.mCurrentDay + mCalendar.Count()) + 1;
        float duration = Random.Range(mDurationRange.Item1, mDurationRange.Item2);

        Mail mail = new Mail(order, picture, name, sendDay, duration);
        mail.mText = CreateDescription(mail);

        mCalendar[sendDay % mCalendar.Count()].Add(mail);
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

        for (int i = 0; i < Random.Range(1, 3); i++)
        {
            IngredientData pIngredient = mIngredients[Random.Range(0, mIngredients.Count)];

            if (Random.Range(1, 101) <= mTorrefiedProbability)
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

    private string CreateDescription(Mail mail)
    {
        return "description mail, envoi jour " + mail.mSendDay + " et expiration jour " + (mail.mSendDay + mail.mDuration);
    }
    #endregion Mails Creation

    public void StartDay()
    {
        // Refresh mails
        AddMailsToSend();

        // Creation des mails pour les jours suivants
        for (int i = 0; i < Random.Range(mMailsToSendRange.Item1, mMailsToSendRange.Item2); i++)
            CreateMail();

        mDailyMailsCreated = true;
    }

    private void AddMailsToSend()
    {
        // Ajout des mails a la liste des mails
        mMails[Order.State.NotSent].AddRange(mCalendar[mDaylight.mCurrentDay % 7]);
        mDailyMailsAmount = mMails[Order.State.NotSent].Count();

        // Temps d'envoi dans la journee
        int mailNumber = 1;
        foreach (Mail mail in mMails[Order.State.NotSent])
        {
            mail.mSendTime = mDaylight.mDayLength / (mDailyMailsAmount + 1) * mailNumber;
            mailNumber += 1;
            mail.UpdateExpiration(mDaylight);
        }

        // Tri des mails en fonction de leur heure d'envoi
        mMails[Order.State.NotSent].OrderBy(mail => mail.mSendTime);

        mCalendar[mDaylight.mCurrentDay % 7].Clear();
    }

    #region Coroutine

    // Ajoute les mails du calendrier à la liste et les supprime une fois par seconde
    IEnumerator WatchMails()
    {
        while (true) // Coroutine lancee une seule fois a partir du awake
        {
            // Envoi des mails
            CheckToSend();

            // Expiration des mails
            CheckExpired();

            yield return new WaitForSeconds(1);
        }
    }

    private void CheckToSend()
    {
        Mail firstMail;

        while (mMails[Order.State.NotSent].Count > 0)
        {
            firstMail = mMails[Order.State.NotSent].First();
            if (mDaylight.mDayTimeSpent >= firstMail.mSendTime)
            {
                ChangeOrderState(firstMail, Order.State.InBox);
                mOnMailSent?.Invoke(firstMail);
                Debug.Log("new mail");
            }
            else
                break;
        }
        mMails[Order.State.InBox].OrderBy(mail => mail.mExpiration);
    }

    private void CheckExpired()
    {
        Mail firstMail;

        float totalTimeSpent = mDaylight.mCurrentDay * mDaylight.mDayLength + mDaylight.mDayTimeSpent;

        foreach (Order.State state in mExpiringMailsStates)
        {
            while (mMails[state].Count > 0)
            {
                firstMail = mMails[state].First();
                if (totalTimeSpent >= firstMail.mExpiration)
                {
                    ChangeOrderState(firstMail, Order.State.Expired);
                    mOnMailExpired?.Invoke(firstMail);
                    Debug.Log("mail expired");
                }
                else
                    break;
            }
        }
    }

    #endregion Coroutine

    public void ChangeOrderState(Mail mail, Order.State newState)
    {
        if (mMails[newState].Contains(mail))
            return;

        mMails[newState].Add(mail);

        if (mMails[mail.mOrder.mState].Contains(mail))
            mMails[mail.mOrder.mState].Remove(mail);

        mail.mOrder.mState = newState;
    }

    // Getter mails
    public List<Mail> GetMailsOfState(Order.State[] states)
    {
        List<Mail> returnedMails = new List<Mail>();

        foreach (Order.State state in states)
            if (mMails.ContainsKey(state))
                returnedMails.AddRange(mMails[state]);

        return returnedMails;
    }
    public List<Mail> GetMailsOfState(Order.State state)
    {
        return GetMailsOfState(new Order.State[1] { state });
    }
}
