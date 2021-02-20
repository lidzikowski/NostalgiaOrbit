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
            Client.SendToSocket(ServerChannels.Game, new ChooseResourceCommand(ResourceTypes.Ammunition1, Client.Pilot.Select_Rocket));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Client.SendToSocket(ServerChannels.Game, new ChooseResourceCommand(ResourceTypes.Ammunition2, Client.Pilot.Select_Rocket));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Client.SendToSocket(ServerChannels.Game, new ChooseResourceCommand(ResourceTypes.Ammunition3, Client.Pilot.Select_Rocket));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Client.SendToSocket(ServerChannels.Game, new ChooseResourceCommand(ResourceTypes.Ammunition4, Client.Pilot.Select_Rocket));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Client.SendToSocket(ServerChannels.Game, new ChooseResourceCommand(ResourceTypes.AmmunitionSab, Client.Pilot.Select_Rocket));
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