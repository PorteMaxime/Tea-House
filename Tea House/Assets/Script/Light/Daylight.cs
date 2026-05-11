using UnityEngine;

public class Daylight : MonoBehaviour
{
    [SerializeField] float mPhaseDuration = 100f;
    float mCurrentDuration = 0f;
    [SerializeField] Phase mPhase = 0;

    private Light mLight;
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
        GetLight();
    }

    // Update is called once per frame
    void Update()
    {
        if (mCurrentDuration >= mPhaseDuration)
        {
            mCurrentDuration = 0f;
            mPhase += 1;
            if (mPhase == Phase.Count) mPhase = Phase.Morning;

            SwitchPhase();
        }
        else
            mCurrentDuration += Time.deltaTime;
    }

    void SwitchPhase()
    {
        GetLight();

        if (mPhase == Phase.Morning) mLight.color = new Color(1f, 0.95f, 0.84f);
        if (mPhase == Phase.Midday) mLight.color = new Color(1f, 1f, 1f);
        if (mPhase == Phase.Noon) mLight.color = new Color(1f, 0.55f, 0.26f);
    }

    void GetLight()
    {
        if(mLight == null)
            mLight = GetComponent<Light>();
    }
}
