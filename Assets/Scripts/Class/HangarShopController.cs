using NostalgiaOrbitDLL;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[Serializable]
public class HangarShopController : MonoBehaviour
{
    [SerializeField]
    public HangarShopManager[] ShopItems;

    [SerializeField]
    public Transform InformationBackground;

    [SerializeField]
    public Button AuctionButton;

    [SerializeField]
    public Button BuyButton;

    private void Start()
    {
        DisableAllShops();
    }

    public void Click(ShopItem shopItem)
    {
        DisableAllShops();

        InformationBackground.gameObject.SetEnable();

        if (shopItem.CanBuyOnAuctionHouse)
        {
            AuctionButton.gameObject.SetEnable();
        }

        if (shopItem.CanBuyByCredit || shopItem.CanBuyUridium)
        {
            BuyButton.gameObject.SetEnable();
        }

        var shop = ShopItems.First(o => o.ItemType == shopItem.ItemShopType);
        shop.ItemInformation.gameObject.SetEnable();
        shop.ItemInformation.GetComponent<HangarShopItemInformation>().Configure(shopItem);
    }

    private void DisableAllShops()
    {
        InformationBackground.gameObject.SetDisable();
        AuctionButton.gameObject.SetDisable();
        BuyButton.gameObject.SetDisable();

        foreach (var child in ShopItems)
        {
            child.ItemInformation.gameObject.SetDisable();
        }
    }
}