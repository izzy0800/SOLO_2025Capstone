using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum ItemType
{
    None,
    Badge,
    Tonic,
    Key,
    Letter,
}

public class ItemData : MonoBehaviour
{
    [Header("Item Information")]
    public ItemType itemType = ItemType.None;
    public string itemName = "Item";
    [TextArea(2, 4)]
    public string itemDescription = "";

    void Start()
    {
        if (!gameObject.CompareTag("Pickup"))
        {
            gameObject.tag = "Pickup";
        }
    }
}
