using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core;
using NostalgiaOrbitDLL.Core.Responses;
using UnityEngine;
using WebSocketSharp;

public class ChatSocket : AbstractSocket
{
    public ChatSocket(Servers server)
    {
        Configure(server, ServerChannels.Chat);
    }

    protected override void Socket_OnMessage(object sender, MessageEventArgs e)
    {
        AbstractResponse response = DLLHelpers.Deserialize<AbstractResponse>(e.RawData);

        if (response is AbstractResponse abstractResponse)
        {
            MainThread.Instance().Enqueue(() =>
            {
                if (PlayerController == null)
                    PlayerController = GetPlayerController;

                if (abstractResponse is ConnectToChannelResponse connectToChannelResponse)
                {
                    PlayerController.OnConnectToChannelResponse(connectToChannelResponse);
                }
                else if (abstractResponse is ChatMessageResponse chatMessageResponse)
                {
                    PlayerController.OnChatMessageResponse(chatMessageResponse);
                }
                else
                {
                    Debug.LogError($"Unhandle chat response {abstractResponse.GetType()}");
                }
            });
        }
        else
        {
            base.Socket_OnMessage(sender, e);
        }
    }
}