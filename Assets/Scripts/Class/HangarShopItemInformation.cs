using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Drones;
using NostalgiaOrbitDLL.Items;
using NostalgiaOrbitDLL.Resources;
using NostalgiaOrbitDLL.Ships;
using System;
using TMPro;
using UnityEngine;

public class HangarShopItemInformation : MonoBehaviour
{
    private ItemShopTypes ItemType;
    private ShopItem ShopItem;

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

    [Header("Drones")]
    [SerializeField]
    public TMP_Text FlaxText;
    [SerializeField]
    public TMP_Text IrisText;

    [Header("Lasers")]
    [SerializeField]
    public TMP_Text Lf1Text;
    [SerializeField]
    public TMP_Text Mp1Text;
    [SerializeField]
    public TMP_Text Lf2Text;
    [SerializeField]
    public TMP_Text Lf3Text;

    [Header("Ammunitions")]
    [SerializeField]
    public TMP_Text Ammo1Text;
    [SerializeField]
    public TMP_Text Ammo2Text;
    [SerializeField]
    public TMP_Text Ammo3Text;
    [SerializeField]
    public TMP_Text AmmoSabText;

    [Header("Rockets")]
    [SerializeField]
    public TMP_Text Rocket1Text;
    [SerializeField]
    public TMP_Text Rocket2Text;
    [SerializeField]
    public TMP_Text Rocket3Text;
    [SerializeField]
    public TMP_Text MineText;

    [Header("Gears")]
    [SerializeField]
    public TMP_Text G3N_1010Text;
    [SerializeField]
    public TMP_Text G3N_6900Text;
    [SerializeField]
    public TMP_Text G3N_7900Text;

    [Header("Shields")]
    [SerializeField]
    public TMP_Text SG3N_A01Text;
    [SerializeField]
    public TMP_Text SG3N_A02Text;
    [SerializeField]
    public TMP_Text SG3N_B01Text;
    [SerializeField]
    public TMP_Text SG3N_B02Text;

    [Header("Extras")]
    [SerializeField]
    public TMP_Text REP_1Text;
    [SerializeField]
    public TMP_Text REP_2Text;
    [SerializeField]
    public TMP_Text REP_3Text;

    public void Configure(ShopItem shopItem)
    {
        ItemType = shopItem.ItemShopType;
        ShopItem = shopItem;

        if (ShopItem is AbstractShip abstractShip)
        {
            SetText(HitpointsText, abstractShip.Hitpoints.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat));

            SetText(SpeedText, abstractShip.Speed.ToString());

            SetText(CargoText, abstractShip.CargoSize.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat));

            SetText(LasersText, abstractShip.LaserSlots.ToString());

            SetText(GeneratorsText, abstractShip.ShieldAndGearSlots.ToString());

            SetText(ExtrasText, abstractShip.ExtrasSlots.ToString());

            if (abstractShip.Bonus_Statistics)
            {
                Bonus_Text.gameObject.SetEnable();

                var hitpoints = abstractShip.Bonus_Hitpoints.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);
                SetText(Bonus_HitpointsText, $"+{hitpoints}");

                SetText(Bonus_SpeedText, $"+ {abstractShip.Bonus_Speed}");

                var cargo = abstractShip.Bonus_CargoSize.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);
                SetText(Bonus_CargoText, cargo);

                SetText(Bonus_LasersText, $"{abstractShip.Bonus_LasersDamageMultiplyInShip * 100} %");

                SetText(Bonus_GeneratorsText, $"{abstractShip.Bonus_ShieldMultiplyInShip * 100} %");
            }
        }
        else if (ShopItem is AbstractDrone abstractDrone)
        {
            switch (abstractDrone.DroneType)
            {
                case DroneTypes.Flax:
                    FlaxText.gameObject.SetEnable();
                    break;
                case DroneTypes.Iris:
                    IrisText.gameObject.SetEnable();
                    break;
                default:
                    throw new NotImplementedException(abstractDrone.DroneType.ToString());
            }
        }
        else if (ShopItem is AbstractItem abstractItem)
        {
            switch (abstractItem.ItemType)
            {
                case ItemTypes.LF_1:
                    Lf1Text.gameObject.SetEnable();
                    break;
                case ItemTypes.MP_1:
                    Mp1Text.gameObject.SetEnable();
                    break;
                case ItemTypes.LF_2:
                    Lf2Text.gameObject.SetEnable();
                    break;
                case ItemTypes.LF_3:
                    Lf3Text.gameObject.SetEnable();
                    break;

                case ItemTypes.SG3N_A01:
                    SG3N_A01Text.gameObject.SetEnable();
                    break;
                case ItemTypes.SG3N_A02:
                    SG3N_A02Text.gameObject.SetEnable();
                    break;
                case ItemTypes.B01:
                    SG3N_B01Text.gameObject.SetEnable();
                    break;
                case ItemTypes.B02:
                    SG3N_B02Text.gameObject.SetEnable();
                    break;

                case ItemTypes.G3N_1010:
                    G3N_1010Text.gameObject.SetEnable();
                    break;
                case ItemTypes.G3N_6900:
                    G3N_6900Text.gameObject.SetEnable();
                    break;
                case ItemTypes.G3N_7900:
                    G3N_7900Text.gameObject.SetEnable();
                    break;

                case ItemTypes.REP_1:
                    REP_1Text.gameObject.SetEnable();
                    break;
                case ItemTypes.REP_2:
                    REP_2Text.gameObject.SetEnable();
                    break;
                case ItemTypes.REP_3:
                    REP_3Text.gameObject.SetEnable();
                    break;

                default:
                    throw new NotImplementedException(abstractItem.ItemType.ToString());
            }
        }
        else if (ShopItem is AbstractResource abstractResource)
        {
            switch (abstractResource.ResourceType)
            {
                case ResourceTypes.Ammunition1:
                    Ammo1Text.gameObject.SetEnable();
                    break;
                case ResourceTypes.Ammunition2:
                    Ammo2Text.gameObject.SetEnable();
                    break;
                case ResourceTypes.Ammunition3:
                    Ammo3Text.gameObject.SetEnable();
                    break;
                case ResourceTypes.AmmunitionSab:
                    AmmoSabText.gameObject.SetEnable();
                    break;
                case ResourceTypes.Rocket1:
                    Rocket1Text.gameObject.SetEnable();
                    break;
                case ResourceTypes.Rocket2:
                    Rocket2Text.gameObject.SetEnable();
                    break;
                case ResourceTypes.Rocket3:
                    Rocket3Text.gameObject.SetEnable();
                    break;
                case ResourceTypes.Mine:
                    MineText.gameObject.SetEnable();
                    break;
                default:
                    throw new NotImplementedException(abstractResource.ResourceType.ToString());
            }
        }
    }

    private void SetText(TMP_Text obj, string text)
    {
        obj.gameObject.SetEnable(true);
        obj.text = text;
    }
}