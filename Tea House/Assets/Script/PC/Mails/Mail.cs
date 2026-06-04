using UnityEngine;
using UnityEngine.UI;

public class Mail
{
    public Order mOrder;
    public Sprite mPicture;
    public string mName;
    public string mText;
    public int mSendDay;
    public int mDuration;
    public bool mIsWaiting;

    public Mail(Order _order, Sprite _image, string _name, string _text, int _sendDay, float _duration)
    {
        mOrder = _order;
        mPicture = _image;
        mName = _name;
        mText = _text;
        mSendDay = _sendDay;
        mDuration = 1;
        mIsWaiting = false;
    }
}
