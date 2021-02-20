using System;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LogMessage : MonoBehaviour
{
    private TMP_Text MessageText;

    void Start()
    {
        StartCoroutine(DestroyMessage());
    }

    float timer = 0;
    void Update()
    {
        timer += Time.deltaTime;

        if (timer < 0.3f || MessageText == null)
            return;

        timer = 0;

        var color = MessageText.color;
        color.a = color.a - 0.01f;
        MessageText.color = color;
    }

    IEnumerator DestroyMessage()
    {
        yield return new WaitForSeconds(5); // TODO - settings
        Destroy(gameObject);
    }

    public void ShowInformation(string information)
    {
        MessageText = GetComponent<TMP_Text>();
        MessageText.text = information;
    }

    public static void NewMessage(string information)
    {
        if (string.IsNullOrWhiteSpace(information))
            return;

        var message = Instantiate(Resources.Load<GameObject>($"LogMessage"), GameObject.Find("LogMessageContent").transform);
        message.GetComponent<LogMessage>().ShowInformation(information);
        message.transform.SetSiblingIndex(0);
    }
}