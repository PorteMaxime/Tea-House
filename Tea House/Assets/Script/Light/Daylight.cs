using System;
using System.Collections.Generic;
using UnityEngine;

public class Daylight : MonoBehaviour
{
    private float dayDuration;
    public TimeSpan currentTime = new TimeSpan(8, 0, 0);

    [SerializeField] float mPhaseDuration;
    float mCurrentDuration = 0f;
    [SerializeField] public Phase mPhase;
    private Dictionary<Phase, float> mPhases = new Dictionary<Phase, float>();

    public enum Phase
    {
        Morning,
        Midday,
        Noon,

        Count
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mPhases[Phase.Morning] = 5f;// * 60;
        mPhases[Phase.Midday] = 2f;//* 60;
        mPhases[Phase.Noon] = 5f;//* 60;
        mPhase = Phase.Count;
        mCurrentDuration = mPhaseDuration;

        foreach (Phase phase in mPhases.Values)
            dayDuration += (int)phase;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += TimeSpan.FromSeconds(Time.deltaTime * 43200 / dayDuration);

        if (mCurrentDuration >= mPhaseDuration)
        {
            mCurrentDuration = 0f;
            mPhase += 1;
            if (mPhase >= Phase.Count)
            {
                mPhase = 0;
                transform.parent.rotation = Quaternion.Euler(new Vector3(0f, -45f, 0f));

                currentTime = TimeSpan.Parse("08:00");
            }

            mPhaseDuration = mPhases[mPhase];
        }
        else
            mCurrentDuration += Time.deltaTime;

        FollowOrigin();
    }

    void FollowOrigin()
    {
        transform.parent.rotation = Quaternion.Euler(0f, -45f, 60f * (mCurrentDuration / mPhaseDuration) + (int)mPhase * 60f);
        transform.LookAt(transform.parent.position);
    }

    public string GetInGameTime()
    {
        return currentTime.ToString(@"hh\:mm");
    }
}
