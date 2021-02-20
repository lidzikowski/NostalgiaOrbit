using NostalgiaOrbitDLL;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class Helpers
{
    private static Dictionary<PrefabTypes, Sprite[]> spriteCache = new Dictionary<PrefabTypes, Sprite[]>();
    private static Dictionary<PrefabTypes, GameObject> prefabCache = new Dictionary<PrefabTypes, GameObject>();
    private static Dictionary<ResourceTypes, GameObject> resourceCache = new Dictionary<ResourceTypes, GameObject>();

    public static Sprite[] LoadSpritesResource(PrefabTypes resource)
    {
        if (!spriteCache.ContainsKey(resource))
            spriteCache.Add(resource, Resources.LoadAll<Sprite>($"sprites/sprite/{resource}/"));

        Debug.Log($"Załadowano tekstury '{resource}' : '{spriteCache[resource].Length}'");

        return spriteCache[resource];
    }

    public static GameObject LoadPrefabResource(PrefabTypes resource)
    {
        if (!prefabCache.ContainsKey(resource))
            prefabCache.Add(resource, Resources.Load<GameObject>($"sprites/prefabs/{resource}"));

        Debug.Log($"Załadowano prefab '{resource}'");

        return prefabCache[resource];
    }

    public static GameObject LoadPrefabResource(ResourceTypes resource)
    {
        if (!resourceCache.ContainsKey(resource))
            resourceCache.Add(resource, Resources.Load<GameObject>($"sprites/sprite/{resource}/{resource}"));

        Debug.Log($"Załadowano resource '{resource}'");

        return resourceCache[resource];
    }

    public static GameObject LoadMapResource(MapTypes resource)
    {
        var map = Resources.Load<GameObject>($"maps_4k/prefabs/{resource}");

        Debug.Log($"Załadowano mape '{resource}'");

        return map;
    }

    public static void DestroyAllChilds(Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }


    public static readonly string ThousandSeparator = "### ### ### ###";
    public static readonly string DoubleSeparator = $"#,0.00";
    public static NumberFormatInfo NumberFormat
    {
        get
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            return nfi;
        }
    }
}