using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryRemember
{
    private static List<InventoryItem> inventory;

    public static void Remember(InventorySystem inventorySystem)
    {
        inventory = new List<InventoryItem>(inventorySystem.inventory);
    }
    public static List<InventoryItem> GetInventory()
    {
        return inventory;
    }
}
