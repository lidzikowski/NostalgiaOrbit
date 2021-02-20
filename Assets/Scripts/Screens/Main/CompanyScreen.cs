using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core.Commands;
using NostalgiaOrbitDLL.Core.Exceptions;
using NostalgiaOrbitDLL.Core.Responses;
using NostalgiaOrbitDLL.Core.Validators;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CompanyScreen : MonoBehaviour
{
    [SerializeField]
    public Button MMOLayer;
    [SerializeField]
    public Button EICLayer;
    [SerializeField]
    public Button VRULayer;

    [SerializeField]
    public Button ChooseMMOButton;
    [SerializeField]
    public Button ChooseEICButton;
    [SerializeField]
    public Button ChooseVRUButton;

    [SerializeField]
    public MainScreen MainScreen;

    private bool pressed = false;

    private void OnEnable()
    {
        MMOLayer.gameObject.SetEnable();
        EICLayer.gameObject.SetEnable();
        VRULayer.gameObject.SetEnable();

        ChooseMMOButton.gameObject.SetDisable();
        ChooseEICButton.gameObject.SetDisable();
        ChooseVRUButton.gameObject.SetDisable();
    }

    public void LayerMMO()
    {
        MMOLayer.gameObject.SetDisable();
        EICLayer.gameObject.SetEnable();
        VRULayer.gameObject.SetEnable();

        ChooseMMOButton.gameObject.SetEnable();
        ChooseEICButton.gameObject.SetDisable();
        ChooseVRUButton.gameObject.SetDisable();
    }

    public void LayerEIC()
    {
        MMOLayer.gameObject.SetEnable();
        EICLayer.gameObject.SetDisable();
        VRULayer.gameObject.SetEnable();

        ChooseMMOButton.gameObject.SetDisable();
        ChooseEICButton.gameObject.SetEnable();
        ChooseVRUButton.gameObject.SetDisable();
    }

    public void LayerVRU()
    {
        MMOLayer.gameObject.SetEnable();
        EICLayer.gameObject.SetEnable();
        VRULayer.gameObject.SetDisable();

        ChooseMMOButton.gameObject.SetDisable();
        ChooseEICButton.gameObject.SetDisable();
        ChooseVRUButton.gameObject.SetEnable();
    }

    public void ChooseMMO()
    {
        Choose(FirmTypes.MMO);
    }

    public void ChooseEIC()
    {
        Choose(FirmTypes.EIC);
    }

    public void ChooseVRU()
    {
        Choose(FirmTypes.VRU);
    }

    private void Choose(FirmTypes firmType)
    {
        if (pressed)
            return;
        pressed = true;

        ChooseFirmCommand command = new ChooseFirmCommand(firmType);

        var exceptions = new ChooseFirmCommandValidator(command).IsValid();

        foreach (var exception in exceptions)
        {
            if (exception is BugHandleException)
            {
                pressed = false;
                Debug.Log("Error");
                // BUG WINDOW
            }
        }

        if (!exceptions.Any())
        {
            Client.SendToSocket<ChooseFirmResponse>(ServerChannels.Main, command, ChooseFirmCallback);
        }
    }

    private void ChooseFirmCallback(ChooseFirmResponse chooseFirmResponse)
    {
        if (chooseFirmResponse.Exceptions?.Any() ?? false)
        {
            foreach (var exception in chooseFirmResponse.Exceptions)
            {
                if (exception is BugHandleException)
                {
                    pressed = false;
                    Debug.Log("Error");
                    // BUG WINDOW
                }
            }
            return;
        }

        Debug.Log(chooseFirmResponse.FirmType);

        if (chooseFirmResponse.FirmType != FirmTypes.None)
            MainScreen.ChangeScreen(MainScreens.home);
    }
}