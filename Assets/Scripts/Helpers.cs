using NostalgiaOrbitDLL;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public static class Helpers
{
    private static Dictionary<PrefabTypes, Sprite[]> spriteCache = new Dictionary<PrefabTypes, Sprite[]>();
    private static Dictionary<PrefabTypes, GameObject> prefabCache = new Dictionary<PrefabTypes, GameObject>();
    private static Dictionary<ResourceTypes, GameObject> resourceCache = new Dictionary<ResourceTypes, GameObject>();
    private static Dictionary<ItemTypes, Texture> InventoryTextureCache = new Dictionary<ItemTypes, Texture>();
    private static Dictionary<ResourceTypes, Texture> InventoryResourceTextureCache = new Dictionary<ResourceTypes, Texture>();

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

    public static Texture LoadInventoryTextureResource(ItemTypes resource)
    {
        if (!InventoryTextureCache.ContainsKey(resource))
            InventoryTextureCache.Add(resource, Resources.Load<Texture>($"ui/items/{resource}"));

        Debug.Log($"Załadowano inventory texture '{resource}'");

        return InventoryTextureCache[resource];
    }

    public static Texture LoadInventoryTextureResource(ResourceTypes resource)
    {
        if (!InventoryResourceTextureCache.ContainsKey(resource))
            InventoryResourceTextureCache.Add(resource, Resources.Load<Texture>($"ui/items/{resource}"));

        Debug.Log($"Załadowano inventory texture '{resource}'");

        return InventoryResourceTextureCache[resource];
    }

    public static void DestroyAllChilds(Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }


    public static readonly string ThousandSeparator = "### ### ###";
    public static readonly string DoubleSeparator = $"#,#.00";
    public static NumberFormatInfo NumberFormat
    {
        get
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            return nfi;
        }
    }

    public static void RefreshUI(Transform transform)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
    }

    public static string GetItemPrice(ShopItem shopItem, int i = 0)
    {
        if (shopItem.CanBuyByCredit)
        {
            var price = shopItem.CreditPurchase[i];
            return price > 0 ? price.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat) + " C." : "0 C.";
        }
        else if (shopItem.CanBuyUridium)
        {
            var price = shopItem.UridiumPurchase[i];
            return price > 0 ? price.ToString(Helpers.ThousandSeparator, Helpers.NumberFormat) + " U." : "0 U.";
        }
        else
        {
            return "-";
        }
    }
}