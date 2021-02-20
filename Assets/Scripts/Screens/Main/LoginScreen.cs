using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core.Commands;
using NostalgiaOrbitDLL.Core.Exceptions;
using NostalgiaOrbitDLL.Core.Responses;
using NostalgiaOrbitDLL.Core.Validators;
using System.Linq;
using TMPro;
using UnityEngine;

public class LoginScreen : MonoBehaviour
{
    [SerializeField]
    public TMP_InputField UsernameInputField;
    [SerializeField]
    public TMP_InputField PasswordInputField;

    [SerializeField]
    public GameObject UsernameError;
    [SerializeField]
    public GameObject PasswordError;

    [SerializeField]
    public MainScreen MainScreen;

    public void LoginExecute()
    {
        ClearErrors();

        LoginCommand command = new LoginCommand(
            UsernameInputField.text,
            PasswordInputField.text);

        var exceptions = new LoginCommandValidator(command).IsValid();

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
        }

        if (!exceptions.Any())
        {
            command.HashPassword();

            Client.SendToSocket<LoginResponse>(ServerChannels.Main, command, LoginCallback);
        }
    }

    private void LoginCallback(LoginResponse loginResponse)
    {
        ClearErrors();

        if (loginResponse.Exceptions?.Any() ?? false)
        {
            foreach (var exception in loginResponse.Exceptions)
            {
                if (exception is IncorrectUsernameOrPasswordException)
                {
                    UsernameError.SetEnable();
                    PasswordError.SetEnable();
                }
            }
            return;
        }

        Client.SetPilot(loginResponse.Pilot, loginResponse.JWToken);

        if (loginResponse.Pilot.FirmType == FirmTypes.None)
            MainScreen.ChangeScreen(MainScreens.company);
        else
            MainScreen.ChangeScreen(MainScreens.home);
    }

    private void OnEnable()
    {
        ClearErrors();
    }

    private void ClearErrors()
    {
        UsernameError.SetDisable();
        PasswordError.SetDisable();
    }
}