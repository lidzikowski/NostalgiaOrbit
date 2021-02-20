using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : MonoBehaviour
{
    [SerializeField]
    public TMP_Text Nickname;
    [SerializeField]
    public TMP_Text Server;
    [SerializeField]
    public RawImage RankSprite;
    [SerializeField]
    public TMP_Text Rank;
    [SerializeField]
    public TMP_Text Premium;
    [SerializeField]
    public TMP_Text Level;
    [SerializeField]
    public TMP_Text Company;
    [SerializeField]
    public TMP_Text Map;
    [SerializeField]
    public TMP_Text Registered;

    private void OnEnable()
    {
        Client.DownloadPilotAndRunFunction(OnDownloadPilot);
    }

    private void OnDownloadPilot(Pilot pilot)
    {
        Nickname.text = pilot.PilotName;

        Server.text = pilot.Server.ToString();

        //RankSprite
        Rank.text = pilot.RankType.ToString(); // Language
        StartCoroutine(UpdateRankTextSize());

        Premium.text = pilot.PremiumStatus.ToString(); // Language

        Level.text = pilot.Level.ToString();

        Company.text = pilot.FirmType.ToString();

        Map.text = pilot.Map.GetMapName();

        Registered.text = pilot.RegisterDate.ToString();
    }

    private IEnumerator UpdateRankTextSize()
    {
        var go = Rank.gameObject.transform.parent.GetComponent<HorizontalLayoutGroup>();
        go.enabled = false;
        yield return new WaitForEndOfFrame();
        go.enabled = true;
    }
}