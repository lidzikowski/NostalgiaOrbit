using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core;
using NostalgiaOrbitDLL.Drones;
using NostalgiaOrbitDLL.Items;
using NostalgiaOrbitDLL.Resources;
using NostalgiaOrbitDLL.Ships;
using System.Collections.Generic;
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
    public HangarShopController ShopController;

    [Header("OnEnable")]
    [SerializeField]
    public Transform ShowTransform;

    private void OnEnable()
    {
        Configure();

        if (ShowTransform != null)
            ShowTransform.gameObject.SetEnable();
    }

    private void OnDisable()
    {
        if (ShowTransform != null)
            ShowTransform?.gameObject.SetDisable();
    }

    public void Click()
    {
        ShopController.Click(ShopItem ?? DLLHelpers.GetShopItem(ItemType));

        // Ship model
        if (ShopItem is AbstractShip abstractShip)
        {
            var shipType = DLLHelpers.ConfigureShipType(abstractShip.ShipType, new List<AbstractItem>());
            ShopController.ShopModelAnimation.ChangePrefabModel(shipType);
        }
        else if (ShopItem is AbstractDrone abstractDrone)
        {
            ShopController.ShopModelAnimation.ChangePrefabModel(DLLHelpers.GetDronePrefab(abstractDrone.DroneType, 1));
        }
        else if (ShopItem.ItemShopType == ItemShopTypes.REP_1 || ShopItem.ItemShopType == ItemShopTypes.REP_2 || ShopItem.ItemShopType == ItemShopTypes.REP_3)
        {
            ShopController.ShopModelAnimation.ChangePrefabModel(PrefabTypes.Repair_robot);
        }
    }

    private void Configure()
    {
        if (ItemType == ItemShopTypes.AmmunitionBuy)
            return;

        ShopItem = DLLHelpers.GetShopItem(ItemType);

        //ItemName.text = ItemType.ToString();

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
    }
}