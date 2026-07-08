using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopPageScript : MonoBehaviour
{
	[SerializeField] private Vector2 mPageSize = new Vector2(4, 2);

	private Dictionary<ItemData, (GameObject ShopItem, GameObject cartItem, int mAmount)> mShopItems = new Dictionary<ItemData, (GameObject, GameObject, int)>();

    private GameObject mCanvasShopItems;
	private GameObject mCurrentPage = null;
    private List<GameObject> mPages = new List<GameObject>();
    private Text mCurrentMoney;

    private GameObject mPagePrefab = null;
	private GameObject mRowPrefab = null;
	private GameObject mShopItemPrefab = null;

    private GameObject mCanvasCart;
    private GameObject mCanvasCartItems;
    private Text mCartMoneyNeeded;
    private GameObject mTextNotEnoughMoney;

    private GameObject mCartItemPrefab = null;

	private int mCartCost;

    private void Awake()
    {
        Transform temp = transform.Find("CanvasShopItems");
        if (temp != null)
            mCanvasShopItems = temp.gameObject;

        temp = transform.Find("TextMoney");
        if (temp != null)
            mCurrentMoney = temp.gameObject.GetComponent<Text>();

        mPagePrefab = Resources.Load<GameObject>("PC/CanvasPage");
        mRowPrefab = Resources.Load<GameObject>("PC/CanvasRow");
        mShopItemPrefab = Resources.Load<GameObject>("PC/Shop/CanvasShopItem");

        temp = transform.Find("CanvasCart");
        if (temp != null)
            mCanvasCart = temp.gameObject;

        temp = transform.Find("CanvasCart/Popup/ScrollView/Viewport/ContentCartItems");
        if (temp != null)
            mCanvasCartItems = temp.gameObject;

        temp = transform.Find("CanvasCart/Popup/TextMoney");
        if (temp != null)
            mCartMoneyNeeded = temp.gameObject.GetComponent<Text>();

        temp = transform.Find("CanvasCart/Popup/TextNotEnoughMoney");
        if (temp != null)
            mTextNotEnoughMoney = temp.gameObject;

        mCartItemPrefab = Resources.Load<GameObject>("PC/Shop/CanvasCartItem");

        ToggleCart(false);

        CreateMissingItems();
    }

    private void OnEnable()
    {
        CreateMissingItems();

		UpdateMoneyTexts();
    }

    private void OnDisable()
	{
		ToggleCart(false);

		if (mPages.Count > 0 && mCurrentPage != mPages[0])
		{
			mCurrentPage.gameObject.SetActive(false);
			mCurrentPage = mPages[0];
			mCurrentPage.gameObject.SetActive(true);
		}
	}

	private void CreateMissingItems()
	{
		if (Inventory.msInstance == null)
			return;

        int itemNumber = mShopItems.Count;
		GameObject creationPage = null;
		GameObject creationRow = null;
		GameObject creationItem = null;

        foreach (ItemData item in Inventory.msInstance.GetItemsOfType<ItemData>())
		{
			if (mShopItems.ContainsKey(item))
			{
				itemNumber += 1;
				continue;
			}

			// Nouvelle page
			if (itemNumber % (mPageSize.x * mPageSize.y) == 0)
            {
                creationPage = Instantiate(mPagePrefab, mCanvasShopItems.transform, false);

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
            creationItem = Instantiate(mShopItemPrefab, creationRow.transform, false);
			mShopItems.Add(item, (creationItem, null, 0));

			Transform priceText = creationItem.transform.Find("TextPrice");
			if (priceText != null)
				priceText.GetComponent<Text>().text = item.mPrice + " $";

            Transform changeAmount = creationItem.transform.Find("CanvasAddToCart/ButtonAddToCart");
            if (changeAmount != null)
                changeAmount.GetComponent<Button>().onClick.AddListener(() => ChangeCartItemAmount(item, 1));

            changeAmount = creationItem.transform.Find("CanvasAddRemove/ButtonRemove");
            if (changeAmount != null)
                changeAmount.GetComponent<Button>().onClick.AddListener(() => ChangeCartItemAmount(item, -1));

            changeAmount = creationItem.transform.Find("CanvasAddRemove/ButtonAdd");
            if (changeAmount != null)
                changeAmount.GetComponent<Button>().onClick.AddListener(() => ChangeCartItemAmount(item, 1));

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

	public void ChangeCartItemAmount(ItemData item, int amount)
	{
		if (!mShopItems.ContainsKey(item))
			return;

		if (mShopItems[item].mAmount + amount < 0)
			return;

		// Modification quantite
        (GameObject ShopItem, GameObject cartItem, int mAmount) tempItem = mShopItems[item];
        tempItem.mAmount = Math.Max(0, tempItem.mAmount + amount);

        if (tempItem.mAmount >  0)
		{
			// Dans le shop
			Transform shopItemTransform = mShopItems[item].ShopItem.transform;

            GameObject addToCart = shopItemTransform.Find("CanvasAddToCart").gameObject;
            addToCart.SetActive(false);

            GameObject addRemove = shopItemTransform.Find("CanvasAddRemove").gameObject;
			addRemove.SetActive(true);

            Text amountText = shopItemTransform.Find("CanvasAddRemove/TextAmount").GetComponent<Text>();

            amountText.text = tempItem.mAmount.ToString();

            // Dans le panier
            if (mShopItems[item].cartItem == null)
			{
                tempItem.cartItem = Instantiate(mCartItemPrefab, mCanvasCartItems.transform, false);

                Button changeAmount = tempItem.cartItem.transform.Find("ButtonRemove").GetComponent<Button>();
                changeAmount.onClick.AddListener(() => ChangeCartItemAmount(item, -1));

                changeAmount = tempItem.cartItem.transform.Find("ButtonAdd").GetComponent<Button>();
                changeAmount.onClick.AddListener(() => ChangeCartItemAmount(item, 1));
            }
            else
				tempItem.cartItem.SetActive(true);

            amountText = tempItem.cartItem.transform.Find("TextAmount").GetComponent<Text>();

            amountText.text = tempItem.mAmount.ToString();
        }
		else
        {
            // Dans le shop
            Transform shopItemTransform = mShopItems[item].ShopItem.transform;

            GameObject addToCart = shopItemTransform.Find("CanvasAddToCart").gameObject;
            addToCart.SetActive(true);

            GameObject addRemove = shopItemTransform.Find("CanvasAddRemove").gameObject;
            addRemove.SetActive(false);

			// Dans le panier
			if (tempItem.cartItem != null)
	            tempItem.cartItem.SetActive(false);
        }

		mCartCost += (tempItem.mAmount - mShopItems[item].mAmount) * item.mPrice;

		UpdateMoneyTexts();

        mShopItems[item] = tempItem;
    }

    public void SetCartItemAmount(ItemData item, int amount)
	{
		if (!mShopItems.ContainsKey(item) || amount < 0)
			return;

        ChangeCartItemAmount(item, amount - mShopItems[item].mAmount);
	}

	private void UpdateMoneyTexts()
    {
        if (mTextNotEnoughMoney != null && mTextNotEnoughMoney.activeSelf == true)
            mTextNotEnoughMoney.SetActive(false);

		if (Inventory.msInstance == null)
			return;

        if (mCartMoneyNeeded != null)
	        mCartMoneyNeeded.text = Inventory.msInstance.mMoneyAmount + "$ / " + mCartCost + "$";

		if (mCurrentMoney != null)
            mCurrentMoney.text = Inventory.msInstance.mMoneyAmount + "$";

    }

    public void ToggleCart(bool active)
	{
		UpdateMoneyTexts();

        mCanvasCart.SetActive(active);
	}

	public void BuyCart()
	{
		if (CanBuyCartContent() == true)
		{
			Inventory.msInstance.mMoneyAmount -= mCartCost;

			foreach (ItemData item in mShopItems.Keys)
			{
				Inventory.msInstance.ChangeItemAmount(item, mShopItems[item].mAmount);
                Debug.Log("card : " + mShopItems[item].mAmount);
            }

			EmptyCart();

            ToggleCart(false);
        }
		else
		{
			if (mTextNotEnoughMoney  != null)
                mTextNotEnoughMoney.SetActive(true);

        }
	}

	private void EmptyCart()
	{
        foreach (ItemData itemData in mShopItems.Keys.Reverse())
            SetCartItemAmount(itemData, 0);
    }

	bool CanBuyCartContent()
	{
		return mCartCost <= Inventory.msInstance.mMoneyAmount;
	}
}

/* A faire :
 * - Mettre l'image de l'item dans le shop et le panier -> CreateItems() + ChangeCartItemAmount()
 * - ...
*/
