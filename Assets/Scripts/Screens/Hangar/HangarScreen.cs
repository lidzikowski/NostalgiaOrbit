using NostalgiaOrbitDLL;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HangarScreen : MonoBehaviour
{
    [SerializeField]
    public GameObject HomeScreen;

    [SerializeField]
    public GameObject OverwievScreen;
    [SerializeField]
    public GameObject HangarLeftScreen;


    private void Start()
    {
        ChangeScreen(HangarScreens.home);
    }

    public void ChangeScreen(int screenIndex)
    {
        var screen = (HangarScreens)screenIndex;
        ChangeScreen(screen);
    }

    public void ChangeScreen(HangarScreens screen)
    {
        DisableAll();

        switch (screen)
        {
            case HangarScreens.home:
                HomeScreen.SetEnable();
                break;

            case HangarScreens.overwiev:
                HangarLeftScreen.SetEnable();
                OverwievScreen.SetEnable(true);
                break;
            case HangarScreens.equipment:
                break;
            case HangarScreens.ships:
                break;
            case HangarScreens.drones:
                break;
            case HangarScreens.weapons:
                break;
            case HangarScreens.ammunitions:
                break;
            case HangarScreens.generators:
                break;
            case HangarScreens.extras:
                break;
            case HangarScreens.boosters:
                break;
            case HangarScreens.designs:
                break;
            case HangarScreens.trade:
                break;
            case HangarScreens.laboratory:
                break;
            case HangarScreens.clan:
                break;
            case HangarScreens.uridium:
                break;
            case HangarScreens.quests:
                break;
            case HangarScreens.help:
                break;
            case HangarScreens.logout:
                break;
            case HangarScreens.map:
                StartMap();
                break;
        }
    }

    private void DisableAll()
    {
        HomeScreen.SetDisable();
        OverwievScreen.SetDisable(true);
        HangarLeftScreen.SetDisable();
    }

    private void StartMap()
    {
        Client.CreateSocket(ServerChannels.Game, () => SceneManager.LoadScene("GameScene"), () => ChangeScreen(HangarScreens.overwiev));
    }
}