using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core;
using NostalgiaOrbitDLL.Items;
using NostalgiaOrbitDLL.Ships;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class HangarShip : MonoBehaviour
{
    [SerializeField]
    public PrefabTypes ShipType;

    [SerializeField]
    public TMP_Text Lasers;
    [SerializeField]
    public TMP_Text Batteries;
    [SerializeField]
    public TMP_Text Rockets;

    [SerializeField]
    public TMP_Text Hitpoints;
    [SerializeField]
    public TMP_Text ShipHitpoints;
    [SerializeField]
    public TMP_Text Speed;
    [SerializeField]
    public TMP_Text SpeedGenerators;
    [SerializeField]
    public TMP_Text ShieldGenerators;
    [SerializeField]
    public TMP_Text ShieldDamage;
    [SerializeField]
    public TMP_Text Drones;

    [SerializeField]
    public GameObject RepairButton;
    [SerializeField]
    public TMP_Text RepairPrice;

    private void OnEnable()
    {
        RepairButton.SetDisable();

        Client.DownloadPilotAndRunFunction(Setup);
    }

    private void Setup(Pilot pilot)
    {
        Func<Item, bool> predicate;
        if (pilot.ConfigurationFirst)
        {
            predicate = o => o.IsEquipConfiguration1 || o.IsEquipInDroneConfiguration1;
        }
        else
        {
            predicate = o => o.IsEquipConfiguration2 || o.IsEquipInDroneConfiguration2;
        }

        var equipItems = pilot.Items.Where(predicate).ToList();

        Lasers.text = equipItems.Count(o => AbstractItem.GetItemByType(o.ItemType).IsLaser).ToString();

        Batteries.text = pilot.Resources.Where(o => DLLHelpers.IsAmmunitionType(o.ResourceType))?.Sum(o => o.Quantity).ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);

        Rockets.text = pilot.Resources.Where(o => DLLHelpers.IsRocketType(o.ResourceType))?.Sum(o => o.Quantity).ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);

        var ship = AbstractShip.GetInstance(pilot.ShipType);

        var hitpoints = pilot.OwnedShips.Single(o => o.ShipType == ship.ShipType).Hitpoints;
        Hitpoints.text = hitpoints.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);

        var ownedBonus = false;
        if (ship.Bonus_Statistics && ship.Bonus_Maps.Contains(pilot.Map))
        {
            if (!ship.Bonus_WorkOnlyFirmMap || (ship.Bonus_WorkOnlyFirmMap && DLLHelpers.IsCompanyMap(pilot.FirmType, pilot.Map, ship.Bonus_Maps)))
            {
                ownedBonus = true;
            }
        }

        if (ownedBonus)
        {
            ShipHitpoints.text = (ship.Hitpoints + ship.Bonus_Hitpoints).ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);

            Speed.text = (ship.Speed + ship.Bonus_Speed).ToString();
        }
        else
        {
            ShipHitpoints.text = ship.Hitpoints.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);

            Speed.text = ship.Speed.ToString();
        }

        SpeedGenerators.text = equipItems.Count(o => AbstractItem.GetItemByType(o.ItemType).IsGear).ToString();

        var equipShields = equipItems.Where(o => AbstractItem.GetItemByType(o.ItemType).IsShield).ToList();

        ShieldGenerators.text = equipShields.Count().ToString();

        var shields = equipShields.Select(o => AbstractItem.GetItemByType(o.ItemType)).ToList();

        var shieldAbsorpion = DLLHelpers.CalculateShieldAbsorptionFromEquipmentShield(shields);
        var hpAbsorpion = 100 - shieldAbsorpion;
        ShieldDamage.text = $"{hpAbsorpion}/{shieldAbsorpion}";

        Drones.text = $"{pilot.Drones.Count(o => o.DroneType == DroneTypes.Flax)}/{pilot.Drones.Count(o => o.DroneType == DroneTypes.Iris)}";

        if (hitpoints <= 0)
        {
            RepairPrice.text = "TODO";

            RepairButton.SetEnable();
        }
    }
}