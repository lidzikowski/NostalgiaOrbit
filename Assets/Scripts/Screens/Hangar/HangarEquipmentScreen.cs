using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HangarEquipmentScreen : MonoBehaviour
{
    [Header("Ship")]
    [SerializeField]
    public Transform ShipTransform;
    [SerializeField]
    public Transform ShipContentTransform;


    [Header("Drones")]
    [SerializeField]
    public Transform DronesTransform;
    [SerializeField]
    public Transform DronesContentTransform;


    [Header("User")]
    [SerializeField]
    public Transform UserContentTransform;


    [Header("Prefabs")]
    [SerializeField]
    public GameObject ItemsCategoryPrefab;
    [SerializeField]
    public GameObject DroneCategoryPrefab;
    [SerializeField]
    public GameObject SeparatorPrefab;
    [SerializeField]
    public GameObject SlotPrefab;
    [SerializeField]
    public GameObject ItemPrefab;


    [Header("Buttons")]
    [SerializeField]
    public Button Config1Button;
    [SerializeField]
    public Button Config2Button;
    [SerializeField]
    public Button ChooseShipButton;
    [SerializeField]
    public Button ShipButton;
    [SerializeField]
    public Button DronesButton;

    [Header("Button sprites")]
    [SerializeField]
    public Sprite ActiveSprite;
    [SerializeField]
    public Sprite InactiveSprite;
    [SerializeField]
    public Sprite SmallActiveSprite;
    [SerializeField]
    public Sprite SmallInactiveSprite;

    private bool? FirstConfiguration = true;
    private Dictionary<CategoryTypes, Transform> Categories = new Dictionary<CategoryTypes, Transform>();
    private Dictionary<Guid, Transform> Drones = new Dictionary<Guid, Transform>();


    private void OnEnable()
    {
        Config1_Click();
        Ship_Click();
    }

    public void Click(bool download = true)
    {
        Helpers.DestroyAllChilds(ShipContentTransform);
        Helpers.DestroyAllChilds(DronesContentTransform);
        Helpers.DestroyAllChilds(UserContentTransform);

        CreateCategories();

        if (download)
        {
            Client.DownloadPilotAndRunFunction(OnDownloadPilot);
        }
        else
        {
            OnDownloadPilot(Client.Pilot);
        }
    }

    private void OnDownloadPilot(Pilot pilot)
    {
        if (pilot.Drones.Any())
        {
            for (int i = 1; i <= pilot.Drones.Count; i++)
            {
                CreateDrone(pilot.Drones[i - 1], i);
            }
        }

        var ship = pilot.Ship;

        for (int i = 0; i < ship.LaserSlots; i++)
        {
            CreateEmptySlot(CategoryTypes.ShipLasers);
        }
        for (int i = 0; i < ship.ShieldAndGearSlots; i++)
        {
            CreateEmptySlot(CategoryTypes.ShipGear);
        }
        for (int i = 0; i < ship.ExtrasSlots; i++)
        {
            CreateEmptySlot(CategoryTypes.ShipExtras);
        }

        foreach (var item in pilot.Items)
        {
            var abstractItem = AbstractItem.GetItemByType(item.ItemType);

            if (abstractItem.IsLaser)
            {
                CreateEmptySlot(CategoryTypes.UserLasers);
            }
            else if (abstractItem.IsGear)
            {
                CreateEmptySlot(CategoryTypes.UserGear);
            }
            else if (abstractItem.IsExtras)
            {
                CreateEmptySlot(CategoryTypes.UserExtras);
            }

            SpawnItem(item, abstractItem);
        }

        foreach (var item in pilot.Resources)
        {
            if (item.ResourceType < ResourceTypes.None)
            {
                if (item.ResourceType == ResourceTypes.Uridium || item.ResourceType == ResourceTypes.Credits || item.ResourceType == ResourceTypes.Jackpot)
                    continue;

                CreateEmptySlot(CategoryTypes.UserResources);

                CreateItem(item);
            }
        }

        RefreshAllUI();
    }

    private void RefreshAllUI()
    {
        Helpers.RefreshUI(ShipContentTransform);
        Helpers.RefreshUI(UserContentTransform);
        Helpers.RefreshUI(DronesContentTransform);
    }

    private void SpawnItem(Item item, AbstractItem abstractItem)
    {
        if (item.IsEquipConfiguration1 && FirstConfiguration == true || item.IsEquipConfiguration2 && FirstConfiguration == false)
        {
            CreateItem(item, MathItem(abstractItem, true, false));
        }
        else if (item.IsEquipInDroneConfiguration1 && FirstConfiguration == true || item.IsEquipInDroneConfiguration2 && FirstConfiguration == false)
        {
            CreateItem(item, MathDroneItem(item.DroneIdConfiguration1.Value));
        }
        else
        {
            CreateItem(item, MathItem(abstractItem, false, true));
        }
    }

    private Transform MathItem(AbstractItem abstractItem, bool ship, bool user)
    {
        if (ship || user)
        {
            if (abstractItem.IsLaser)
            {
                return ship ? Categories[CategoryTypes.ShipLasers] : Categories[CategoryTypes.UserLasers];
            }
            else if (abstractItem.IsGear || abstractItem.IsShield)
            {
                return ship ? Categories[CategoryTypes.ShipGear] : Categories[CategoryTypes.UserGear];
            }
            else if (abstractItem.IsExtras)
            {
                return ship ? Categories[CategoryTypes.ShipExtras] : Categories[CategoryTypes.UserExtras];
            }
        }

        return null;
    }

    private Transform MathDroneItem(Guid droneId)
    {
        return Drones[droneId].GetChild(1);
    }

    private void CreateItem(Item item, Transform root)
    {
        GameObject go = Instantiate(ItemPrefab, FindEmptySlot(root.GetChild(1)));
        go.GetComponent<ItemSlot>().Setup(item);
    }

    private void CreateItem(PilotResource item)
    {
        GameObject go = Instantiate(ItemPrefab, FindEmptySlot(Categories[CategoryTypes.UserResources].GetChild(1)));
        go.GetComponent<ItemSlot>().Setup(item);
    }

    private Transform FindEmptySlot(Transform root)
    {
        foreach (Transform child in root)
        {
            if (child.childCount == 0)
            {
                return child;
            }
        }

        throw new NotImplementedException("Not found empty slot.");
    }

    private void CreateCategories()
    {
        foreach (var item in Categories)
        {
            Destroy(item.Value.gameObject);
        }
        Categories.Clear();

        foreach (var item in Drones)
        {
            Destroy(item.Value.gameObject);
        }
        Drones.Clear();

        // Ship
        CreateCategory(CategoryTypes.ShipLasers, ShipContentTransform);
        CreateSeparator(ShipContentTransform);
        CreateCategory(CategoryTypes.ShipGear, ShipContentTransform);
        CreateSeparator(ShipContentTransform);
        CreateCategory(CategoryTypes.ShipExtras, ShipContentTransform);

        // User
        CreateCategory(CategoryTypes.UserLasers, UserContentTransform);
        CreateSeparator(UserContentTransform);
        CreateCategory(CategoryTypes.UserGear, UserContentTransform);
        CreateSeparator(UserContentTransform);
        CreateCategory(CategoryTypes.UserExtras, UserContentTransform);
        CreateSeparator(UserContentTransform);
        CreateCategory(CategoryTypes.UserResources, UserContentTransform);
    }

    private void CreateCategory(CategoryTypes categoryType, Transform parent)
    {
        GameObject go = Instantiate(ItemsCategoryPrefab, parent);

        go.transform.GetChild(0).GetComponent<TMP_Text>().text = categoryType.ToString();
        Helpers.DestroyAllChilds(go.transform.GetChild(1).transform);

        Categories.Add(categoryType, go.transform);
    }

    private Transform CreateSeparator(Transform parent)
    {
        GameObject go = Instantiate(SeparatorPrefab, parent);

        return go.transform;
    }

    private Transform CreateEmptySlot(CategoryTypes categoryType)
    {
        var transform = Categories[categoryType];

        GameObject go = Instantiate(SlotPrefab, transform.GetChild(1));

        return go.transform;
    }

    private void CreateDrone(Drone drone, int index)
    {
        GameObject go = Instantiate(DroneCategoryPrefab, DronesContentTransform);
        go.GetComponent<DroneManager>().Setup(drone, index);

        Drones.Add(drone.Id, go.transform);
    }



    public void Config1_Click()
    {
        InactiveAll(true, false);
        Active(Config1Button, true);

        if (FirstConfiguration == true)
            return;

        if (FirstConfiguration == null)
        {
            FirstConfiguration = true;

            Inactive(ChooseShipButton, true);
            Ship_Click();
        }

        FirstConfiguration = true;
        Click(false);
    }
    public void Config2_Click()
    {
        InactiveAll(true, false);
        Active(Config2Button, true);

        if (FirstConfiguration == false)
            return;

        if (FirstConfiguration == null)
        {
            FirstConfiguration = false;

            Inactive(ChooseShipButton, true);
            Ship_Click();
        }

        FirstConfiguration = false;
        Click(false);
    }
    public void ChooseShip_Click()
    {
        InactiveAll(true, true, true);
        Active(ChooseShipButton, true);
        //ShipTransform.gameObject.SetEnable(); TODO

        FirstConfiguration = null;
        RefreshAllUI();
    }
    public void Ship_Click()
    {
        InactiveAll(false);
        Active(ShipButton, false);
        ShipTransform.gameObject.SetEnable();

        if (FirstConfiguration == null)
        {
            FirstConfiguration = true;

            Active(Config1Button, true);
            Click(false);
        }

        RefreshAllUI();
    }
    public void Drones_Click()
    {
        InactiveAll(false);
        Active(DronesButton, false);
        DronesTransform.gameObject.SetEnable();

        if (FirstConfiguration == null)
        {
            FirstConfiguration = true;

            Active(Config1Button, true);
            Click(false);
        }

        RefreshAllUI();
    }

    private void Active(Button button, bool small)
    {
        button.GetComponent<Image>().sprite = small ? SmallActiveSprite : ActiveSprite;
    }
    private void Inactive(Button button, bool small)
    {
        button.GetComponent<Image>().sprite = small ? SmallInactiveSprite : InactiveSprite;
    }
    private void InactiveAll(bool header, bool withLayer = true, bool all = false)
    {
        if (all)
        {
            Inactive(Config1Button, true);
            Inactive(Config2Button, true);
            Inactive(ChooseShipButton, true);
            Inactive(ShipButton, false);
            Inactive(DronesButton, false);
        }
        else
        {
            if (header)
            {
                Inactive(Config1Button, true);
                Inactive(Config2Button, true);
                Inactive(ChooseShipButton, true);
            }
            else
            {
                Inactive(ChooseShipButton, true);
                Inactive(ShipButton, false);
                Inactive(DronesButton, false);
            }
        }

        if (withLayer)
        {
            ShipTransform.gameObject.SetDisable();
            DronesTransform.gameObject.SetDisable();
        }
    }


    private enum CategoryTypes
    {
        ShipLasers,
        ShipGear,
        ShipExtras,

        UserLasers,
        UserGear,
        UserExtras,
        UserResources,
    }
}