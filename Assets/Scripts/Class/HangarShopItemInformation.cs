using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Ships;
using TMPro;
using UnityEngine;

public class HangarShopItemInformation : MonoBehaviour
{
    private ItemShopTypes ItemType;
    private ShopItem ShopItem;

    [SerializeField]
    public TMP_Text ItemPrice;

    [Header("Statistics")]
    [SerializeField]
    public TMP_Text HitpointsText;
    [SerializeField]
    public TMP_Text SpeedText;
    [SerializeField]
    public TMP_Text CargoText;
    [SerializeField]
    public TMP_Text LasersText;
    [SerializeField]
    public TMP_Text GeneratorsText;
    [SerializeField]
    public TMP_Text ExtrasText;
    [Header("Bonus statistics")]
    [SerializeField]
    public GameObject Bonus_Text;
    [SerializeField]
    public TMP_Text Bonus_HitpointsText;
    [SerializeField]
    public TMP_Text Bonus_SpeedText;
    [SerializeField]
    public TMP_Text Bonus_CargoText;
    [SerializeField]
    public TMP_Text Bonus_LasersText;
    [SerializeField]
    public TMP_Text Bonus_GeneratorsText;

    public void Configure(ShopItem shopItem)
    {
        ItemType = shopItem.ItemShopType;
        ShopItem = shopItem;

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

        if (ShopItem is AbstractShip abstractShip)
        {
            HitpointsText.text = abstractShip.Hitpoints.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);
            SpeedText.text = abstractShip.Speed.ToString();
            CargoText.text = abstractShip.CargoSize.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);
            LasersText.text = abstractShip.LaserSlots.ToString();
            GeneratorsText.text = abstractShip.ShieldAndGearSlots.ToString();
            ExtrasText.text = abstractShip.ExtrasSlots.ToString();

            if (abstractShip.Bonus_Statistics)
            {
                Bonus_HitpointsText.gameObject.SetEnable();
                Bonus_HitpointsText.text = abstractShip.Bonus_Hitpoints.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);
                Bonus_SpeedText.gameObject.SetEnable();
                Bonus_SpeedText.text = abstractShip.Bonus_Speed.ToString();
                Bonus_CargoText.gameObject.SetEnable();
                Bonus_CargoText.text = abstractShip.Bonus_CargoSize.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);
                Bonus_LasersText.gameObject.SetEnable();
                Bonus_LasersText.text = $"{abstractShip.Bonus_LasersDamageMultiplyInShip * 100} %";
                Bonus_GeneratorsText.gameObject.SetEnable();
                Bonus_GeneratorsText.text = $"{abstractShip.Bonus_ShieldMultiplyInShip * 100} %";
            }
        }
    }
}