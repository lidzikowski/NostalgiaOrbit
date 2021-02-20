using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core.Commands;
using NostalgiaOrbitDLL.Core.Responses;
using System.Collections;
using TMPro;
using UnityEngine;

public class HeaderScreen : MonoBehaviour
{
    [SerializeField]
    public TMP_Text UserId;
    [SerializeField]
    public TMP_Text Experience;
    [SerializeField]
    public TMP_Text Level;

    [SerializeField]
    public TMP_Text Jackpot;
    [SerializeField]
    public TMP_Text Credits;
    [SerializeField]
    public TMP_Text Uridium;

    [SerializeField]
    public TMP_Text OnlineUsers;

    private void OnEnable()
    {
        Client.DownloadPilotAndRunFunction(OnDownloadPilot);
    }

    bool executingUpdateOnlinePlayers;
    private void Update()
    {
        if (!executingUpdateOnlinePlayers)
        {
            executingUpdateOnlinePlayers = true;
            StartCoroutine(nameof(UpdateOnlinePlayers));
        }
    }

    private IEnumerator UpdateOnlinePlayers()
    {
        Client.SendToSocket<OnlinePlayersResponse>(ServerChannels.Hangar, new OnlinePlayersCommand(), OnOnlinePlayers);

        yield return new WaitForSeconds(30);

        executingUpdateOnlinePlayers = false;
    }

    private void OnOnlinePlayers(OnlinePlayersResponse onlinePlayersResponse)
    {
        OnlineUsers.text = onlinePlayersResponse.OnlinePilots.ToString();
    }

    private void OnDownloadPilot(Pilot pilot)
    {
        UserId.text = pilot.Id.ToString();

        Experience.text = pilot.Experience.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat);

        Level.text = pilot.Level.ToString();

        Jackpot.text = pilot.GetResource(ResourceTypes.Jackpot).ToString(Helpers.DoubleSeparator, Helpers.NumberFormat);

        Credits.text = pilot.GetResource(ResourceTypes.Credits).ToString(Helpers.DoubleSeparator, Helpers.NumberFormat);

        Uridium.text = pilot.GetResource(ResourceTypes.Uridium).ToString(Helpers.DoubleSeparator, Helpers.NumberFormat);
    }
}