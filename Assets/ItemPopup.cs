using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Ships;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopup : MonoBehaviour
{
    private ShopItem ShopItem;

    [SerializeField]
    public HangarShopItemInformation HangarShopItemInformation;
    [SerializeField]
    public TMP_Text ItemNameText;
    [SerializeField]
    public TMP_Text ItemPriceText;

    [SerializeField]
    public Button AuctionButton;
    [SerializeField]
    public Button BuyButton;

    [SerializeField]
    public GameObject ShipInfoText;

    [SerializeField]
    public Color TextColor;

    public void Configure(ShopItem shopItem, bool fromAuction = false)
    {
        ShopItem = shopItem;

        HangarShopItemInformation.ConfigureColor(TextColor);
        HangarShopItemInformation.Configure(shopItem);

        ItemNameText.text = shopItem.ItemShopType.ToString();

        ItemPriceText.text = Helpers.GetItemPrice(shopItem);

        if (shopItem.CanBuyOnAuctionHouse && !fromAuction)
        {
            AuctionButton.gameObject.SetEnable();
        }

        if (shopItem.CanBuyByCredit || shopItem.CanBuyUridium)
        {
            BuyButton.gameObject.SetEnable();
        }

        if (shopItem is AbstractShip)
        {
            ShipInfoText.SetEnable();
        }
        else
        {
            ShipInfoText.SetDisable();
        }

        gameObject.SetEnable();
    }

    public void Buy()
    {
        Debug.LogWarning($"Buy {ShopItem.ItemShopType}");
    }
}