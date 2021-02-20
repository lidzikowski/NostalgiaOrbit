using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core.Commands;
using NostalgiaOrbitDLL.Core.Exceptions;
using NostalgiaOrbitDLL.Core.Responses;
using NostalgiaOrbitDLL.Core.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterScreen : MonoBehaviour
{
    [SerializeField]
    public TMP_InputField UsernameInputField;
    [SerializeField]
    public TMP_InputField PasswordInputField;
    [SerializeField]
    public TMP_InputField ConfirmPasswordInputField;
    [SerializeField]
    public TMP_InputField EmailInputField;
    [SerializeField]
    public TMP_Dropdown ServerDropDown;
    [SerializeField]
    public Toggle RulesToogle;
    [SerializeField]
    public Toggle NewsletterToogle;
    [SerializeField]
    public TMP_InputField PilotNameInputField;

    [SerializeField]
    public GameObject UsernameError;
    [SerializeField]
    public GameObject PasswordError;
    [SerializeField]
    public GameObject ConfirmPasswordError;
    [SerializeField]
    public GameObject EmailError;
    [SerializeField]
    public GameObject RulesError;
    [SerializeField]
    public GameObject PilotNameError;

    [SerializeField]
    public GameObject UsernameOccupiedError;
    [SerializeField]
    public GameObject EmailOccupiedError;
    [SerializeField]
    public GameObject PilotNameOccupiedError;

    [SerializeField]
    public MainScreen MainScreen;

    public void RegisterExecute()
    {
        ClearErrors();

        RegisterCommand command = new RegisterCommand(
            UsernameInputField.text,
            PasswordInputField.text,
            EmailInputField.text,
            (Servers)(ServerDropDown.value + 1),
            RulesToogle.isOn,
            NewsletterToogle.isOn,
            PilotNameInputField.text);

        var exceptions = new RegisterCommandValidator(command).IsValid();

        foreach (var exception in exceptions)
        {
            if (exception is IncorrectUsernameException)
            {
                UsernameError.SetEnable();
            }
            else if (exception is IncorrectPasswordException)
            {
                PasswordError.SetEnable();
            }
            else if (exception is IncorrectEmailException)
            {
                EmailError.SetEnable();
            }
            else if (exception is IncorrectRulesException)
            {
                RulesError.SetEnable();
            }
            else if (exception is IncorrectPilotNameException)
            {
                PilotNameError.SetEnable();
            }
        }

        if (PasswordInputField.text != ConfirmPasswordInputField.text)
        {
            PasswordError.SetEnable();
            ConfirmPasswordError.SetEnable();
            return;
        }

        if (!exceptions.Any())
        {
            command.HashPassword();

            Client.SendToSocket<RegisterResponse>(ServerChannels.Main, command, RegisterCallback);
        }
    }

    private void RegisterCallback(RegisterResponse registerResponse)
    {
        ClearErrors();

        if (registerResponse.Exceptions?.Any() ?? false)
        {
            foreach (var exception in registerResponse.Exceptions)
            {
                if (exception is OccupiedUsernameException)
                {
                    UsernameOccupiedError.SetEnable();
                }
                else if (exception is OccupiedEmailException)
                {
                    EmailOccupiedError.SetEnable();
                }
                else if (exception is OccupiedPilotNameException)
                {
                    PilotNameOccupiedError.SetEnable();
                }
            }
            return;
        }

        Client.SetPilot(registerResponse.Pilot, registerResponse.JWToken);

        if (registerResponse.Pilot.FirmType == FirmTypes.None)
            MainScreen.ChangeScreen(MainScreens.company);
        else
            MainScreen.ChangeScreen(MainScreens.home);
    }

    private void OnDisable()
    {
        UsernameInputField.ClearText();
        PasswordInputField.ClearText();
        ConfirmPasswordInputField.ClearText();
        EmailInputField.ClearText();
        RulesToogle.isOn = false;
        NewsletterToogle.isOn = true;
    }

    private void OnEnable()
    {
        ClearErrors();

        var options = new List<TMP_Dropdown.OptionData>();

        foreach (Servers server in (Servers[])Enum.GetValues(typeof(Servers)))
        {
            if (server == Servers.Main)
                continue;

            options.Add(new TMP_Dropdown.OptionData(server.ToString()));
        }

        ServerDropDown.options = options;
    }

    private void ClearErrors()
    {
        UsernameError.SetDisable();
        PasswordError.SetDisable();
        ConfirmPasswordError.SetDisable();
        EmailError.SetDisable();
        RulesError.SetDisable();
        PilotNameError.SetDisable();
        UsernameOccupiedError.SetDisable();
        EmailOccupiedError.SetDisable();
        PilotNameOccupiedError.SetDisable();
    }
}