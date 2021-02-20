using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core.Commands;
using NostalgiaOrbitDLL.Core.Responses;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainScreen : MonoBehaviour
{
    [SerializeField]
    public TMP_Text OnlinePlayersText;
    [SerializeField]
    public TMP_Text RegisteredPlayersText;

    [SerializeField]
    public TMP_Text ServerStatusText;
    public bool ServerStatus;

    [SerializeField]
    public TMP_Dropdown LanguagesDropDown;
    [SerializeField]
    public VideoPlayer TrailerVideoPlayer;
    [SerializeField]
    public GameObject NewsletterView;
    [SerializeField]
    public VideoClip TrailerVideoClip;
    [SerializeField]
    public VideoClip GameplayVideoClip;
    private Clips currentClip = Clips.trailer;

    [SerializeField]
    public GameObject LoginScreen;
    [SerializeField]
    public GameObject RegisterScreen;
    [SerializeField]
    public GameObject ForgotScreen;
    [SerializeField]
    public GameObject CompanyScreen;



    private void Start()
    {
        Client.SetPilot(default, default);
        Client.commandQueue.Clear();

        switch (Application.systemLanguage)
        {
            case SystemLanguage.Polish:
                LanguagesDropDown.value = 0;
                break;
            default:
                LanguagesDropDown.value = 1;
                break;
        }

        ChangeScreen(MainScreens.login);
        TrailerVideoPlayer.loopPointReached += TrailerVideoPlayer_loopPointReached;
        PlayNextClip(currentClip);

        Client.CreateSocket(ServerChannels.Main, ServerOnline, ServerOffline);
    }

    bool executingUpdateOnlinePlayers;
    private void Update()
    {
        if (!ServerStatus)
            return;

        if (!executingUpdateOnlinePlayers)
        {
            executingUpdateOnlinePlayers = true;
            StartCoroutine(nameof(UpdateOnlinePlayers));
        }
    }

    private IEnumerator UpdateOnlinePlayers()
    {
        yield return new WaitForSeconds(1);

        Client.SendToSocket<OnlinePlayersResponse>(ServerChannels.Main, new OnlinePlayersCommand(), UpdateOnlinePlayers);

        yield return new WaitForSeconds(60);

        executingUpdateOnlinePlayers = false;
    }

    private void ServerOnline()
    {
        ServerStatus = true;
        executingUpdateOnlinePlayers = false;
    }
    private void ServerOffline()
    {
        ServerStatus = false;

        OnlinePlayersText.text = "-";
        RegisteredPlayersText.text = "-";
        ServerStatusText.text = @"<color=""red"">OFFLINE</color>";
    }

    private void UpdateOnlinePlayers(OnlinePlayersResponse command)
    {
        OnlinePlayersText.text = command.OnlinePilots.ToString();
        RegisteredPlayersText.text = command.RegisteredPilots.ToString();

        ServerStatusText.text = @"<color=""green"">ONLINE</color>";
    }

    private void OnDisable()
    {
        Client.Disconnected(ServerChannels.Main, "OnDisable");
    }

    private void TrailerVideoPlayer_loopPointReached(VideoPlayer source)
    {
        currentClip++;
        PlayNextClip(currentClip);
    }

    public void PlayNextClip(int clipIndex)
    {
        var clip = (Clips)clipIndex;
        PlayNextClip(clip);
    }

    public void PlayNextClip(Clips clip)
    {
        if (currentClip != clip)
            currentClip = clip;

        if (clip != Clips.newsletter && !TrailerVideoPlayer.gameObject.activeSelf)
        {
            NewsletterView.SetActive(false);
            TrailerVideoPlayer.gameObject.SetActive(true);
        }

        switch (clip)
        {
            case Clips.trailer:
                TrailerVideoPlayer.clip = TrailerVideoClip;
                break;
            case Clips.gameplay:
                TrailerVideoPlayer.clip = GameplayVideoClip;
                break;
            case Clips.newsletter:
                TrailerVideoPlayer.clip = null;
                TrailerVideoPlayer.gameObject.SetActive(false);
                NewsletterView.SetActive(true);
                break;
        }
    }

    public void ChangeScreen(int screenIndex)
    {
        var screen = (MainScreens)screenIndex;
        ChangeScreen(screen);
    }

    public void ChangeScreen(MainScreens screen)
    {
        LoginScreen.SetDisable();
        RegisterScreen.SetDisable();
        ForgotScreen.SetDisable();
        CompanyScreen.SetDisable();

        switch (screen)
        {
            case MainScreens.login:
                LoginScreen.SetEnable();
                break;
            case MainScreens.register:
                RegisterScreen.SetEnable();
                break;
            case MainScreens.forgot:
                ForgotScreen.SetEnable();
                break;
            case MainScreens.company:
                CompanyScreen.SetEnable();
                break;
            case MainScreens.home:
                GoToHomeScene();
                break;
        }
    }

    private void GoToHomeScene()
    {
        Client.CreateSocket(ServerChannels.Hangar, () => SceneManager.LoadScene("HangarScene"), () => ChangeScreen(MainScreens.login));
    }

    public void ChangeLanguage(int value)
    {
        string code;
        switch (value)
        {
            case 0:
                code = "pl";
                break;
            default:
                code = "en";
                break;
        }

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(code);
    }
}