using NostalgiaOrbitDLL;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    private Item Item;
    private PilotResource Resource;

    public void Setup(Item item)
    {
        Item = item;

        GetComponent<RawImage>().texture = Helpers.LoadInventoryTextureResource(item.ItemType);
    }

    public void Setup(PilotResource resource)
    {
        Resource = resource;

        GetComponent<RawImage>().texture = Helpers.LoadInventoryTextureResource(resource.ResourceType);
    }
}