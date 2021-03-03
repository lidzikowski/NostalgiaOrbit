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

        ItemPrice.text = Helpers.GetItemPrice(shopItem);

        Child.Configure(shopItem);
    }
}