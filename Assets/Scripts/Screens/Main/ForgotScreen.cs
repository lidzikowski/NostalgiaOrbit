using TMPro;
using UnityEngine;

public class ForgotScreen : MonoBehaviour
{
    [SerializeField]
    public TMP_InputField UsernameOrEmailInputField;

    [SerializeField]
    public GameObject UsernameOrEmailError;

    public void ForgotExecute()
    {
        UsernameOrEmailError.SetEnable();

        Debug.Log("ForgotExecute");
    }

    private void OnDisable()
    {
        UsernameOrEmailInputField.ClearText();
    }

    private void OnEnable()
    {
        UsernameOrEmailError.SetDisable();
    }
}