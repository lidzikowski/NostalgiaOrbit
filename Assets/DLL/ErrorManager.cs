using UnityEngine;

[RequireComponent(typeof(Client))]
public class ErrorManager : MonoBehaviour
{
    void Awake()
    {
        Application.logMessageReceived += HandleException;
    }

    void HandleException(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            GetComponent<Client>().SendExceptionToServer(logString, stackTrace);
        }
    }
}