using System.Collections.Generic;
using UnityEngine;

public class Daylight : MonoBehaviour
{
    [SerializeField] float mPhaseDuration;
    float mCurrentDuration = 0f;
    [SerializeField] Phase mPhase;
    private Dictionary<Phase, float> mPhases = new Dictionary<Phase, float>();

    enum Phase
    {
        Morning,
        Midday,
        Noon,

        Count
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mPhases[Phase.Morning] = 10f;
        mPhases[Phase.Midday] = 2.5f;
        mPhases[Phase.Noon] = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        if (mCurrentDuration >= mPhaseDuration)
        {
            mCurrentDuration = 0f;
            mPhase += 1;
            if (mPhase == Phase.Count)
            {
                mPhase = 0;
                transform.parent.rotation = Quaternion.Euler(new Vector3(0f, -45f, 0f));
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
}
