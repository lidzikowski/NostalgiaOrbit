using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core.Commands;
using NostalgiaOrbitDLL.Core.Responses;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteAnimation))]
public class ShipController : MonoBehaviour
{
    [SerializeField]
    public DroneLayerController DronesLayer;
    private SpriteAnimation ShipAnimation;

    [SerializeField]
    public TMP_Text NameText;

    [SerializeField]
    public Transform CompanyPlayerCircle;
    [SerializeField]
    public Transform EnemyCircle;

    [SerializeField]
    public GameObject ObjectCircle;
    [SerializeField]
    public GameObject AttackedObjectCircle;

    [SerializeField]
    public Transform HitpointBar;
    [SerializeField]
    public Transform ShieldBar;

    [SerializeField]
    public GameObject DamageMessagePrefab;

    [SerializeField]
    public Transform DamageTransform;



    public MapObject MapObject { get; private set; }
    private Transform ParentTransform;
    private Vector3 TargetPosition
    {
        get => MapObject.TargetPosition.ToVector();
        set
        {
            MapObject.TargetPosition = value.ToPositionVector();

            // Localplayer synchronize
        }
    }

    public Vector3 Position
    {
        get => MapObject.Position.ToVector();
        set
        {
            if (IsLocalPlayer)
            {
                value.z = -5;
                Client.Pilot.Position = value.ToPositionVector();
            }
            else if (MapObject.FirmType.HasValue)
            {
                value.z = -3;
            }

            ParentTransform.position = value;
            MapObject.Position = value.ToPositionVector();
        }
    }

    private ShipController selectedMapObject;
    public ShipController SelectedMapObject
    {
        get => selectedMapObject;
        set
        {
            if (selectedMapObject == value)
                return;

            if (selectedMapObject != null && !selectedMapObject.IsLocalPlayer)
                selectedMapObject.UnSelectObject();

            selectedMapObject = value;

            AmmunitionAttackSelectedObject = false;

            if (value != null && IsLocalPlayer)
            {
                Client.SendToSocket(ServerChannels.Game, new SelectTargetCommand(SelectedMapObject.MapObject.Id, false, false));

                value.SelectObject();
            }
        }
    }
    private bool ammunitionAttackSelectedObject;
    public bool AmmunitionAttackSelectedObject
    {
        get => ammunitionAttackSelectedObject;
        set
        {
            if (ammunitionAttackSelectedObject == value)
                return;

            ammunitionAttackSelectedObject = value;

            if (SelectedMapObject == null)
                return;

            if (IsLocalPlayer)
            {
                Client.SendToSocket(ServerChannels.Game, new SelectTargetCommand(SelectedMapObject.MapObject.Id, AmmunitionAttackSelectedObject, false));

                if (value)
                {
                    LogMessage.NewMessage($"Attacking {SelectedMapObject.MapObject.Name}"); // TODO lang
                    SelectedMapObject.AttackObject(true);
                }
                else
                {
                    LogMessage.NewMessage($"Stop attack {SelectedMapObject.MapObject.Name}"); // TODO lang
                    SelectedMapObject.SelectObject();
                }
            }
        }
    }

    public void UseRocket()
    {
        if (IsLocalPlayer)
        {
            LogMessage.NewMessage($"Use rocket on {SelectedMapObject.MapObject.Name}"); // TODO lang
            Client.SendToSocket(ServerChannels.Game, new SelectTargetCommand(SelectedMapObject.MapObject.Id, AmmunitionAttackSelectedObject, true));
        }
    }

    public bool IsLocalPlayer { get => MapObject.Id == Client.Pilot.Id; }



    private void Start()
    {
        DamageTransform = GameObject.Find("Bullets").transform;
        ShipAnimation = GetComponent<SpriteAnimation>();
    }

    private bool synchronizeTargetPosition;
    private float bulletsTimer;
    private void Update()
    {
        if (MapObject == null)
            return;

        Fly();

        if (IsLocalPlayer && !synchronizeTargetPosition && lastUpdateTargetPosition != TargetPosition)
        {
            synchronizeTargetPosition = true;

            StartCoroutine(SynchronizeTargetPosition());
        }

        bulletsTimer += Time.deltaTime;

        if (bulletsTimer > 0.2f && receivedDamageInHitpoints > 0)
        {
            bulletsTimer = 0;

            CreateDamagePrefab(receivedDamageInHitpoints);
            receivedDamageInHitpoints = 0;
        }
    }

    private void FixedUpdate()
    {
        if (AmmunitionAttackSelectedObject && SelectedMapObject != null)
        {
            ShipAnimation?.RotateToPosition(SelectedMapObject.Position);
            DronesLayer?.SetTargetPosition(Position, SelectedMapObject.Position);
        }
        else
        {
            ShipAnimation?.RotateToPosition(TargetPosition);
            DronesLayer?.SetTargetPosition(Position, TargetPosition);
        }
    }

    private Vector3 lastUpdateTargetPosition;
    private IEnumerator SynchronizeTargetPosition()
    {
        lastUpdateTargetPosition = TargetPosition;

        Client.SendToSocket(ServerChannels.Game, new ChangePositionCommand(MapObject));

        yield return new WaitForSeconds(0.2f);

        synchronizeTargetPosition = false;
    }

    private void Fly()
    {
        if (Position != TargetPosition)
        {
            Position = Vector3.MoveTowards(Position, TargetPosition, Time.deltaTime * (MapObject.Speed / 20));
            if (IsLocalPlayer)
                GearStatus(true);
        }
        else if (IsLocalPlayer)
            GearStatus(false);
    }

