using UnityEngine;
using UnityEngine.UI;

public class HomePageScript : MonoBehaviour
{
    private Text mTextTime;
    [SerializeField] private Daylight mDaylight;

    void Awake()
    {
        Transform temp = transform.Find("ImageTime/TextTime");
        if (temp != null)
            mTextTime = temp.GetComponent<Text>();
    }

    void Update()
    {
        if (mTextTime != null)
        {
            mTextTime.text = mDaylight.GetInGameTime();
        }
    }
}
