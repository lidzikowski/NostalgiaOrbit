using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core;
using NostalgiaOrbitDLL.Core.Commands;
using NostalgiaOrbitDLL.Core.Responses;
using NostalgiaOrbitDLL.Items;
using NostalgiaOrbitDLL.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    [Header("User data")]
    [SerializeField]
    public TMP_Text ExperienceText;
    [SerializeField]
    public TMP_Text LevelText;
    [SerializeField]
    public TMP_Text HonorText;
    [SerializeField]
    public TMP_Text JackpotText;

    [Header("User data")]
    [SerializeField]
    public TMP_Text CreditsText;
    [SerializeField]
    public TMP_Text UridiumText;
    [SerializeField]
    public TMP_Text CargoText;

    [Header("Shields slider")]
    [SerializeField]
    public TMP_Text ShieldsText;
    [SerializeField]
    public Slider ShieldsSlider;
    [SerializeField]
    public Image ShieldsFill;

    [Header("Hitpoints slider")]
    [SerializeField]
    public TMP_Text HitpointsText;
    [SerializeField]
    public Slider HitpointsSlider;
    [SerializeField]
    public Image HitpointsFill;

    [Header("Batteries slider")]
    [SerializeField]
    public TMP_Text BatteriesText;
    [SerializeField]
    public Slider BatteriesSlider;
    [SerializeField]
    public Image BatteriesFill;

    [Header("Rockets slider")]
    [SerializeField]
    public TMP_Text RocketsText;
    [SerializeField]
    public Slider RocketsSlider;
    [SerializeField]
    public Image RocketsFill;


    [Header("Configuration buttons")]
    [SerializeField]
    public GameObject Configuration1Button;
    [SerializeField]
    public GameObject Configuration2Button;

    [Header("Minimap")]
    [SerializeField]
    public TMP_Text MinimapText;
    [SerializeField]
    public Slider EnemyCounterSlider;

    [Header("Hide HUB")]
    [SerializeField]
    public Button HubButton;
    [SerializeField]
    public GameObject HeaderGameObject;
    [SerializeField]
    public GameObject FooterBarGameObject;
    [SerializeField]
    public GameObject ConfigurationBarGameObject;
    [SerializeField]
    public GameObject ChatGameObject;
    [SerializeField]
    public GameObject SmallHubGameObject;
    [SerializeField]
    public TMP_Text ExtraShieldsText;
    [SerializeField]
    public TMP_Text ExtraHitpointsText;
    [SerializeField]
    public Slider ExtraShieldsSlider;
    [SerializeField]
    public Slider ExtraHitpointsSlider;
    [SerializeField]
    public TMP_Text ConfigurationHubText;

    [SerializeField]
    public TMP_Text[] AmmunitionsBarText;
    [SerializeField]
    public Button[] AmmunitionsBuyButton;

    [SerializeField]
    public TMP_Text[] RocketsBarText;
    [SerializeField]
    public Button[] RocketsBuyButton;

    [Header("Logout communicat")]
    [SerializeField]
    public GameObject LogoutCommunicatGameObject;
    [SerializeField]
    public TMP_Text LogoutLeftText;


    [Header("Footer bar")]
    [SerializeField]
    public FooterButton[] FooterButtons;
    [SerializeField]
    public Sprite NormalSprite;
    [SerializeField]
    public Sprite UseSprite;



    private void Start()
    {
        LogoutStatus = default;
    }

    public void OnPilotDataChange(Pilot pilot)
    {
        UpdateUserInterface(pilot);

        OnHitpointsChange(pilot.HaveHitpoints, pilot.Equipment_Hitpoints);
        OnShieldsChange(pilot.HaveShields, pilot.Equipment_Shields);

        OnMiniMapPositionUpdate(pilot.Position.ToVector(), pilot.Map);
        OnMiniMapThermometer(UnityEngine.Random.Range(0, 5)); // TODO
        OnConfigurationChange();
        OnAmmunitionChange(pilot.Resources);
        SmallHub();
    }

    private void UpdateUserInterface(Pilot pilot)
    {
        ExperienceText.text = pilot.Experience.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);

        LevelText.text = pilot.Level.ToString();

        HonorText.text = pilot.Honor.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);

        JackpotText.text = pilot.GetResource(ResourceTypes.Jackpot).ToString(Helpers.DoubleSeparator, Helpers.NumberFormat);

        CreditsText.text = pilot.GetResource(ResourceTypes.Credits).ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);

        UridiumText.text = pilot.GetResource(ResourceTypes.Uridium).ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);

        var cargoQuantity = pilot.Resources.Where(o => DLLHelpers.IsCargoType(o.ResourceType)).Sum(o => o.Quantity);
        CargoText.text = cargoQuantity.ToString();

        var batteriesQuantity = Client.Pilot.Resources.Where(o => DLLHelpers.IsAmmunitionType(o.ResourceType)).Sum(o => o.Quantity);
        BatteriesText.text = batteriesQuantity.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);

        var rocketsQuantity = Client.Pilot.Resources.Where(o => DLLHelpers.IsRocketType(o.ResourceType)).Sum(o => o.Quantity);
        RocketsText.text = rocketsQuantity.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);
    }

    public void OnRewardGain(RewardResponse response)
    {
        Client.Pilot.ApplyReward(response.Reward);

        var message = response.MapObject == nameof(EnvironmentObject) ? "" : $"Reward for {response.MapObject}";

        if (response.Reward.Cargos?.Any() ?? false)
        {
            Client.Pilot.ApplyCargo(response.Reward.Cargos);
        }

        UpdateUserInterface(Client.Pilot);
        UpdateAmmunitionBar();

        LogMessage.NewMessage(TextFromReward(message, response.Reward));
    }

    private string TextFromReward(string msg, Reward reward)
    {
        if (reward.Experience != 0)
        {
            msg += $"{Environment.NewLine}Experience {reward.Experience}";
        }

        if (reward.Honor != 0)
        {
            msg += $"{Environment.NewLine}Honor {reward.Honor}";
        }

        if (reward.Resources?.Any() ?? false)
        {
            foreach (var resource in reward.Resources)
            {
                msg += $"{Environment.NewLine}{resource.Key} {resource.Value}";
            }
        }

        if (reward.Items?.Any() ?? false)
        {
            foreach (var item in reward.Items)
            {
                msg += $"{Environment.NewLine}Item {AbstractItem.GetItemByType(item).GetType().Name}";
            }
        }

        if (reward.Cargos?.Any() ?? false)
        {
            foreach (var cargo in reward.Cargos)
            {
                if (msg.Length > 0)
                    msg += $"{Environment.NewLine}{cargo.Resource} {cargo.Quantity}";
                else
                    msg += $"{cargo.Resource} {cargo.Quantity}";
            }
        }

        return msg;
    }

    public void OnHitpointsChange(long hitpoints, long maxHitpoints)
    {
        HitpointsText.text = ExtraHitpointsText.text = $"{hitpoints}";

        var percentage = (float)hitpoints / (float)maxHitpoints;

        HitpointsSlider.value = percentage;
        ExtraHitpointsSlider.value = percentage;
    }

    public void OnShieldsChange(long shields, long maxShields)
    {
        var shield = maxShields > 0 ? $"{shields} / {maxShields}" : "";
        ShieldsText.text = ExtraShieldsText.text = shield;

        var percentage = maxShields > 0 ? (float)shields / (float)maxShields : 0;

        ShieldsSlider.value = percentage;
        ExtraShieldsSlider.value = percentage;
    }

    public void OnConfigurationChange()
    {
        // TODO: czas miedzy zmiana konfiguracji, synchronizacja na serwer

        if (Client.Pilot.ConfigurationFirst)
        {
            ChangeYPosition(Configuration1Button, 5, 1);
            ChangeYPosition(Configuration2Button, 3, 0.6f);
            ConfigurationHubText.text = "1";
        }
        else
        {
            ChangeYPosition(Configuration1Button, 3, 0.6f);
            ChangeYPosition(Configuration2Button, 5, 1);
            ConfigurationHubText.text = "2";
        }

        Client.Pilot.ConfigurationFirst = !Client.Pilot.ConfigurationFirst; // TODO
    }
    private void ChangeYPosition(GameObject gameObject, int y, float alpha)
    {
        var position = gameObject.transform.localPosition;
        var newPosition = new Vector3(position.x, y);
        gameObject.transform.localPosition = newPosition;

        var image = gameObject.GetComponent<RawImage>();
        ChangeFillAlpha(image, alpha);
    }

    private MapTypes currentMap;
    private const int mapSize = 10;
    public void OnMiniMapPositionUpdate(Vector2 position, MapTypes? map = null)
    {
        if (map.HasValue)
            currentMap = map.Value;

        MinimapText.text = $"{currentMap.GetMapName()} / {Mathf.RoundToInt(position.x / mapSize)} - {Mathf.RoundToInt(-position.y / mapSize)}";
    }

    public void OnMiniMapThermometer(int value)
    {
        EnemyCounterSlider.value = value;
    }

    private bool statisticsStatus;
    public void StatisticsChange()
    {
        if (statisticsStatus)
        {
            ShieldsText.gameObject.SetEnable();
            HitpointsText.gameObject.SetEnable();
            BatteriesText.gameObject.SetEnable();
            RocketsText.gameObject.SetEnable();

            ChangeFillAlpha(ShieldsFill, 0.3f);
            ChangeFillAlpha(HitpointsFill, 0.3f);
            ChangeFillAlpha(BatteriesFill, 0.3f);
            ChangeFillAlpha(RocketsFill, 0.3f);
        }
        else
        {
            ShieldsText.gameObject.SetDisable();
            HitpointsText.gameObject.SetDisable();
            BatteriesText.gameObject.SetDisable();
            RocketsText.gameObject.SetDisable();

            ChangeFillAlpha(ShieldsFill, 1);
            ChangeFillAlpha(HitpointsFill, 1);
            ChangeFillAlpha(BatteriesFill, 1);
            ChangeFillAlpha(RocketsFill, 1);
        }

        statisticsStatus = !statisticsStatus;
    }
    private void ChangeFillAlpha(Graphic image, float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private bool smallInterfaceStatus;
    public void SmallHub()
    {
        if (smallInterfaceStatus)
        {
            HubButton.gameObject.SetEnable();
            SmallHubGameObject.gameObject.SetEnable();

            HeaderGameObject.gameObject.SetDisable();
            FooterBarGameObject.gameObject.SetDisable();
            ConfigurationBarGameObject.gameObject.SetDisable();
            ChatGameObject.gameObject.SetDisable();
        }
        else
        {
            HubButton.gameObject.SetDisable();
            SmallHubGameObject.gameObject.SetDisable();

            HeaderGameObject.gameObject.SetEnable();
            FooterBarGameObject.gameObject.SetEnable();
            ConfigurationBarGameObject.gameObject.SetEnable();
            ChatGameObject.gameObject.SetEnable();
        }

        smallInterfaceStatus = !smallInterfaceStatus;
    }

    private void OnAmmunitionChange(List<PilotResource> resources)
    {
        Client.Pilot.Resources = resources;

        var batteriesQuantity = Client.Pilot.Resources.Where(o => DLLHelpers.IsAmmunitionType(o.ResourceType)).Sum(o => o.Quantity);

        BatteriesText.text = batteriesQuantity.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);

        var batteries = (float)batteriesQuantity / 10000;

        BatteriesSlider.value = batteries;


        var rocketsQuantity = Client.Pilot.Resources.Where(o => DLLHelpers.IsRocketType(o.ResourceType)).Sum(o => o.Quantity);

        RocketsText.text = rocketsQuantity.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);

        var rockets = (float)rocketsQuantity / 10000;

        RocketsSlider.value = rockets;

        UpdateAmmunitionBar();
    }

    public void UpdateAmmunitionBar()
    {
        for (int i = 0; i < AmmunitionsBarText.Length; i++)
        {
            switch (i)
            {
                case 0:
                    Apply(AmmunitionsBarText[i], AmmunitionsBuyButton[i], ResourceTypes.Ammunition1);
                    break;
                case 1:
                    Apply(AmmunitionsBarText[i], AmmunitionsBuyButton[i], ResourceTypes.Ammunition2);
                    break;
                case 2:
                    Apply(AmmunitionsBarText[i], AmmunitionsBuyButton[i], ResourceTypes.Ammunition3);
                    break;
                case 3:
                    Apply(AmmunitionsBarText[i], AmmunitionsBuyButton[i], ResourceTypes.Ammunition4);
                    break;
                case 4:
                    Apply(AmmunitionsBarText[i], AmmunitionsBuyButton[i], ResourceTypes.AmmunitionSab);
                    break;
            };
        }

        for (int i = 0; i < RocketsBarText.Length; i++)
        {
            switch (i)
            {
                case 0:
                    Apply(RocketsBarText[i], RocketsBuyButton[i], ResourceTypes.Rocket1);
                    break;
                case 1:
                    Apply(RocketsBarText[i], RocketsBuyButton[i], ResourceTypes.Rocket2);
                    break;
                case 2:
                    Apply(RocketsBarText[i], RocketsBuyButton[i], ResourceTypes.Rocket3);
                    break;
            };
        }

        void Apply(TMP_Text tmp_text, Button buyButton, ResourceTypes resource)
        {
            tmp_text.text = Quantity(resource);
            tmp_text.transform.parent.GetComponent<Image>().sprite = Client.Pilot.Select_Ammunition == resource || Client.Pilot.Select_Rocket == resource ? UseSprite : NormalSprite;

            if (buyButton != null)
            {
                buyButton.gameObject.SetActive(Client.Pilot.Select_Ammunition == resource || Client.Pilot.Select_Rocket == resource);

                var item = AbstractResource.GetResourceByType(resource);
                float price = item.CanBuyUridium ? item.UridiumPurchase[0] : item.CanBuyByCredit ? item.CreditPurchase[0] : 0;
                int quantity = DLLHelpers.IsAmmunitionType(resource) ? 1000 : 100;
                float sum = price * quantity;

                buyButton.transform.GetChild(1).GetComponent<TMP_Text>().text = (sum).ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);
                buyButton.onClick.AddListener(() =>
                {
                    Debug.LogWarning($"Buy {quantity}x {resource} for {sum}");
                });
            }
        }

        string Quantity(ResourceTypes resource)
        {
            return (Client.Pilot.Resources.FirstOrDefault(o => o.ResourceType == resource)?.Quantity ?? 0).ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);
        }
    }

    public static bool LogoutStatus;
    private int logountCounter;
    public void OnLogout(bool status = false)
    {
        if (LogoutStatus == status)
        {
            logountCounter = 0;
            Client.SendToSocket(ServerChannels.Game, new LogoutCommand(LogoutTypes.FromMap, status));
        }
    }

    public void OnLogout(LogoutResponse logoutResponse)
    {
        LogoutStatus = logoutResponse.IsWantLogout;

        if (logoutResponse.LogoutTimer == 0)
        {
            logountCounter++;

            if (logountCounter > 1)
            {
                LogoutCommunicatGameObject.SetActive(false);
                OnLogout(true);
            }
        }
        else
        {
            LogoutCommunicatGameObject.SetActive(LogoutStatus);
            LogoutLeftText.text = $"{logoutResponse.RequireLogoutTime - logoutResponse.LogoutTimer}";
        }
    }
}