    private AudioSource localAudioSource;
    public void GearStatus(bool status)
    {
        if (status && !localAudioSource.isPlaying)
            localAudioSource.Play();
        else if (!status && localAudioSource.isPlaying)
            localAudioSource.Stop();
    }

    public void SetMapObject(MapObject mapObject, Transform parent)
    {
        MapObject = mapObject;
        ParentTransform = parent;

        ParentTransform.position = mapObject.Position.ToVector();

        OnChangeShip(mapObject.ShipType);
        SpawnDrones(mapObject.Drones);
        SetName(mapObject);
        UpdateShipBars();

        localAudioSource = transform.parent.GetComponent<AudioSource>();

        if (IsLocalPlayer)
        {
            HitpointBar.gameObject.SetEnable();
            ShieldBar.gameObject.SetEnable();
        }
    }

    private void SpawnDrones(List<Drone> drones)
    {
        DronesLayer.SetDrones(drones);
    }

    private void SetName(MapObject mapObject)
    {
        var name = string.Empty;

        CompanyPlayerCircle.gameObject.SetDisable();
        EnemyCircle.gameObject.SetDisable();

        if (Client.Pilot.Id == MapObject.Id)
        {
            name += "<color=white>";
        }
        else if (Client.Pilot.FirmType == MapObject.FirmType)
        {
            name += "<color=#00C5FF>";
            CompanyPlayerCircle.gameObject.SetEnable();
        }
        else
        {
            name += "<color=#FF3C00>";
            EnemyCircle.gameObject.SetEnable();
        }

        if (MapObject.RankType.HasValue)
        {
            name += $"<sprite={(int)MapObject.RankType.Value}> ";
        }

        name += mapObject.Name;

        if (MapObject.FirmType.HasValue)
        {
            name += $"  <sprite={((int)RankTypes.Outlaw + (int)MapObject.FirmType.Value)}>";
        }

        NameText.text = name;
    }

    #region Events
    public void OnNewTargetPosition(Vector3 targetPosition)
    {
        TargetPosition = targetPosition;
    }

    public void OnChangeMapObjectPosition(ChangeMapObjectPositionResponse response)
    {
        if (Vector2.Distance(Position, response.Position.ToVector()) > 30)
            Position = response.Position.ToVector();

        TargetPosition = response.TargetPosition.ToVector();
        MapObject.Speed = response.Speed;
    }

    private long receivedDamageInHitpoints = 0;
    public void OnChangeLife(ChangeLifeResponse response)
    {
        var totalDamage = MapObject.Hitpoints - response.Hitpoints + MapObject.Shields - response.Shields;

        if (totalDamage > 0)
        {
            receivedDamageInHitpoints += totalDamage;
        }
        else if (totalDamage == 0)
        {
            StartCoroutine(CreateSlowDamagePrefab(totalDamage));
        }
        else
        {
            StartCoroutine(CreateSlowDamagePrefab(totalDamage));
        }

        MapObject.Hitpoints = response.Hitpoints;
        MapObject.MaxHitpoints = response.MaxHitpoints;

        MapObject.Shields = response.Shields;
        MapObject.MaxShields = response.MaxShields;

        UpdateShipBars();
    }

    private IEnumerator CreateSlowDamagePrefab(long damage)
    {
        yield return new WaitForSeconds(0.3f);
        CreateDamagePrefab(damage);
    }

    private void CreateDamagePrefab(long damage)
    {
        var damageMessage = Instantiate(DamageMessagePrefab, DamageTransform);

        var position = transform.position;
        position.y += 8;
        position.z = -10;
        damageMessage.transform.position = position;

        damageMessage.GetComponent<DamageMessage>().SetText(damage);
    }

    private void UpdateShipBars()
    {
        var hitpoints = HitpointBar.localScale;
        hitpoints.x = (float)MapObject.Hitpoints / (float)MapObject.MaxHitpoints;
        HitpointBar.localScale = hitpoints;

        var shields = ShieldBar.localScale;
        if (MapObject.MaxShields > 0)
        {
            shields.x = (float)MapObject.Shields / (float)MapObject.MaxShields;
            ShieldBar.localScale = shields;
        }
        else
        {
            shields.x = 0;
            ShieldBar.localScale = shields;
        }
    }

    public void OnChangeShip(PrefabTypes prefabType)
    {
        GetComponent<SpriteAnimation>().ChangePrefabModel(prefabType);
    }
    #endregion Events

    public void SelectObject()
    {
        ObjectCircle.SetEnable();
        AttackedObjectCircle.SetDisable();

        HitpointBar.gameObject.SetEnable();
        ShieldBar.gameObject.SetEnable();
    }

    public void UnSelectObject()
    {
        ObjectCircle.SetDisable();
        AttackedObjectCircle.SetDisable();

        HitpointBar.gameObject.SetDisable();
        ShieldBar.gameObject.SetDisable();
    }

    public void AttackObject(bool isMy)
    {
        ObjectCircle.SetDisable();
        AttackedObjectCircle.SetEnable();

        if (isMy)
        {
            AttackedObjectCircle.GetComponent<SpriteRenderer>().color = new Color(197, 0, 0, 150);
        }
        else
        {
            AttackedObjectCircle.GetComponent<SpriteRenderer>().color = new Color(106, 106, 106, 150);
        }
    }

    public void OnAttackResponse(AttackResponse response, ShipController targetController, Transform bulletTransform)
    {
        SelectedMapObject = targetController;
        AmmunitionAttackSelectedObject = response.ResourceType.HasValue;

        if (response.ResourceType.HasValue)
        {
            var bullet = Instantiate(Helpers.LoadPrefabResource(response.ResourceType.Value), bulletTransform);
            var bulletController = bullet.GetComponent<BulletController>();

            bulletController.Setup(Position, SelectedMapObject.Position);
        }
    }
}