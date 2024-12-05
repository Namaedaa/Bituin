using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]

public class Item : ScriptableObject
{
    ItemPopUpManager itemPopUpManager;

    new public string name = "New Item";
    public string description = "Description";
    public Sprite icon = null;
    public bool isDefaultItem = false;

    public virtual void Use()
    {
        itemPopUpManager = FindObjectOfType<ItemPopUpManager>();
        itemPopUpManager.OpenItemPopUp(name, description);
    }
}
