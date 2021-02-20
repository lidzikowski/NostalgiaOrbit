using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core;
using NostalgiaOrbitDLL.Core.Commands;
using NostalgiaOrbitDLL.Environments;
using UnityEngine;

public class OreController : MonoBehaviour
{
    public EnvironmentObject EnvironmentObject;

    [SerializeField]
    public float CollectingTime = 0;

    public bool collected;

    private void Update()
    {
        if (!collected && Client.Pilot != null && OreDistance() == 0)
        {
            collected = true;

            // TODO sprawdzanie ladowni i blokowanie command

            Collect();
        }
    }

    private void Collect()
    {
        Client.SendToSocket(ServerChannels.Game, new CollectEnvironmentCommand(EnvironmentObject.Id));
    }

    private float OreDistance()
    {
        return Vector2.Distance(GetOrePosition(transform), Client.Pilot.Position.ToVector());
    }

    public static Vector2 GetOrePosition(Transform transform)
    {
        var controller = transform.GetComponent<OreController>();
        if (controller != null)
            controller.collected = false;

        var position = transform.position;
        position.y += AbstractEnvironment.OffsetY;
        return position;
    }
    public Vector3 GetSpawnPosition(Vector3 pos)
    {
        var position = pos;
        position.y -= AbstractEnvironment.OffsetY;
        position.z = transform.parent.position.z;
        return position;
    }

    public void SetEnvironmentObject(EnvironmentObject environmentObject)
    {
        EnvironmentObject = environmentObject;

        transform.position = GetSpawnPosition(EnvironmentObject.Position.ToVector());
    }

    public void OnChangeEnvironmentObject(PrefabTypes prefabTypes)
    {
        GetComponent<SpriteAnimation>().ChangePrefabModel(prefabTypes);
    }
}