using NostalgiaOrbitDLL;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    private Item Item;

    public void Setup(Item item)
    {
        Item = item;

        GetComponent<RawImage>().texture = Helpers.LoadInventoryTextureResource(item.ItemType);
    }
}