using NostalgiaOrbitDLL;
using TMPro;
using UnityEngine;

public static class Extensions
{
    public static void SetDisable(this GameObject gameObject, bool withParent = false)
    {
        if (withParent)
            gameObject.transform.parent.gameObject.SetActive(false);

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
    public static void SetEnable(this GameObject gameObject, bool withParent = false)
    {
        if (withParent)
            gameObject.transform.parent.gameObject.SetActive(true);

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }
    public static void ClearText(this TMP_InputField inputField)
    {
        inputField.text = string.Empty;
    }
    public static Vector3 ToVector(this PositionVector positionVector)
    {
        return new Vector3(positionVector.Position_X, positionVector.Position_Y);
    }
    public static PositionVector ToPositionVector(this Vector3 vector)
    {
        return new PositionVector(vector.x, vector.y);
    }
}