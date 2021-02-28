using NostalgiaOrbitDLL;
using TMPro;
using UnityEngine;

public class HangarShopItemInformationParent : MonoBehaviour
{
    private ItemShopTypes ItemType;
    private ShopItem ShopItem;

    [SerializeField]
    public TMP_Text ItemPrice;

    [SerializeField]
    public HangarShopItemInformation Child;

    public void Configure(ShopItem shopItem)
    {
        ItemType = shopItem.ItemShopType;
        ShopItem = shopItem;

        if (ShopItem.CanBuyByCredit)
        {
            var price = ShopItem.CreditPurchase[0];
            ItemPrice.text = price > 0 ? price.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat) + " C." : "0 C.";
        }
        else if (ShopItem.CanBuyUridium)
        {
            var price = ShopItem.UridiumPurchase[0];
            ItemPrice.text = price > 0 ? price.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat) + " U." : "0 U.";
        }
        else
        {
            ItemPrice.text = "-";
        }

        Child.Configure(shopItem);
    }
}