using NostalgiaOrbitDLL;
using System;
using UnityEngine;

[Serializable]
public class HangarShopManager
{
    [SerializeField]
    public ItemShopTypes ItemType;

    [SerializeField]
    public HangarShopItemInformationParent ItemInformation;
}