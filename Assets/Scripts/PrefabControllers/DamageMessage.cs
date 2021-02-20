using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class DamageMessage : MonoBehaviour
{
    private Vector3 TargetPosition;

    void Start()
    {
        TargetPosition = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);
        StartCoroutine(DestroyMessage());
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, TargetPosition, Time.deltaTime * 8);
    }

    IEnumerator DestroyMessage()
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);
    }

    public void SetText(long damage)
    {
        string message;

        if (damage > 0)
        {
            message = $"<color=#FF0000>{damage.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat)}</color>";
        }
        else if (damage < 0)
        {
            message = $"<color=#19FF00>{(-damage).ToString(Helpers.ThousandSeparator, Helpers.NumberFormat)}</color>";
        }
        else
        {
            message = "MISS";
        }

        GetComponent<TMP_Text>().text = message.ToString();
    }
}