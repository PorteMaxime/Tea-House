using UnityEngine;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour
{
    public ItemData itemData;
    Image image;
    public GameObject infoPage;
    private void Start()
    {
        image = GetComponent<Image>();
    }
    public void UpdateSlot(Sprite lockedItem)
    {
        if (itemData.mIsUnlocked == true)
        {
            image.sprite = itemData.sprite;
        }
        else
        {
            image.sprite = lockedItem;
        }
    }

    public void OpenInfoPage()
    {
        Debug.Log(infoPage);
        infoPage.GetComponent<Image>().sprite = itemData.infoPage;
        infoPage.SetActive(true);
    }
}
