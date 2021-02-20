using NostalgiaOrbitDLL;

public class MainSocket : AbstractSocket
{
    public MainSocket()
    {
        AutoReconnect = true;

        Configure(Servers.Main, ServerChannels.Main);
    }
}