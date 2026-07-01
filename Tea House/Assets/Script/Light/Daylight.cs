using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Daylight : MonoBehaviour
{
    private Dictionary<Phase, float> mPhases = new Dictionary<Phase, float>();
    public Phase mCurrentPhase;
    private float mCurrentPhaseLength;

    public float mDayTimeSpent = 0f;
    public float mDayLength = 0f;

    private float mPhaseTimeSpent = 0f;
    public int mCurrentDay = 0;

    [SerializeField] private TimeSpan mDayStartHour = new TimeSpan(8, 0, 0);
    [SerializeField] private float mDaySpeedMultiplier = 1; // 1 seconde irl = 1h en jeu
    public TimeSpan mCurrentTime;

    [SerializeField] private MailSystem mMailSystem;
    [SerializeField] private MailCreator mMailCreator;

    [SerializeField] private DatabaseManager mDatabaseManager;

    public enum Phase
    {
        Morning,
        Midday,
        Noon,
        Overtime,

        Count
    }

    void Awake()
    {
        mPhases[Phase.Morning] = 5f;
        mPhases[Phase.Midday] = 2f;
        mPhases[Phase.Noon] = 5f;
        mPhases[Phase.Overtime] = 1f;

        mCurrentTime = mDayStartHour;
        ChangePhase(mPhases.Keys.First());

        foreach (float phaseLength in mPhases.Values)
            mDayLength += phaseLength * (1 / mDaySpeedMultiplier);
    }

    public void Start()
    {
        mMailCreator.CreateDailyMails();
    }

    void Update()
    {
        float timeSpent = Time.deltaTime * mDaySpeedMultiplier;

        mPhaseTimeSpent += timeSpent;
        mDayTimeSpent += timeSpent;

        mCurrentTime += TimeSpan.FromSeconds(timeSpent * 3600);

        if (mPhaseTimeSpent >= mCurrentPhaseLength)
        {
            mPhaseTimeSpent = 0f;

            if (mCurrentPhase + 1 >= Phase.Count)
            {
                // Journee suivante
                transform.parent.rotation = Quaternion.Euler(new Vector3(0f, -45f, 0f));
                mCurrentTime = mDayStartHour;
                mCurrentDay += 1;
                mDayTimeSpent = 0f;

                ChangePhase(mPhases.Keys.First());

                mMailSystem.AddMailsToSend();

                mMailCreator.CreateDailyMails();

                // Sauvegarder la journee
                // TODO: Save day datas
                // mDatabaseManager.SaveMails();
                // mDatabaseManager.SaveDaylight();
                // mDatabaseManager.SaveInventory();
            }
            else
            {
                // Phase suivante de la journee
                ChangePhase(mCurrentPhase + 1);
            }
        }
    }

    void ChangePhase(Phase phase)
    {
        mCurrentPhase = phase;
        mCurrentPhaseLength = mPhases[phase];
    }

    public string GetInGameTime()
    {
        return mCurrentTime.ToString(@"hh\:mm");
    }

    public float GetTotalTimeSpent()
    {
        return mCurrentDay * mDayLength + mDayTimeSpent;
    }
}
