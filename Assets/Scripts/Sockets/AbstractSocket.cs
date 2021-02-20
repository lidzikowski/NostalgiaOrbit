using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core;
using NostalgiaOrbitDLL.Core.Commands;
using NostalgiaOrbitDLL.Core.Responses;
using System;
using System.Collections;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Net;

public delegate void SocketEventDelegate();

public abstract class AbstractSocket
{
    protected Servers Server;
    protected ServerChannels ServerChannel;
    private string Url;

    public WebSocket Socket;

    public bool Connected;
    public bool AutoReconnect;

    public SocketEventDelegate OnOpen;
    public SocketEventDelegate OnClose;
    public SocketEventDelegate OnError;
    public SocketEventDelegate OnMessage;
    public SocketEventDelegate OnRetry;

    protected PlayerController PlayerController;
    protected PlayerController GetPlayerController => GameObject.Find("PlayerController").GetComponent<PlayerController>();

    protected void Configure(Servers server, ServerChannels serverChannel)
    {
        Server = server;
        ServerChannel = serverChannel;

        var ip = Server switch
        {
            Servers.Main => $"{Client.ServerIps[Servers.Main]}/{ServerChannel}",
            Servers.Poland => $"{Client.ServerIps[Servers.Poland]}/{ServerChannel}",
            _ => null,
        };

        if (string.IsNullOrWhiteSpace(ip))
            throw new NotImplementedException();

        Url = $"ws://{ip}";
        Setup();
    }

    private void Setup()
    {
        Socket = new WebSocket(Url);

        if (!string.IsNullOrWhiteSpace(Client.JWToken))
        {
            //Debug.Log(Client.JWToken);
            Socket.SetCookie(new Cookie(nameof(AbstractCommand.JWToken), Client.JWToken));
        }

        Socket.OnOpen += Socket_OnOpen;
        Socket.OnClose += Socket_OnClose;
        Socket.OnError += Socket_OnError;
        Socket.OnMessage += Socket_OnMessage;
    }

    public void ConnectToServer()
    {
        if (!Socket.IsAlive)
        {
            try
            {
                Socket.ConnectAsync();
            }
            catch (Exception)
            {
                if (AutoReconnect)
                {
                    Setup();
                    ConnectToServer();
                }
            }
        }
    }

    public void ConfigureRetryConnection(MonoBehaviour monoBehaviour)
    {
        OnRetry = () => monoBehaviour.StartCoroutine(RetryConnection());
    }

    private IEnumerator RetryConnection()
    {
        ConnectToServer();
        yield return new WaitForSeconds(5);
    }


    protected virtual void Socket_OnOpen(object sender, EventArgs e)
    {
        MainThread.Instance().Enqueue(() =>
        {
            Connected = true;

            OnOpen?.Invoke();
        });
        Debug.Log($"Socket_OnOpen {ServerChannel}");
    }

    protected virtual void Socket_OnClose(object sender, CloseEventArgs e)
    {
        MainThread.Instance().Enqueue(() =>
        {
            Connected = false;

            OnClose?.Invoke();
            if ((CloseStatusCode)e.Code != CloseStatusCode.ProtocolError)
            {
                OnRetry?.Invoke();
            }
        });
        Debug.Log($"Socket_OnClose {ServerChannel} - {e.Code}");
    }

    protected virtual void Socket_OnError(object sender, ErrorEventArgs e)
    {
        MainThread.Instance().Enqueue(() =>
        {
            Connected = false;

            OnError?.Invoke();
        });
        Debug.Log($"Socket_OnError {ServerChannel} - {e.Exception} : {e.Message}");
    }

    protected virtual void Socket_OnMessage(object sender, MessageEventArgs e)
    {
        AbstractResponse response = DLLHelpers.Deserialize<AbstractResponse>(e.RawData);

        if (response is AbstractResponse abstractResponse)
        {
            Client.ExecuteCommand(abstractResponse);
        }
        else
        {
            Debug.LogError("Unhandle response");
        }
    }
}