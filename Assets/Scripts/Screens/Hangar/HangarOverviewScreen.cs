using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core;
using System.Linq;
using UnityEngine;

public class HangarOverviewScreen : MonoBehaviour
{
    [SerializeField]
    public HangarManager[] HangarManager;

    private void OnEnable()
    {
        foreach (var hangarManager in HangarManager)
        {
            hangarManager.HangarShip.gameObject.SetDisable();
        }

        Client.DownloadPilotAndRunFunction(OnDownloadPilot);
    }

    private void OnDownloadPilot(Pilot pilot)
    {
        HangarManager.First(o => DLLHelpers.IsPrefabType(o.ShipType, pilot.ShipType)).HangarShip.gameObject.SetEnable();
    }
}