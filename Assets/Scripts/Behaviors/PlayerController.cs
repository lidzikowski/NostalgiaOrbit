using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core.Responses;
using NostalgiaOrbitDLL.Enemies;
using NostalgiaOrbitDLL.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MouseController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public GameScreen GameScreen;
    [SerializeField]
    public ChatScreen ChatScreen;

    [SerializeField]
    public Transform LocalShipLayer;
    [HideInInspector]
    public ShipController LocalShipController;

    [SerializeField]
    public Transform OtherObjectsLayer;
    private Dictionary<Guid, ShipController> AllObjectsController = new Dictionary<Guid, ShipController>();
    private Dictionary<Guid, OreController> AllEnvironments = new Dictionary<Guid, OreController>();

    [SerializeField]
    public Transform MapTransform;
    [SerializeField]
    public Transform BaseTransform;
    [SerializeField]
    public Transform PortalsTransform;
    [SerializeField]
    public Transform EnvironmentsTransform;
    [SerializeField]
    public Transform BulletTransform;



    private void Update()
    {
        GameScreen.OnMiniMapPositionUpdate(transform.position);
    }

    public void SpawnLocalPlayer(Pilot pilot)
    {
        LocalShipController = CreateModel(LocalShipLayer, new MapObject(pilot), true);

        AllObjectsController.Add(pilot.Id, LocalShipController);

        OnPilotDataChange(pilot);
        OnMapChange(pilot.Map);
    }

    public void OnSpawnMapObject(SpawnMapObjectResponse spawnMapObjectResponse)
    {
        var mapObject = spawnMapObjectResponse.MapObject;

        if (AllObjectsController.ContainsKey(mapObject.Id))
        {
            Debug.LogError($"OnSpawnMapObject : {mapObject.Id} already exist!");
            return;
        }

        var shipController = CreateModel(OtherObjectsLayer, mapObject);
        AllObjectsController.Add(mapObject.Id, shipController);
    }

    public void OnSpawnEnvironmentObjectResponse(SpawnEnvironmentObjectResponse response)
    {
        var environmentObject = response.EnvironmentObject;
        if (AllEnvironments.ContainsKey(environmentObject.Id))
        {
            Debug.LogWarning($"OnSpawnEnvironmentObjectResponse : {environmentObject.Id} already exist!"); // TODO update prefab
            return;
        }

        GameObject model = Helpers.LoadPrefabResource(environmentObject.OwnerId == Client.Pilot.Id ? response.OriginalPrefabType : environmentObject.PrefabType);
        if (model == null)
        {
            throw new NotImplementedException($"{environmentObject.PrefabType} || {response.OriginalPrefabType}");
        }
        GameObject environmentGameObject = Instantiate(model, EnvironmentsTransform);

        var controller = environmentGameObject.GetComponent<OreController>();
        controller.SetEnvironmentObject(environmentObject);

        AllEnvironments.Add(environmentObject.Id, controller);
    }

    public void OnDisposeMapObject(DisposeMapObjectResponse disposeMapObjectResponse)
    {
        var mapObjectId = disposeMapObjectResponse.ObjectId;

        if (!AllObjectsController.ContainsKey(mapObjectId))
        {
            Debug.LogError($"OnDisposeMapObject : {mapObjectId} not exist to dispose!");
            return;
        }

        if (LocalShipController.MapObject.Id == mapObjectId)
        {
            Debug.LogWarning($"OnDisposeMapObject : local player dispose.");
            return;
        }

        DisposeMapObject(mapObjectId);
    }
    public void DisposeMapObject(Guid guid)
    {
        Destroy(AllObjectsController[guid].gameObject);
        AllObjectsController.Remove(guid);
    }

    public void OnDisposeEnvironmentObjectResponse(DisposeEnvironmentObjectResponse response)
    {
        var environmentObjectId = response.ObjectId;

        if (!AllEnvironments.ContainsKey(environmentObjectId))
        {
            Debug.LogError($"OnDisposeEnvironmentObjectResponse : {environmentObjectId} not exist to dispose!");
            return;
        }

        Destroy(AllEnvironments[environmentObjectId].gameObject);
        AllEnvironments.Remove(environmentObjectId);
    }

    public void OnSafeZoneResponse(SafeZoneResponse safeZoneResponse)
    {
        LogMessage.NewMessage($"Safe zone {safeZoneResponse.Status}");
    }

    public void OnChangeEnvironmentObject(ChangeEnvironmentObjectResponse response)
    {
        if (AllEnvironments.ContainsKey(response.ObjectId))
            AllEnvironments[response.ObjectId].OnChangeEnvironmentObject(response.NewPrefabType);
    }

    public void OnDestroyMapObjectResponse(DestroyMapObjectResponse destroyMapObjectResponse)
    {
        var mapObjectId = destroyMapObjectResponse.ObjectId;

        if (!AllObjectsController.ContainsKey(mapObjectId))
        {
            Debug.LogError($"OnDestroyMapObjectResponse : {mapObjectId} not exist to destroy!");
            return;
        }

        foreach (var item in AllObjectsController.Where(o => o.Value.SelectedMapObject?.MapObject.Id == mapObjectId))
        {
            item.Value.SelectedMapObject = null;
        }

        if (LocalShipController.MapObject.Id == mapObjectId)
        {
            Debug.LogWarning($"OnDestroyMapObjectResponse : local player destroy.");
            return;
        }

        // Spawn explosion

        Destroy(AllObjectsController[mapObjectId].gameObject);
        AllObjectsController.Remove(mapObjectId);
    }

    public void OnRewardResponse(RewardResponse rewardResponse)
    {
        GameScreen.OnRewardGain(rewardResponse);
    }

    private ShipController CreateModel(Transform transform, MapObject mapObject, bool isLocalplayer = false)
    {
        mapObject.ShipType = mapObject.RankType.HasValue ? mapObject.ShipType : AbstractEnemy.GetEnemyPrefab(mapObject.ShipType);
        var model = Helpers.LoadPrefabResource(mapObject.ShipType);
        if (model == null)
        {
            throw new NotImplementedException(mapObject.ShipType.ToString());
        }
        GameObject shipGameObject = Instantiate(model, transform);
        var shipController = shipGameObject.GetComponent<ShipController>();

        shipController.SetMapObject(mapObject, isLocalplayer ? gameObject.transform : shipController.transform);
        shipController.transform.name = mapObject.Id.ToString();
        if (isLocalplayer)
            shipController.transform.tag = "LocalPlayer";

        Debug.Log($"CreateModel : {mapObject.Id} , {mapObject.Name}");

        return shipController;
    }

    public void OnChangeMapObjectPosition(ChangeMapObjectPositionResponse response)
    {
        if (AllObjectsController.ContainsKey(response.ObjectId))
            AllObjectsController[response.ObjectId].OnChangeMapObjectPosition(response);
    }

    public void OnChangeLife(ChangeLifeResponse response)
    {
        if (AllObjectsController.ContainsKey(response.ObjectId))
            AllObjectsController[response.ObjectId].OnChangeLife(response);

        if (response.ObjectId == LocalShipController.MapObject.Id)
        {
            GameScreen.OnHitpointsChange(response.Hitpoints, response.MaxHitpoints);
            GameScreen.OnShieldsChange(response.Shields, response.MaxShields);
        }
    }

    public void OnPilotDataChange(Pilot pilot)
    {
        GameScreen.OnPilotDataChange(pilot);
    }

    public void OnResourceUpdateResponse(ResourceUpdateResponse response)
    {
        Debug.LogWarning(response);
    }
    public void OnAttackResponse(AttackResponse response)
    {
        if (AllObjectsController.ContainsKey(response.AttackerId))
        {
            if (response.TargetId.HasValue && AllObjectsController.ContainsKey(response.TargetId.Value))
            {
                AllObjectsController[response.AttackerId].OnAttackResponse(response, AllObjectsController[response.TargetId.Value], BulletTransform);
            }
            else
            {
                AllObjectsController[response.AttackerId].SelectedMapObject = null;
            }
        }
    }



    public void OnMapChange(MapTypes mapType)
    {
        var previousMap = AbstractMap.GetMapByType(Client.Pilot.Map);

        Client.Pilot.Map = mapType;

        Helpers.DestroyAllChilds(MapTransform);
        Helpers.DestroyAllChilds(BaseTransform);
        Helpers.DestroyAllChilds(PortalsTransform);
        Helpers.DestroyAllChilds(EnvironmentsTransform);

        var map = AbstractMap.GetMapByType(mapType);

        var mapPrefab = Helpers.LoadMapResource(map.MapType);

        Instantiate(mapPrefab, MapTransform);

        if (map.Base_PrefabType.HasValue)
        {
            GameObject baseObject = Instantiate(Helpers.LoadPrefabResource(map.Base_PrefabType.Value), BaseTransform);
            var position = map.Base_Position.ToVector();
            baseObject.transform.position = new Vector3(position.x, position.y, baseObject.transform.position.z);
        }

        foreach (var portal in map.Portals)
        {
            GameObject portalObject = Instantiate(Helpers.LoadPrefabResource(portal.PrefabType), PortalsTransform);
            var position = portal.Position.ToVector();
            portalObject.transform.position = new Vector3(position.x, position.y, portalObject.transform.position.z);
            portalObject.GetComponent<PortalController>().Portal = portal;
        }

        Vector3? portalPos = previousMap.Portals.FirstOrDefault(o => o.Target_MapType == mapType)?.Target_Position?.ToVector();

        if (portalPos != null && portalPos.HasValue)
        {
            LocalShipController.OnNewTargetPosition(portalPos.Value);
            LocalShipController.Position = portalPos.Value;
        }
    }


    // Chat:
    public void OnConnectToChannelResponse(ConnectToChannelResponse connectToChannelResponse)
    {
        ChatScreen.OnConnectToChannelResponse(connectToChannelResponse);
    }

    public void OnChatMessageResponse(ChatMessageResponse chatMessageResponse)
    {
        ChatScreen.OnChatMessageResponse(chatMessageResponse);
    }
}