using UnityEngine;


public class Mail
{
    public Order mOrder;
    public Sprite mPicture;
    public string mName;
    public string mText;
    public int mSendDay;
    public float mSendTime;
    public float mDuration;
    public float mExpiration;
    public bool mIsWaiting;

    public Mail(Order _order, Sprite _image, string _name, int _sendDay, float _duration)
    {
        mOrder = _order;
        mPicture = _image;
        mName = _name;
        mSendDay = _sendDay;
        mDuration = 1;
        mIsWaiting = false;
    }

    public void UpdateExpiration(Daylight daylight)
    {
        mExpiration = (mSendDay + mDuration) * daylight.mDayLength + mSendTime;
    }
}
