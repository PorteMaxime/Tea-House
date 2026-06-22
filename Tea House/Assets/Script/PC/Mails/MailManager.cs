//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using Random = UnityEngine.Random;

//public class MailManager : MonoBehaviour
//{
//    #region Singleton
//    public static MailManager msInstance;

//    void Awake()
//    {
//        msInstance = this;
//    }
//    #endregion

//    public static event Action<Mail> mOnMailSent;
//    public static event Action<Mail> mOnMailExpired;

//    public Daylight mDaylight;

//    private List<IngredientData> mAvailableIngredients = new List<IngredientData>();
//    private List<IngredientData> mAvailableBases = new List<IngredientData>();

//    [SerializeField] private List<Sprite> mPictures = new List<Sprite>();
//    [SerializeField] private List<string> mFirstNames = new List<string>();
//    [SerializeField] private List<string> mLastNames = new List<string>();

//    [SerializeField] private int mPresetProbability = 10; // X % de chance que le mail soit choisi parmis les mails pre definis

//    [SerializeField] private Vector2Int mIngredientAmountRange = new Vector2Int(1, 3); // entre X et Y ingredient par commande
//    [SerializeField] private int mTorrefiedProbability = 25; // X % de chance qu'un ingredient soit considere comme torrefie a la creation du mail

//    [SerializeField] private Vector2Int mDurationRange = new Vector2Int(2, 5); // les mails durent entre X et Y jours
//    [SerializeField] private Vector2Int mMailsToSendRange = new Vector2Int(5, 8); // entre X et Y mails a creer par jour

//    [SerializeField] private Order.State[] mExpiringMailsStates = { Order.State.InBox, Order.State.Accepted }; // etats depuis lesquels les mails peuvent expirer

//    private bool mDailyMailsCreated = false;
//    private int mDailyMailsAmount = 0;

//    private Dictionary<Order.State, List<Mail>> mMails = new Dictionary<Order.State, List<Mail>>();
//    private List<Mail>[] mCalendar = new List<Mail>[7];

//    void Start()
//    {
//        mDaylight = GetComponent<Daylight>();

//        foreach (Order.State state in (Order.State[]) Order.State.GetValues(typeof(Order.State)))
//            mMails[state] = new List<Mail>();

//        for (int i = 0; i < mCalendar.Length; i++)
//            mCalendar[i] = new List<Mail>();

//        StartDay();

//        StartCoroutine(WatchMails());
//    }

//    void Update()
//    {
//        if (mDaylight.mCurrentPhase == Daylight.Phase.Midday && mDailyMailsCreated == true)
//            mDailyMailsCreated = false;
//    }

//    #region Mails Creation
//    public void CreateMail()
//    {
//        Mail mail;
//        Sprite picture = mPictures[Random.Range(0, mPictures.Count)];
//        string name = mFirstNames[Random.Range(0, mFirstNames.Count)] + " " + mLastNames[Random.Range(0, mLastNames.Count)];
//        int sendDay = Random.Range(mDaylight.mCurrentDay, mDaylight.mCurrentDay + mCalendar.Count()) + 1;
//        int duration = Random.Range(mDurationRange.x, mDurationRange.y);

//        if (Random.Range(1, 101) <= mPresetProbability)
//        {
//            mail = new Mail(new Order(), picture, "prefab mail - " + name, sendDay, duration + 99);
//        }
//        else
//        {
//            Order order = CreateOrder();

//            mail = new Mail(order, picture, name, sendDay, duration);
//            mail.mText = CreateDescription(mail);

//        }

//        mCalendar[sendDay % mCalendar.Count()].Add(mail);
//    }

//    public Order CreateOrder()
//    {
//        Order order = new Order();

//        // Base
//        if (mAvailableBases.Count > 0)
//            order.AddStats(mAvailableBases[Random.Range(0, mAvailableBases.Count)]);

//        // Ingredients
//        if (mAvailableIngredients.Count > 0)
//        {
//            for (int i = 0; i < Random.Range(mIngredientAmountRange.x, mIngredientAmountRange.y); i++)
//            {
//                IngredientData ingredient = mAvailableIngredients[Random.Range(0, mAvailableIngredients.Count)];

//                if (Inventory.msInstance.isTorrefactionUnlocked == true && Random.Range(1, 101) <= mTorrefiedProbability)
//                    ingredient = ingredient.GetTorrefied();

//                order.AddStats(ingredient);
//            }
//        }

//        return order;
//    }

//    private string CreateDescription(Mail mail)
//    {
//        /* description pour tests */
//        string test = "description mail, envoi jour " + mail.mSendDay + " et expiration jour " + (mail.mSendDay + mail.mDuration);

