using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Drones;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DroneManager : MonoBehaviour
{
    [Header("Name")]
    [SerializeField]
    public TMP_Text DroneNameText;

    [Header("Drone sprite")]
    [SerializeField]
    public RawImage SpriteRawImage;
    [SerializeField]
    public Texture IrisTexture;
    [SerializeField]
    public Texture FlaxTexture;

    [Header("Slots")]
    [SerializeField]
    public Transform SlotsTransform;
    [SerializeField]
    public GameObject SlotPrefab;

    [Header("Statistics")]
    [SerializeField]
    public TMP_Text DroneLevelText;
    [SerializeField]
    public TMP_Text DroneBonusText;
    [SerializeField]
    public TMP_Text DroneDamageText;

    public void Setup(Drone drone, int index)
    {
        Helpers.DestroyAllChilds(SlotsTransform);

        DroneNameText.text = $"{index} - {drone.DroneType}";

        switch (drone.DroneType)
        {
            case DroneTypes.Flax:
                SpriteRawImage.texture = FlaxTexture;
                break;
            case DroneTypes.Iris:
                SpriteRawImage.texture = IrisTexture;
                break;
            default:
                throw new NotImplementedException(drone.DroneType.ToString());
        }

        var droneType = AbstractDrone.GetDroneByType(drone.DroneType);
        for (int i = 0; i < droneType.LaserOrShieldSlots; i++)
        {
            Instantiate(SlotPrefab, SlotsTransform);
        }

        DroneLevelText.text = $"Level: {drone.Level}"; // TODO - Lang
        DroneBonusText.text = $"Bonus: to/do"; // TODO - Lang
        DroneDamageText.text = $"Damage: {Mathf.RoundToInt((drone.Destructions / droneType.SurviveDestructions) * 100)}"; // TODO - Lang
    }
}