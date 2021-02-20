using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core;
using NostalgiaOrbitDLL.Core.Responses;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class GameSocket : AbstractSocket
{
    public GameSocket(Servers server)
    {
        Configure(server, ServerChannels.Game);
    }

    protected override void Socket_OnClose(object sender, CloseEventArgs e)
    {
        base.Socket_OnClose(sender, e);

        MainThread.Instance().Enqueue(() =>
        {
            Client.Disconnected(ServerChannel, "Socket_OnClose");

            if (Client.Sockets.ContainsKey(ServerChannels.Hangar) && Client.Sockets[ServerChannels.Hangar].Connected)
                SceneManager.LoadScene("HangarScene");
            else
                SceneManager.LoadScene("MainScene");
        });
    }

    protected override void Socket_OnMessage(object sender, MessageEventArgs e)
    {
        AbstractResponse response = DLLHelpers.Deserialize<AbstractResponse>(e.RawData);

        if (response is AbstractGameResponse abstractGameResponse)
        {
            MainThread.Instance().Enqueue(() =>
            {
                if (PlayerController == null)
                    PlayerController = GetPlayerController;

                if (abstractGameResponse.ResponseId != default)
                {
                    if (!Client.ExecuteCommand(abstractGameResponse))
                        Debug.LogError($"Unhandle game response {abstractGameResponse.GetType()}");
                    return;
                }

                if (abstractGameResponse is JoinToMapResponse joinToMapResponse)
                {
                    PlayerController.SpawnLocalPlayer(joinToMapResponse.Pilot);
                }
                else if (abstractGameResponse is ChangeMapObjectPositionResponse changeMapObjectPositionResponse)
                {
                    PlayerController.OnChangeMapObjectPosition(changeMapObjectPositionResponse);
                }
                else if (abstractGameResponse is ChangeLifeResponse changeLifeResponse)
                {
                    PlayerController.OnChangeLife(changeLifeResponse);
                }
                else if (abstractGameResponse is SpawnMapObjectResponse spawnMapObjectResponse)
                {
                    PlayerController.OnSpawnMapObject(spawnMapObjectResponse);
                }
                else if (abstractGameResponse is DisposeMapObjectResponse disposeMapObjectResponse)
                {
                    PlayerController.OnDisposeMapObject(disposeMapObjectResponse);
                }
                else if (abstractGameResponse is ResourceUpdateResponse resourceUpdateResponse)
                {
                    PlayerController.OnResourceUpdateResponse(resourceUpdateResponse);
                }
                else if (abstractGameResponse is DestroyMapObjectResponse destroyMapObjectResponse)
                {
                    PlayerController.OnDestroyMapObjectResponse(destroyMapObjectResponse);
                }
                else if (abstractGameResponse is AttackResponse attackResponse)
                {
                    PlayerController.OnAttackResponse(attackResponse);
                }
                else if (abstractGameResponse is RewardResponse rewardResponse)
                {
                    PlayerController.OnRewardResponse(rewardResponse);
                }

                else if (abstractGameResponse is SpawnEnvironmentObjectResponse spawnEnvironmentObjectResponse)
                {
                    PlayerController.OnSpawnEnvironmentObjectResponse(spawnEnvironmentObjectResponse);
                }
                else if (abstractGameResponse is DisposeEnvironmentObjectResponse disposeEnvironmentObjectResponse)
                {
                    PlayerController.OnDisposeEnvironmentObjectResponse(disposeEnvironmentObjectResponse);
                }
                else if (abstractGameResponse is SafeZoneResponse safeZoneResponse)
                {
                    PlayerController.OnSafeZoneResponse(safeZoneResponse);
                }
                else if (abstractGameResponse is ChangeEnvironmentObjectResponse changeEnvironmentObjectResponse)
                {
                    PlayerController.OnChangeEnvironmentObject(changeEnvironmentObjectResponse);
                }
                else if (abstractGameResponse is ChangeMapResponse changeMapResponse)
                {
                    PlayerController.OnMapChange(changeMapResponse.MapType);
                }
                else if (abstractGameResponse is LogoutResponse logoutResponse)
                {
                    PlayerController.OnLogout(logoutResponse);
                }

                //else if (!Client.ExecuteCommand(abstractGameResponse))
                //{
                //    Debug.LogError($"Unhandle game response {abstractGameResponse.GetType()}");
                //}
            });
        }
        else
        {
            base.Socket_OnMessage(sender, e);
        }
    }
}