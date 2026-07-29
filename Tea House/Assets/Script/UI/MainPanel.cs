using UnityEngine;
using TMPro;

public class MainPanel : MonoBehaviour
{
    public TMP_Text mMoneyAmount;
    public TMP_Text mTimer;
    public TMP_Text mNote;

    private int newMailsCount = 0;
    [SerializeField] private GameObject mImageNewMail;

    private Daylight mDayLight;
    private void Start()
    {
        mDayLight = GameObject.FindGameObjectWithTag("SunLight").GetComponent<Daylight>();
    }

    public void OnUpdateUI()
    {
        if (gameObject.activeSelf == false)
            return;
        mMoneyAmount.text = Inventory.msInstance.mMoneyAmount.ToString();
        mTimer.text = mDayLight.GetInGameTime();
    }

    public void AddNewMail()
    {
        if (newMailsCount <= 0)
            mImageNewMail.SetActive(true);

        newMailsCount += 1;
    }

    public void RemoveNewMail()
    {
        newMailsCount -= 1;

        if (newMailsCount <= 0)
            mImageNewMail.SetActive(false);
    }
}
