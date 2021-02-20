using NostalgiaOrbitDLL;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DroneLayerController : MonoBehaviour
{
    public List<SpriteAnimation> DroneAnimations = new List<SpriteAnimation>();

    private Transform RootTransform;
    private Vector2 TargetPosition;
    private Vector2 localTargetPosition;
    private float RotateAngle;

    private void Start()
    {
        TargetPosition = transform.position;
    }

    private void Update()
    {
        RotateTransform();
        Rotate();
    }

    private void RotateTransform()
    {
        float angle = Mathf.Atan2(TargetPosition.y - transform.position.y, TargetPosition.x - transform.position.x);
        if (angle == 0)
            return;
        RotateAngle = angle * Mathf.Rad2Deg + 180;
    }

    private void Rotate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, RotateAngle), Time.deltaTime);

        if (DroneAnimations.Any())
        {
            var direction = (TargetPosition - (Vector2)transform.position).normalized;
            var targetPos = (Vector2)transform.position + direction * 20;

            if ((Vector2)transform.position != targetPos)
            {
                localTargetPosition = targetPos;
            }

            int droneFrame = DroneAnimations[0].CalculateFrameToPosition(transform, localTargetPosition);

            if (droneFrame == -1)
                return;

            foreach (var drone in DroneAnimations)
            {
                drone.RenderFrame(droneFrame);
            }
        }
    }

    public void SetDrones(List<Drone> drones)
    {
        DroneAnimations.Clear();

        foreach (Transform child in transform)
        {
            child.gameObject.SetDisable();
        }

        if (drones == null || !drones.Any())
            return;

        for (int i = 0; i < drones.Count; i++)
        {
            var child = transform.GetChild(i);
            child.gameObject.SetEnable();

            var droneAnimation = child.GetComponent<SpriteAnimation>();

            droneAnimation.ChangePrefabModel(drones[i].DronePrefab);

            DroneAnimations.Add(droneAnimation);
        }
    }

    public void SetTargetPosition(Transform rootTransform, Vector2 targetPosition)
    {
        RootTransform = rootTransform;
        TargetPosition = targetPosition;
    }
}