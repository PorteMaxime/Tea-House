using UnityEngine;
using TMPro;

public class MainPanel : MonoBehaviour
{
    public TMP_Text mMoneyAmount;
    public TMP_Text mTimer;
    public TMP_Text mNote;

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
}
