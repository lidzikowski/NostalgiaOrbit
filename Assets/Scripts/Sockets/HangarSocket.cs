using NostalgiaOrbitDLL;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class HangarSocket : AbstractSocket
{
    public HangarSocket(Servers server)
    {
        Configure(server, ServerChannels.Hangar);
    }

    protected override void Socket_OnClose(object sender, CloseEventArgs e)
    {
        base.Socket_OnClose(sender, e);

        MainThread.Instance().Enqueue(() =>
        {
            Client.Disconnected(ServerChannel, "Socket_OnClose");

            SceneManager.LoadScene("MainScene");
        });
    }
}