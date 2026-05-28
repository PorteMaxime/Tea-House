using UnityEngine;
using UnityEngine.UI;

public class HomePageScript : MonoBehaviour
{
    private Text mTextTime;
    [SerializeField] private Daylight mDaylight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform temp = transform.Find("ImageTime/TextTime");
        if (temp != null)
            mTextTime = temp.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mTextTime != null)
        {
            mTextTime.text = mDaylight.GetInGameTime();
        }
    }
}
