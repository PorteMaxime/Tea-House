using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPageScript : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    
    [SerializeField] public Vector2 mPageSize = new Vector2(4, 2);

    public Dictionary<ItemData, GameObject> mInventoryItems = new Dictionary<ItemData, GameObject>();

    public List<GameObject> mPages = new List<GameObject>();
    public GameObject mCurrentPage = null;

    private GameObject mPagePrefab = null;
    private GameObject mRowPrefab = null;
    private GameObject mInventoryItemPrefab = null;

    private GameObject mCanvasInventoryItems;

    public void Awake()
    {
        mPagePrefab = Resources.Load<GameObject>("PC/CanvasPage");
        mRowPrefab = Resources.Load<GameObject>("PC/CanvasRow");
        mInventoryItemPrefab = Resources.Load<GameObject>("PC/Inventory/CanvasInventoryItem");

        Transform temp = transform.Find("CanvasInventoryItems");
        if (temp != null)
            mCanvasInventoryItems = temp.gameObject;

        CreateMissingItems();
    }

    private void OnEnable()
    {
        CreateMissingItems();

        UpdateItemsAmounts();
    }

    private void CreateMissingItems()
    {
        if (Inventory.msInstance == null)
            return;

        int itemNumber = mInventoryItems.Count;
        GameObject creationPage = null;
        GameObject creationRow = null;
        GameObject creationItem = null;

        foreach (ItemData item in Inventory.msInstance.GetItemsOfType<ItemData>())
        {
            if (mInventoryItems.ContainsKey(item))
            {
                itemNumber += 1;
                continue;
            }

            // Nouvelle page
            if (itemNumber % (mPageSize.x * mPageSize.y) == 0)
            {
                creationPage = Instantiate(mPagePrefab, mCanvasInventoryItems.transform, false);

                if (mCurrentPage == null)
                    mCurrentPage = creationPage;
                else
                    creationPage.SetActive(false);
                mPages.Add(creationPage);

                // Setup boutons page suivante et page precedente
                Button buttonLeft = creationPage.transform.Find("ButtonLeft").GetComponent<Button>();
                Button buttonRight = creationPage.transform.Find("ButtonRight").GetComponent<Button>();

                if (buttonLeft == null || buttonRight == null)
                    break;

                if (mPages.Count == 1)
                    buttonLeft.gameObject.SetActive(false);
                else
                {
                    buttonLeft.onClick.AddListener(() => ChangePage(-1));

                    Button lastButtonRight = mPages[mPages.Count - 2].transform.Find("ButtonRight").GetComponent<Button>();

                    if (lastButtonRight != null)
                    {
                        lastButtonRight.gameObject.SetActive(true);
                        lastButtonRight.onClick.AddListener(() => ChangePage(1));
                    }
                }

                buttonRight.gameObject.SetActive(false);
            }
            // Nouvelle ligne dans la page
            if (itemNumber % mPageSize.x == 0)
            {
                Canvas canvasContent = creationPage.transform.Find("CanvasContent").GetComponent<Canvas>();

                if (canvasContent != null)
                    creationRow = Instantiate(mRowPrefab, canvasContent.transform, false);
            }

            // Ajout d'un item à la ligne actuelle
            creationItem = Instantiate(mInventoryItemPrefab, creationRow.transform, false);
            mInventoryItems.Add(item, creationItem);

            GameObject image = creationItem.transform.Find("ImageItemOutside/ImageItem").gameObject;
            if (image != null)
                image.GetComponent<Image>().sprite = item.mIcon;

            GameObject ItemText = creationItem.transform.Find("TextName").gameObject;
            if (ItemText != null)
                ItemText.GetComponent<Text>().text = item.mName;

            if (itemNumber % mPageSize.x == 0)
            {
                float itemWidth = creationItem.GetComponent<RectTransform>().rect.width;

                for (int i = 1; i < mPageSize.x; i++)
                {
                    GameObject filler = new GameObject("filler" + i, typeof(RectTransform));
                    filler.transform.parent = creationRow.transform;
                    filler.GetComponent<RectTransform>().sizeDelta = new Vector2(itemWidth, 1);
                }
            }
            else
            {
                Destroy(creationRow.transform.Find("filler" + itemNumber % mPageSize.x).gameObject);
                creationItem.transform.SetSiblingIndex((int)(itemNumber % mPageSize.x));
            }

            itemNumber += 1;
        }
    }

    public void ChangePage(int amount)
    {
        if (mPages.Count <= 1)
            return;

        mCurrentPage.gameObject.SetActive(false);

        mCurrentPage = mPages[Math.Max(0, Math.Min(mPages.Count - 1, mPages.IndexOf(mCurrentPage) + amount))];

        mCurrentPage.gameObject.SetActive(true);
    }

    public void UpdateItemsAmounts()
    {
        Inventory inv = Inventory.msInstance;

        if (inv == null)
            return;

        Text textAmount;
        foreach (var item in mInventoryItems.Keys)
        {
            textAmount = mInventoryItems[item].transform.Find("TextAmount").GetComponent<Text>();

            if (textAmount != null)
                textAmount.text = "Stock : " + inv.GetAmount(item);
        }
    }
}
