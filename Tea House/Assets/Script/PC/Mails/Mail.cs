using SQLite;
using UnityEngine;

public class Mail
{
    public static int mailID = 0;

    [PrimaryKey]
    [Column("id")]
    public int id { get; set; }

    public Sprite mPicture;
    [Column("pictureName")]
    public string mPictureName { get; set; }

    [Column("name")]
    public string mName { get; set; }

    [Column("text")]
    public string mText { get; set; }

    [Column("sendDay")]
    public int mSendDay { get; set; }

    [Column("sendTime")]
    public float mSendTime { get; set; }

    [Column("duration")]
    public int mDuration { get; set; } = 0;

    [Column("expiration")]
    public float mExpiration { get; set; }

    [Column("isRead")]
    public bool mIsRead { get; set; } = false;

    public Order mOrder;

    [HideInInspector] public bool mIsPrinted = false;

    public Mail()
    {
        id = mailID;

        mailID += 1;
    }

    public override string ToString()
    {
        return $"=== DEBUG MAIL (ID: {id}) ===\n" +
            $"• Name: {mName}\n" +
            $"• Text: {mText}\n" +
            $"• Send Day: {mSendDay} | Send Time: {mSendTime}\n" +
            $"• Duration: {mDuration} | Expiration: {mExpiration}\n" +
            $"• Has Order: {(mOrder != null ? "Yes" : "No")}\n" +
            $"• Has Sprite: {(mPicture != null ? "Yes" : "No")}\n" +
            $"=============================";
    }

    public void UpdateExpiration(Daylight daylight)
    {
        mExpiration = (mSendDay + mDuration) * daylight.mDayLength + mSendTime;
    }

    public void SetOrder(Order _order)
    {
        mOrder = _order;
        mOrder.mailId = id;
    }
}
