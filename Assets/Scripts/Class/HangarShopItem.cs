using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core;
using TMPro;
using UnityEngine;

public class HangarShopItem : MonoBehaviour
{
    [SerializeField]
    public ItemShopTypes ItemType;
    private ShopItem ShopItem;

    [SerializeField]
    public TMP_Text ItemName;
    [SerializeField]
    public TMP_Text ItemPrice;
    [SerializeField]
    public Transform InformationTransform;

    [SerializeField]
    public HangarShopController ShopController;
    
    public void Click()
    {
        ShopController.Click(ShopItem ?? DLLHelpers.GetShopItem(ItemType));
    }

    public void Configure(ItemShopTypes itemShopType)
    {
        ShopItem = DLLHelpers.GetShopItem(itemShopType);

        ItemName.text = ItemType.ToString();

        if (ShopItem.CanBuyByCredit)
        {
            ItemPrice.text = ShopItem.CreditPurchase[0].ToString(Helpers.ThousandSeparator, Helpers.NumberFormat) + " C.";
        }
        else if (ShopItem.CanBuyUridium)
        {
            ItemPrice.text = ShopItem.UridiumPurchase[0].ToString(Helpers.ThousandSeparator, Helpers.NumberFormat) + " U.";
        }
        else
        {
            ItemPrice.text = "-";
        }
    }
}