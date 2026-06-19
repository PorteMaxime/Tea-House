using UnityEngine;


public class Mail
{
    public int id;

    public static int mailID = 0;

    public Order mOrder;
    public Sprite mPicture;
    public string mName;
    public string mText;
    public int mSendDay;
    public float mSendTime;
    public int mDuration;
    public float mExpiration;

    public Mail()
    {
        id = mailID;
    }

    public void InitData(Order _order, Sprite _image, string _name, int _sendDay, int _duration)
    {
        mOrder = _order;
        mPicture = _image;
        mName = _name;
        mSendDay = _sendDay;
        mDuration = _duration;
    }

    public void UpdateExpiration(Daylight daylight)
    {
        mExpiration = (mSendDay + mDuration) * daylight.mDayLength + mSendTime;

        mText += "\nExiration " + mExpiration;
    }
}
