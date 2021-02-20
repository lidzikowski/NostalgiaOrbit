using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core;
using NostalgiaOrbitDLL.Core.Commands;
using NostalgiaOrbitDLL.Core.Exceptions;
using NostalgiaOrbitDLL.Core.Responses;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class Client : MonoBehaviour
{
    private static Client Instance;

    public static readonly Dictionary<Servers, string> ServerIps = new Dictionary<Servers, string>()
    {
        //{ Servers.Main, "192.168.0.122:24231" }, // Localhost
        //{ Servers.Poland, "192.168.0.122:24231" }, // Localhost

        { Servers.Main, "89.65.236.64:24231" }, // Server
        { Servers.Poland, "89.65.236.64:24231" }, // Server
    };
    public static Dictionary<ServerChannels, AbstractSocket> Sockets = new Dictionary<ServerChannels, AbstractSocket>();

    public static void CreateSocket(ServerChannels serverChannel, Action onOpen = null, Action onClose = null)
    {
        if (serverChannel != ServerChannels.Main && Pilot == null)
            return;

        if (Sockets.ContainsKey(serverChannel))
        {
            if (onOpen != null)
                Sockets[serverChannel].OnOpen = () => onOpen.Invoke();
            if (onClose != null)
                Sockets[serverChannel].OnClose = () => onClose.Invoke();

            Sockets[serverChannel].ConnectToServer();
        }
        else
        {
            AbstractSocket abstractSocket = serverChannel switch
            {
                ServerChannels.Main => new MainSocket(),
                ServerChannels.Hangar => new HangarSocket(Pilot.Server),
                ServerChannels.Game => new GameSocket(Pilot.Server),
                ServerChannels.Chat => new ChatSocket(Pilot.Server),
                _ => throw new NotImplementedException(),
            };

            if (onOpen != null)
                abstractSocket.OnOpen = () => onOpen.Invoke();
            if (onClose != null)
                abstractSocket.OnClose = () => onClose.Invoke();

            Sockets.Add(serverChannel, abstractSocket);
            Sockets[serverChannel].ConnectToServer();

            if (abstractSocket.AutoReconnect)
                Sockets[serverChannel].ConfigureRetryConnection(Instance);
        }
    }

    public static void RemoveSocket(ServerChannels serverChannel)
    {
        Debug.Log($"{Sockets.Count} -> Remove {serverChannel}");

        Sockets[serverChannel].Socket.Close();
    }

    public static void Disconnected(ServerChannels serverChannel, string message)
    {
        Debug.Log($"{Sockets.Count} -> Disconnected {serverChannel}");

        Sockets[serverChannel].Socket.CloseAsync(CloseStatusCode.Normal, message);
    }

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
    }

    public static Dictionary<Guid, Action<AbstractResponse>> commandQueue = new Dictionary<Guid, Action<AbstractResponse>>();
    public static void SendToSocket(ServerChannels serverChannel, AbstractCommand abstractCommand)
    {
        if (!Sockets.ContainsKey(serverChannel))
            throw new UnknownChannelException(serverChannel);

        if (!Sockets[serverChannel].Connected)
            return;

        try
        {
            var pilot = Pilot;
            abstractCommand.ConfigureJWToken(JWToken);

            Sockets[serverChannel].Socket.Send(DLLHelpers.Serialize(abstractCommand));
            Debug.Log($"{serverChannel} -> SendToSocket: {abstractCommand.GetType().Name}");
        }
        catch (InvalidOperationException)
        {
            foreach (var item in Sockets)
            {
                RemoveSocket(item.Key);
            }

            SceneManager.LoadScene("MainScene");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
    public static void SendToSocket<T>(ServerChannels serverChannel, AbstractCommand abstractCommand, Action<T> response = null) where T : AbstractResponse
    {
        if (!Sockets.ContainsKey(serverChannel))
            throw new UnknownChannelException(serverChannel);

        if (!Sockets[serverChannel].Connected)
            return;

        try
        {
            var pilot = Pilot;
            abstractCommand.ConfigureJWToken(JWToken);

            var commandId = Guid.NewGuid();
            if (!commandQueue.ContainsKey(commandId))
                commandQueue.Add(commandId, (o) => response.Invoke(o as T));

            abstractCommand.CommandId = commandId;

            Sockets[serverChannel].Socket.Send(DLLHelpers.Serialize(abstractCommand));
            Debug.Log($"{serverChannel} -> SendToSocket: {abstractCommand.GetType().Name} Id: {commandId}");
        }
        catch (InvalidOperationException)
        {
            foreach (var item in Sockets)
            {
                RemoveSocket(item.Key);
            }

            SceneManager.LoadScene("MainScene");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
    public static bool ExecuteCommand(AbstractResponse abstractResponse)
    {
        var responseId = abstractResponse.ResponseId;
        if (commandQueue.ContainsKey(responseId))
        {
            Debug.Log($"ExecuteCommand {responseId}");

            MainThread.Instance().Enqueue(() =>
            {
                commandQueue[responseId].Invoke(abstractResponse);
                commandQueue.Remove(responseId);
            });

            return true;
        }
        else
        {
            Debug.LogWarning($"ExecuteCommand error {abstractResponse.GetType().Name} {responseId}");
            return false;
        }
    }

    public static Pilot Pilot { get; private set; }
    private static DateTime NextDownloadPilotTime;
    public static string JWToken;
    public static void DownloadPilotAndRunFunction(Action<Pilot> function)
    {
        if (DateTime.Now <= NextDownloadPilotTime)
        {
            function.Invoke(Pilot);
            return;
        }

        SendToSocket<PilotDataResponse>(ServerChannels.Hangar, new PilotDataCommand(), (o) =>
        {
            Pilot = o.Pilot;
            NextDownloadPilotTime = DateTime.Now.AddSeconds(1);
            function.Invoke(o.Pilot);
        });
    }
    public static void SetPilot(Pilot pilot, string jwt)
    {
        Pilot = pilot;
        JWToken = jwt;
    }

    public void SendExceptionToServer(string logString, string stackTrace)
    {
        //Debug.LogWarning(logString);
        // TODO send data to socket
    }

    private void OnApplicationQuit()
    {
        foreach (var item in Sockets)
        {
            Disconnected(item.Key, "OnApplicationQuit");
        }
    }
}