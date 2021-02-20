using NostalgiaOrbitDLL;
using System;
using UnityEngine;

[Serializable]
public class HangarManager
{
    [SerializeField]
    public PrefabTypes ShipType;

    [SerializeField]
    public HangarShip HangarShip;
}