//        if (mail.mOrder.mOrderData.mStats.Count == 0)
//            return (test += "\n Aucune stat attendue dans la commande...");

//        foreach (StatValue stat in mail.mOrder.mOrderData.mStats)
//            test += "\n" + stat.mStat.ToString() + " : " + stat.mValue;

//        return test;
//    }

//    #endregion Mails Creation

//    public void StartDay()
//    {
//        // Refresh mails
//        AddMailsToSend();

//        // Refresh liste des bases et ingredients
//        UpdateItemsLists();

//        // Creation des mails pour les jours suivants
//        for (int i = 0; i < Random.Range(mMailsToSendRange.x, mMailsToSendRange.y + 1); i++)
//            CreateMail();

//        mDailyMailsCreated = true;
//    }

//    private void UpdateItemsLists()
//    {
//        foreach (var item in Inventory.msInstance.mItems.Keys)
//        {
//            if (item is IngredientData)
//            {
//                IngredientData ingredient = (IngredientData) item;

//                if (ingredient.mIsBase && !mAvailableBases.Contains(ingredient))
//                    mAvailableBases.Add(ingredient);

//                else if (!ingredient.mIsBase && mAvailableIngredients.Contains(ingredient))
//                    mAvailableIngredients.Add(ingredient);
//            }
            
//        }
//    }

//    private void AddMailsToSend()
//    {
//        // Ajout des mails a la liste des mails
//        mMails[Order.State.NotSent].AddRange(mCalendar[mDaylight.mCurrentDay % 7]);
//        mDailyMailsAmount = mMails[Order.State.NotSent].Count();

//        // Temps d'envoi dans la journee
//        int mailNumber = 1;
//        foreach (Mail mail in mMails[Order.State.NotSent])
//        {
//            mail.mSendTime = mDaylight.mDayLength / (mDailyMailsAmount + 1) * mailNumber;
//            mailNumber += 1;
//            mail.UpdateExpiration(mDaylight);
//        }

//        // Tri des mails en fonction de leur heure d'envoi
//        mMails[Order.State.NotSent].OrderBy(mail => mail.mSendTime);

//        mCalendar[mDaylight.mCurrentDay % 7].Clear();
//    }

//    #region Coroutine

//    // Ajoute les mails du calendrier à la liste et les supprime une fois par seconde
//    IEnumerator WatchMails()
//    {
//        while (true) // Coroutine lancee une seule fois a partir du awake
//        {
//            // Envoi des mails
//            CheckToSend();

//            // Expiration des mails
//            CheckExpired();

//            yield return new WaitForSeconds(1);
//        }
//    }

//    private void CheckToSend()
//    {
//        Mail firstMail;

//        while (mMails[Order.State.NotSent].Count > 0)
//        {
//            firstMail = mMails[Order.State.NotSent].First();
//            if (mDaylight.mDayTimeSpent >= firstMail.mSendTime)
//            {
//                ChangeOrderState(firstMail, Order.State.InBox);
//                mOnMailSent?.Invoke(firstMail);
//                Debug.Log("new mail");
//            }
//            else
//                break;
//        }
//        mMails[Order.State.InBox].OrderBy(mail => mail.mExpiration);
//    }

//    private void CheckExpired()
//    {
//        Mail firstMail;

//        float totalTimeSpent = mDaylight.mCurrentDay * mDaylight.mDayLength + mDaylight.mDayTimeSpent;

//        foreach (Order.State state in mExpiringMailsStates)
//        {
//            while (mMails[state].Count > 0)
//            {
//                firstMail = mMails[state].First();
//                if (totalTimeSpent >= firstMail.mExpiration)
//                {
//                    ChangeOrderState(firstMail, Order.State.Expired);
//                    mOnMailExpired?.Invoke(firstMail);
//                    Debug.Log("mail expired");
//                }
//                else
//                    break;
//            }
//        }
//    }

//    #endregion Coroutine

//    public void ChangeOrderState(Mail mail, Order.State newState)
//    {
//        if (mMails[newState].Contains(mail))
//            return;

//        mMails[newState].Add(mail);

//        if (mMails[mail.mOrder.mState].Contains(mail))
//            mMails[mail.mOrder.mState].Remove(mail);

//        mail.mOrder.mState = newState;
//    }

//    // Getter mails
//    public List<Mail> GetMailsOfState(Order.State[] states)
//    {
//        List<Mail> returnedMails = new List<Mail>();

//        foreach (Order.State state in states)
//            if (mMails.ContainsKey(state))
//                returnedMails.AddRange(mMails[state]);

//        return returnedMails;
//    }
//    public List<Mail> GetMailsOfState(Order.State state)
//    {
//        return GetMailsOfState(new Order.State[1] { state });
//    }
//}
