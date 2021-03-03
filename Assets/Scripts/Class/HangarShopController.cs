using NostalgiaOrbitDLL;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using NostalgiaOrbitDLL.Core;
using NostalgiaOrbitDLL.Ships;
using NostalgiaOrbitDLL.Drones;
using NostalgiaOrbitDLL.Resources;
using NostalgiaOrbitDLL.Items;

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

    [SerializeField]
    public SpriteAnimation ShopModelAnimation;

    [SerializeField]
    public Transform ContentItemsTransform;

    [SerializeField]
    public ItemPopup ItemPopup;

    private void OnEnable()
    {
        DisableAllShops();
    }

    public void Click(ShopItem shopItem)
    {
        DisableAllShops();

        InformationBackground.gameObject.SetEnable();

        // Todo Ship / Drones exists
        if (shopItem.CanBuyOnAuctionHouse)
        {
            AuctionButton.gameObject.SetEnable();
        }

        if (shopItem.CanBuyByCredit || shopItem.CanBuyUridium)
        {
            BuyButton.gameObject.SetEnable();
            BuyButton.onClick.AddListener(() =>
            {
                ItemPopup.Configure(shopItem);
            });
        }

        var shop = ShopItems.First(o => o.ItemType == shopItem.ItemShopType);
        shop.ItemInformation.gameObject.SetEnable();
        shop.ItemInformation.GetComponent<HangarShopItemInformationParent>().Configure(shopItem);

        ItemPopup.gameObject.SetDisable();
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

    public void Configure(HangarScreens screen)
    {
        foreach (Transform child in ContentItemsTransform)
        {
            child.gameObject.SetActive(ButtonStatus(screen, child.GetComponent<HangarShopItem>()));
        }
    }

    private bool ButtonStatus(HangarScreens screen, HangarShopItem hangarShopItem)
    {
        ShopItem shopItem = null;
        if (hangarShopItem.ItemType != ItemShopTypes.AmmunitionBuy)
        {
            shopItem = DLLHelpers.GetShopItem(hangarShopItem.ItemType);
        }

        return screen switch
        {
            HangarScreens.ships => shopItem is AbstractShip,
            HangarScreens.drones => shopItem is AbstractDrone,
            HangarScreens.weapons => (shopItem is AbstractItem abstractItem && abstractItem.IsLaser) || hangarShopItem.ItemType == ItemShopTypes.AmmunitionBuy,
            HangarScreens.ammunitions => shopItem is AbstractResource,
            HangarScreens.generators => shopItem is AbstractItem abstractItem && (abstractItem.IsShield || abstractItem.IsGear),
            HangarScreens.extras => shopItem is AbstractItem abstractItem && abstractItem.IsExtras,
            HangarScreens.boosters => false,
            HangarScreens.designs => false,
            _ => throw new NotImplementedException(screen.ToString())
        };
    }
}