using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopPageScript : MonoBehaviour
{
    //public List<> mShopItems;
    //public Dictionary<IngredientData, int> mShoppingCart;

    private GameObject mCanvasCart;

    private void Awake()
    {
        Transform temp = transform.Find("CanvasCart");
        if (temp != null)
            mCanvasCart = temp.gameObject;
    }

    private void OnDisable()
    {
        ToggleCart(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateItems()
    {
        // créer items shop
    }

    public void ToggleCart(bool active)
    {
        mCanvasCart.SetActive(active);
    }

    bool CanBuyCartContent()
    {
        return true; // modifier
    }
}
