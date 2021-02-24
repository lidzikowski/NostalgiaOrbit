using NostalgiaOrbitDLL;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HangarScreen : MonoBehaviour
{
    [Header("Current area")]
    [SerializeField]
    public TMP_Text CurrentAreaText;

    [Header("HOME")]
    [SerializeField]
    public GameObject HomeScreen;

    [Header("HANGAR")]
    [SerializeField]
    public GameObject HangarLeftScreen;

    [Header("HANGAR - Bookmarks")]
    [SerializeField]
    public GameObject OverwievScreen;
    [SerializeField]
    public GameObject EquipmentScreen;
    [SerializeField]
    public GameObject ShopScreen;


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

        CurrentAreaText.text = screen.ToString();

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
                HangarLeftScreen.SetEnable();
                EquipmentScreen.GetComponent<HangarEquipmentScreen>().Click();
                EquipmentScreen.SetEnable(true);
                break;

            case HangarScreens.ships:
            case HangarScreens.drones:
            case HangarScreens.weapons:
            case HangarScreens.ammunitions:
            case HangarScreens.generators:
            case HangarScreens.extras:
            case HangarScreens.boosters:
            case HangarScreens.designs:
                HangarLeftScreen.SetEnable();
                ShopScreen.SetEnable(true);
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

        HangarLeftScreen.SetDisable();

        OverwievScreen.SetDisable(true);
        EquipmentScreen.SetDisable(true);
        ShopScreen.SetDisable(true);
    }

    private void StartMap()
    {
        Client.CreateSocket(ServerChannels.Game, () => SceneManager.LoadScene("GameScene"), () => ChangeScreen(HangarScreens.overwiev));
    }
}