using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ShopPageScript : MonoBehaviour
{
	[SerializeField] public List<ItemData> mHardCodedItems = new List<ItemData>();
	[SerializeField] public Vector2 mShopPageSize = new Vector2(4, 2);

	public Dictionary<ItemData, (GameObject ShopItem, GameObject cartItem, int mAmount)> mShopItems = new Dictionary<ItemData, (GameObject, GameObject, int)>();

	public List<GameObject> mShopPages = new List<GameObject>();
	public GameObject mCurrentPage = null;

	private GameObject mCanvasCart;
	private GameObject mCanvasCartItems;
	private GameObject mCanvasShopItems;

	private GameObject mShopPagePrefab = null;
	private GameObject mItemsRowPrefab = null;
	private GameObject mShopItemPrefab = null;

	private GameObject mCartItemPrefab = null;

	private void Awake()
	{
		Transform temp = transform.Find("CanvasCart");
		if (temp != null)
			mCanvasCart = temp.gameObject;

		temp = transform.Find("CanvasCart/Popup/ScrollView/Viewport/ContentCartItems");
		if (temp != null)
            mCanvasCartItems = temp.gameObject;

		temp = transform.Find("CanvasShopContent/CanvasShopItems");
		if (temp != null)
            mCanvasShopItems = temp.gameObject;

		mShopPagePrefab = Resources.Load<GameObject>("Shop/CanvasShopPage");
		mItemsRowPrefab = Resources.Load<GameObject>("Shop/CanvasRowContent");
		mShopItemPrefab = Resources.Load<GameObject>("Shop/CanvasShopItem");
	
		mCartItemPrefab = Resources.Load<GameObject>("Shop/CanvasCartItem");

		CreateItems();
	}

	private void OnDisable()
	{
		ToggleCart(false);

		if (mShopPages.Count > 0)
		{
			mCurrentPage.gameObject.SetActive(false);
			mCurrentPage = mShopPages[0];
			mCurrentPage.gameObject.SetActive(true);
		}
	}

	private void CreateItems()
	{
		int itemNumber = 0;
		GameObject creationPage = null;
		GameObject creationRow = null;
		GameObject creationItem = null;

		foreach (var item in mHardCodedItems)
		{
			// Nouvelle page
			if (itemNumber % (mShopPageSize.x * mShopPageSize.y) == 0)
			{
                creationPage = Instantiate(mShopPagePrefab, mCanvasShopItems.transform, false);

                if (mCurrentPage == null)
					mCurrentPage = creationPage;
				else
                    creationPage.SetActive(false);
				mShopPages.Add(creationPage);

				// Setup boutons page suivante et page precedente
				Button buttonLeft = creationPage.GetComponent<Transform>().Find("ButtonLeft").GetComponent<Button>();
                Button buttonRight = creationPage.GetComponent<Transform>().Find("ButtonRight").GetComponent<Button>();

				if (buttonLeft == null || buttonRight == null)
					break;

                if (mShopPages.Count == 1)
					buttonLeft.gameObject.SetActive(false);
                else
				{
                    buttonLeft.onClick.AddListener(() => ChangePage(-1));

                    Button lastButtonRight = mShopPages[mShopPages.Count - 2].GetComponent<Transform>().Find("ButtonRight").GetComponent<Button>();

					if (lastButtonRight != null)
					{
						lastButtonRight.gameObject.SetActive(true);
						lastButtonRight.onClick.AddListener(() => ChangePage(1));
					}
                }

                buttonRight.gameObject.SetActive(false);
            }
			// Nouvelle ligne dans la page
			if (itemNumber % mShopPageSize.x == 0)
            {
				Canvas canvasContent = creationPage.GetComponent<Transform>().Find("CanvasContent").GetComponent<Canvas>();

				if (canvasContent != null)
                    creationRow = Instantiate(mItemsRowPrefab, canvasContent.transform, false);
            }

            // Ajout d'un item à la ligne actuelle

            creationItem = Instantiate(mShopItemPrefab, creationRow.transform, false);
			mShopItems.Add(item, (creationItem, null, 0));

            Button changeAmount = creationItem.GetComponent<Transform>().Find("CanvasAddToCart/ButtonAddToCart").GetComponent<Button>();
            changeAmount.onClick.AddListener(() => ChangeItemAmount(item, 1));

            changeAmount = creationItem.GetComponent<Transform>().Find("CanvasAddRemove/ButtonRemove").GetComponent<Button>();
            changeAmount.onClick.AddListener(() => ChangeItemAmount(item, -1));

            changeAmount = creationItem.GetComponent<Transform>().Find("CanvasAddRemove/ButtonAdd").GetComponent<Button>();
            changeAmount.onClick.AddListener(() => ChangeItemAmount(item, 1));

            itemNumber += 1;
		}
	}

	public void ChangePage(int amount)
	{
		if (mShopPages.Count <= 1)
			return;

		mCurrentPage.gameObject.SetActive(false);

		mCurrentPage = mShopPages[Math.Max(0, Math.Min(mShopPages.Count - 1, mShopPages.IndexOf(mCurrentPage) + amount))];

		mCurrentPage.gameObject.SetActive(true);
	}


	public void ChangeItemAmount(ItemData item, int amount)
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

                Button changeAmount = tempItem.cartItem.GetComponent<Transform>().Find("ButtonRemove").GetComponent<Button>();
                changeAmount.onClick.AddListener(() => ChangeItemAmount(item, -1));

                changeAmount = tempItem.cartItem.GetComponent<Transform>().Find("ButtonAdd").GetComponent<Button>();
                changeAmount.onClick.AddListener(() => ChangeItemAmount(item, 1));
            }
            else
				tempItem.cartItem.SetActive(true);

            amountText = tempItem.cartItem.GetComponent<Transform>().Find("TextAmount").GetComponent<Text>();

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

        mShopItems[item] = tempItem;
    }
    public void SetItemAmount(ItemData item, int amount)
	{
		if (!mShopItems.ContainsKey(item) || amount < 0)
			return;

		ChangeItemAmount(item, amount - mShopItems[item].mAmount);
	}

    public void ToggleCart(bool active)
	{
		mCanvasCart.SetActive(active);
	}

	public void BuyCart()
	{
		if (CanBuyCartContent() == true)
		{
			// modifier argent
			// ajouter cart à inventaire

			EmptyCart();

            ToggleCart(false);
        }
		else
		{
			// popup "pas assez d'argent"
		}
	}

	private void EmptyCart()
	{
        foreach (ItemData itemData in mShopItems.Keys.Reverse())
            SetItemAmount(itemData, 0);
    }

	bool CanBuyCartContent()
	{
		return true;
	}
}

/* A faire :
 * - mettre liste d'items dans inventaire ? -> mHardCodedItems
 * - ajouter contenu cart à l'inventaire -> BuyCart()
 * - condition possibilité d'acheter -> CanBuyCartContent()
 * - Mettre l'image de l'item dans le shop et le panier -> CreateItems() + ChangeItemAmount()
 * - ...
*/
