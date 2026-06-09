using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] private Daylight mDaylight;
    [SerializeField] private Quaternion test = Quaternion.Euler(0, 0, 90);

    GameObject mHours;
    GameObject mMinutes;

    void Awake()
    {
        mHours = transform.Find("short").gameObject;
        mMinutes = transform.Find("long").gameObject;
    }

    void Update()
    {
        if (mDaylight == null)
            return;

        TimeSpan time = mDaylight.currentTime;

        if (mHours != null)
            mHours.transform.rotation = Quaternion.Euler(0, 0, time.Hours * 30 + time.Minutes / 2);

        if (mMinutes != null)
            mMinutes.transform.rotation = Quaternion.Euler(0, 0, time.Minutes * 6);
    }
}
