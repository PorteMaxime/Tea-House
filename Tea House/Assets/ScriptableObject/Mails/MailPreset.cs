using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMailPreset", menuName = "Scriptable Objects/MailPreset")]
public class MailPreset : ScriptableObject
{
    [SerializeField] private string mName = "Bob";

    [SerializeField] private List<StatValue> mData = new List<StatValue>();

    [SerializeField] [TextArea] private string mText = "Hi. I'm Bob.";
    [SerializeField] private int mDuration = 0;

    public int mPresetLevel = 1;

    public Mail CreateMail()
    {
        Mail mail = new Mail();

        mail.mName = mName;
        mail.mText = mText;
        mail.mDuration = mDuration;

        CreateOrder(mail);

        return mail;
    }

    private void CreateOrder(Mail mail)
    {
        Order order = new Order();

        order.mOrderData = mData;

        mail.SetOrder(order);
    }
}