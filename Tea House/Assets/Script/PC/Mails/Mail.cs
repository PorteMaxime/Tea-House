using SQLite;
using UnityEngine;

public class Mail
{
    [Column("id")]
    public int id { get; set; }

    public static int mailID = 0;

    public Order mOrder;
    public Sprite mPicture;

    [Column("")]
    public string mName { get; set; }
    [Column("")]
    public string mText { get; set; }
    [Column("")]
    public int mSendDay { get; set; }
    [Column("")]
    public float mSendTime { get; set; }
    [Column("")]
    public int mDuration { get; set; } = 0;
    [Column("")]
    public float mExpiration { get; set; }

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
    }
}
