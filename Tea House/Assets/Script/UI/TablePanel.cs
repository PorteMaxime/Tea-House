using System.Collections;
using TMPro;
using UnityEngine;

public class TablePanel : MonoBehaviour
{
    public TableScript mTableScript;
    public TMP_Text mPopupSuppr;
    public void OnUpdateUI()
    {
        if (gameObject.activeSelf == false)
            return;

    }

    public void OpenDeliveryPage()
    {

    }
    public void RemovePacket(int index)
    {
        if(index > mTableScript.mPackets.Count - 1)
            return;

        StartCoroutine(SupprPacket(index));
    }

    IEnumerator SupprPacket(int index)
    {
        Destroy(mTableScript.mPackets[index]);
        mPopupSuppr.text = "Packet " + index + " suppr !";
        mPopupSuppr.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        mPopupSuppr.gameObject.SetActive(false);
    }
}
