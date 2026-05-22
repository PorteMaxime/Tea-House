using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopPageScript : MonoBehaviour
{
    //public List<> mShopItems;

    public Dictionary<IngredientData, int> mShoppingCart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateItems()
    {

    }

    bool CanBuyCartContent()
    {
        return true;
    }
}
