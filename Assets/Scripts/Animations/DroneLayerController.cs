using NostalgiaOrbitDLL;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DroneLayerController : MonoBehaviour
{
    public List<SpriteAnimation> DroneAnimations = new List<SpriteAnimation>();

    private Vector2 RootPosition;
    private Vector2 TargetPosition;
    private float RotateAngle;
    private Quaternion beforeRotation;

    private void Start()
    {
        TargetPosition = transform.position;
    }

    private void Update()
    {
        RotateTransform();
        Rotate();
    }

    private void Rotate()
    {
        beforeRotation = transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, RotateAngle), Time.deltaTime);

        if (beforeRotation != transform.rotation && DroneAnimations.Any() && Vector2.Distance(RootPosition, TargetPosition) > 0)
        {
            int droneFrame = DroneAnimations[0].CalculateFrameToPosition(transform, TargetPosition);

            if (droneFrame == -1)
                return;

            foreach (var drone in DroneAnimations)
            {
                drone.RenderFrame(droneFrame);
            }
        }
    }

    private void RotateTransform()
    {
        float angle = Mathf.Atan2(TargetPosition.y - transform.position.y, TargetPosition.x - transform.position.x);
        if (angle == 0)
            return;
        RotateAngle = angle * Mathf.Rad2Deg + 180;
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

        //drones = new List<Drone>()
        //{
        //    new Drone(DroneTypes.Iris),
        //    new Drone(DroneTypes.Iris),
        //    new Drone(DroneTypes.Iris),
        //    new Drone(DroneTypes.Iris),
        //    new Drone(DroneTypes.Iris),
        //    new Drone(DroneTypes.Iris),
        //    new Drone(DroneTypes.Iris),
        //    new Drone(DroneTypes.Iris),
        //};

        for (int i = 0; i < drones.Count; i++)
        {
            var child = transform.GetChild(i);
            child.gameObject.SetEnable();

            var droneAnimation = child.GetComponent<SpriteAnimation>();

            droneAnimation.ChangePrefabModel(drones[i].DronePrefab);

            DroneAnimations.Add(droneAnimation);
        }
    }

    public void SetTargetPosition(Vector2 rootPosition, Vector2 targetPosition)
    {
        RootPosition = rootPosition;
        TargetPosition = targetPosition;
    }
}