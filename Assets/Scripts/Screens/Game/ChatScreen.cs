using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core.Commands;
using NostalgiaOrbitDLL.Core.Responses;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatScreen : MonoBehaviour
{
    [Header("Channels")]
    [SerializeField]
    public Transform ChannelsTransform;
    [SerializeField]
    public GameObject ChannelPrefab;

    [Header("Messages")]
    [SerializeField]
    public Transform MessagesTransform;
    [SerializeField]
    public GameObject MessagePrefab;


    [Header("Input field")]
    public TMP_InputField SendMessageInputField;
    public static bool UseChat;

    [Header("Chat window")]
    [SerializeField]
    public GameObject ChatWindowGameobject;
    [SerializeField]
    public GameObject ChatShowButtonGameObject;


    public Dictionary<Guid, ChatChannelTypes> Channels = new Dictionary<Guid, ChatChannelTypes>();
    public Dictionary<Guid, List<ChatUser>> Users = new Dictionary<Guid, List<ChatUser>>();
    public Dictionary<Guid, List<ChatMessage>> ChannelMessages = new Dictionary<Guid, List<ChatMessage>>();
    private Guid? ActiveChannel;
    private Func<string, ChatMessage> ServerMessage = o => new ChatMessage(default, default, new ChatUser(default, "<color=yellow>Server</color>"), $"{o}");
    private bool ChatVisibility;



    private void Start()
    {
        Helpers.DestroyAllChilds(ChannelsTransform);
        Helpers.DestroyAllChilds(MessagesTransform);

        ChangeChatVisibility();

        SendMessageInputField.onSubmit.AddListener(SendMessageToChannel);

        Client.CreateSocket(ServerChannels.Chat);
    }



    public void ActiveChat()
    {
        UseChat = true;
    }
    public void UnactiveChat()
    {
        UseChat = false;
    }
    public void SendMessageToChannel(string message)
    {
        if (!ActiveChannel.HasValue || string.IsNullOrWhiteSpace(message))
            return;

        Client.SendToSocket(ServerChannels.Chat, new ChannelMessageCommand(ActiveChannel.Value, message));

        SendMessageInputField.ClearText();
    }



    public void OnConnectToChannelResponse(ConnectToChannelResponse response)
    {
        if (!Channels.ContainsKey(response.ChannelId))
        {
            Channels.Add(response.ChannelId, response.ChannelType);

            GameObject channelButton = Instantiate(ChannelPrefab, ChannelsTransform);
            channelButton.transform.GetChild(0).GetComponent<TMP_Text>().text = response.ChannelType.ToString();
            channelButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                ActiveChannel = response.ChannelId;
                RefreshChannel();
            });
        }

        if (!Users.ContainsKey(response.ChannelId))
        {
            Users.Add(response.ChannelId, new List<ChatUser>());
        }

        Users[response.ChannelId].Add(response.User);

        if (!ChannelMessages.ContainsKey(response.ChannelId))
        {
            ChannelMessages.Add(response.ChannelId, new List<ChatMessage>());
        }

        if (!ActiveChannel.HasValue)
        {
            ActiveChannel = response.ChannelId;
            CreateMessage(ServerMessage($"Channel {response.ChannelType}"));
        }
    }

    private void RefreshChannel()
    {
        Helpers.DestroyAllChilds(MessagesTransform);

        CreateMessage(ServerMessage($"Channel {Channels[ActiveChannel.Value]}"));

        foreach (var message in ChannelMessages[ActiveChannel.Value])
        {
            CreateMessage(message);
        }
    }

    public void OnChatMessageResponse(ChatMessageResponse response)
    {
        foreach (var message in response.Messages)
        {
            ChannelMessages[message.ChannelId].Add(message);

            if (message.ChannelId == ActiveChannel)
                CreateMessage(message);
        }
    }

    private void CreateMessage(ChatMessage chatMessage)
    {
        GameObject message = Instantiate(MessagePrefab, MessagesTransform);
        message.GetComponent<TMP_Text>().text = $"{chatMessage.ChatUser.Name} :  <color=white>{chatMessage.Message}</color>";
    }

    public void ChangeChatVisibility()
    {
        ChatVisibility = !ChatVisibility;

        if (ChatVisibility)
        {
            ChatWindowGameobject.SetEnable();
            ChatShowButtonGameObject.SetDisable();
        }
        else
        {
            ChatWindowGameobject.SetDisable();
            ChatShowButtonGameObject.SetEnable();
        }
    }
}