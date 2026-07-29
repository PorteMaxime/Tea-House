using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MailSystem : MonoBehaviour
{
    public Dictionary<Order.State, List<Mail>> mMails = new Dictionary<Order.State, List<Mail>>();

    private Order.State[] mExpiringMailsStates = { Order.State.InBox, Order.State.Accepted };

    [SerializeField] public Daylight mDaylight;
    [SerializeField] public MainPanel mMainPanel;

    MailCreator mMailCreator;
    MailCanvasScript mMailCanvasScript;

    void Awake()
    {
        mMailCreator = GetComponent<MailCreator>();
        mMailCanvasScript = GetComponent<MailCanvasScript>();

        foreach (Order.State state in (Order.State[])Order.State.GetValues(typeof(Order.State)))
            mMails[state] = new List<Mail>();

        StartCoroutine(WatchMails());
    }

    public void AddMailsToSend()
    {
        // Ajout des mails a la liste des mails
        mMails[Order.State.NotSent].AddRange(mMailCreator.mCalendar[mDaylight.mCurrentDay % 7]);

        // Heure/minute d'envoi dans la journee
        int mailsAmount = mMails[Order.State.NotSent].Count() + 1;
        int mailNumber = 1;
        foreach (Mail mail in mMails[Order.State.NotSent])
        {
            mail.mSendTime = mDaylight.mDayLength / mailsAmount * mailNumber;
            mailNumber += 1;
            mail.UpdateExpiration(mDaylight);
        }

        // Tri des mails en fonction de leur heure d'envoi
        mMails[Order.State.NotSent] = mMails[Order.State.NotSent].OrderBy(mail => mail.mSendTime).ToList();

        mMailCreator.mCalendar[mDaylight.mCurrentDay % 7].Clear();
    }

    #region Coroutine

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
                mMailCanvasScript.CreateMailPrefab(firstMail);
            }
            else
                break;
        }

        mMails[Order.State.InBox] = mMails[Order.State.InBox].OrderBy(mail => mail.mExpiration).ToList();
    }

    private void CheckExpired()
    {
        Mail firstMail;

        float totalTimeSpent = mDaylight.GetTotalTimeSpent();

        foreach (Order.State state in mExpiringMailsStates)
        {
            while (mMails[state].Count > 0)
            {
                firstMail = mMails[state].First();
                if (totalTimeSpent >= firstMail.mExpiration)
                {
                    ChangeOrderState(firstMail, Order.State.Expired);
                    mMailCanvasScript.RemoveMailPrefab(firstMail);
                    if (!mMailCanvasScript.mOrdersDictionary.Keys.Contains(firstMail))
                        mMailCanvasScript.CreateOrderPrefab(firstMail);
                    else
                        mMailCanvasScript.UpdatePrefabState(firstMail);
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

        if (mail.mOrder.mState == Order.State.InBox && mail.mIsRead == true)
            mMainPanel.RemoveNewMail();

        if (mMails[mail.mOrder.mState].Contains(mail))
            mMails[mail.mOrder.mState].Remove(mail);

        mail.mOrder.mState = newState;
    }

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
