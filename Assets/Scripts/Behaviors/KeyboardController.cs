using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core.Commands;
using NostalgiaOrbitDLL.Core.Exceptions;
using NostalgiaOrbitDLL.Core.Responses;
using NostalgiaOrbitDLL.Maps;
using System;
using System.Linq;
using UnityEngine;

public class KeyboardController : MonoBehaviour
{
    private PlayerController PlayerController;

    private void Start()
    {
        PlayerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (ChatScreen.UseChat)
            return;

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            PlayerController.LocalShipController.AmmunitionAttackSelectedObject = !PlayerController.LocalShipController.AmmunitionAttackSelectedObject;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerController.LocalShipController.UseRocket();
        }

        else if (Input.GetKeyDown(KeyCode.Alpha1))  // TODO - server
        {
            ChangeAmmunition(ResourceTypes.Ammunition1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeAmmunition(ResourceTypes.Ammunition2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeAmmunition(ResourceTypes.Ammunition3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeAmmunition(ResourceTypes.Ammunition4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeAmmunition(ResourceTypes.AmmunitionSab);
        }

        else if (Input.GetKeyDown(KeyCode.J) && !UsePortal)
        {
            var portals = AbstractMap.GetMapByType(Client.Pilot.Map).Portals;

            FindPortals();

            void FindPortals()
            {
                foreach (var portal in portals)
                {
                    if (FindPortal(portal))
                    {
                        return;
                    }
                }

                LogMessage.NewMessage($"Portal not found..."); // TODO lang
            }
        }

        else if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerController.GameScreen.OnConfigurationChange(); // TODO - server
        }

        else if (Input.GetKeyDown(KeyCode.H))
        {
            PlayerController.GameScreen.SmallHub();
        }

        else if (Input.GetKeyDown(KeyCode.L))
        {
            PlayerController.GameScreen.OnLogout();
        }
    }

    public void ChangeAmmunition(int id)
    {
        ChangeAmmunition((ResourceTypes)id);
    }
    public void ChangeAmmunition(ResourceTypes resource)
    {
        Client.Pilot.Select_Ammunition = resource;
        Client.SendToSocket(ServerChannels.Game, new ChooseResourceCommand(resource, Client.Pilot.Select_Rocket));
        PlayerController.GameScreen.UpdateAmmunitionBar();
        PlayerController.GameScreen.FooterButtons[2].Click();
    }

    public void ChangeRocket(int id)
    {
        ChangeRocket((ResourceTypes)id);
    }
    public void ChangeRocket(ResourceTypes resource)
    {
        Client.Pilot.Select_Rocket = resource;
        Client.SendToSocket(ServerChannels.Game, new ChooseResourceCommand(Client.Pilot.Select_Ammunition, resource));
        PlayerController.GameScreen.UpdateAmmunitionBar();
        PlayerController.GameScreen.FooterButtons[3].Click();
    }

    private DateTime NextUsePortal;
    private bool UsePortal;
    private bool FindPortal(Portal portal)
    {
        if (Vector2.Distance(Client.Pilot.Position.ToVector(), portal.Position.ToVector()) <= Portal.JumpDistance)
        {
            var targetMap = AbstractMap.GetMapByType(portal.Target_MapType);
            var requiredLevel = 1;
            //var requiredLevel = targetMap.MapIsFirmType == Client.Pilot.FirmType ? targetMap.RequiredLevel : targetMap.RequiredLevelForEnemy;

            if (Client.Pilot.Level >= requiredLevel)
            {
                if (NextUsePortal > DateTime.Now)
                {
                    LogMessage.NewMessage($"Jump cooldown (client)"); // TODO lang
                    return true;
                }

                NextUsePortal = DateTime.Now.AddSeconds(1);
                LogMessage.NewMessage($"Jump to {portal.Target_MapType} (client-command)"); // TODO lang

                UsePortal = true;

                Client.SendToSocket<ChangeMapResponse>(ServerChannels.Game, new UseJumpPortalCommand(), OnChangeMapResponse);
                return true;
            }
            else
            {
                LogMessage.NewMessage($"Portal required level {requiredLevel} (client)"); // TODO lang
                return true;
            }
        }

        return false;
    }

    private void OnChangeMapResponse(ChangeMapResponse response)
    {
        UsePortal = false;

        if (response.Exceptions?.Any() ?? false)
        {
            foreach (var exception in response.Exceptions)
            {
                switch (exception.GetType().Name)
                {
                    case nameof(PortalNotFoundException):
                        LogMessage.NewMessage($"Portal not found (server)."); // TODO lang
                        break;

                    case nameof(PortalRequiredLevelException):
                        LogMessage.NewMessage($"Portal required level (server)."); // TODO lang
                        break;

                    case nameof(PilotIsAttackedException):
                        LogMessage.NewMessage($"You dont use portal when you attacked (server)."); // TODO lang
                        break;
                }
            }

            return;
        }

        PlayerController.OnMapChange(response.MapType);
    }
}