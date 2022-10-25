using System.Collections.Generic;

public class InventoryRemember
{
    private static List<InventoryItem> inventory;

    //used for remembering inventory when player moves to next level.
    public static void Remember(InventorySystem inventorySystem)
    {
        inventory = new List<InventoryItem>(inventorySystem.inventory);
    }
    public static List<InventoryItem> GetInventory()
    {
        return inventory;
    }
}
