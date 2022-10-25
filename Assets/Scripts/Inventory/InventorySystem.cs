using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public delegate void OnInventoryChangedEvent();
    public static event OnInventoryChangedEvent OnChange;

    [SerializeField] public List<InventoryItem> inventory;
    [SerializeField] private GameManager gameManager;
    private Dictionary<InventoryItemData, InventoryItem> itemDictionry;


    private void Awake()
    {
        inventory = InventoryRemember.GetInventory();

        if (inventory==null)
        { 
            inventory = new List<InventoryItem>();
        }
        
        itemDictionry = new Dictionary<InventoryItemData, InventoryItem>();
    }

    public InventoryItem Get(InventoryItemData data)//get all the inventory
    {
        if(itemDictionry.TryGetValue(data,out InventoryItem value))
        {
            return value;
        }
        return null;
    }
    public void Add(InventoryItemData data)//add item to inventory
    {
        if(itemDictionry.TryGetValue(data,out InventoryItem value))
        {
            value.AddToStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(data,inventory.Count+1);
            inventory.Add(newItem);
            itemDictionry.Add(data, newItem);
        }
        OnChange?.Invoke();
    }
    public void Remove(InventoryItemData data)//remove item from inventory
    {
        if(itemDictionry.TryGetValue(data,out InventoryItem value))
        {
            value.RemoveFromStack();

            if(value.stackSize==0)
            {
                inventory.Remove(value);
                itemDictionry.Remove(data);

                foreach(InventoryItem item in inventory)//update slot key number.
                {
                    item.SlotNum(inventory.Count);
                }
            }
        }
        OnChange?.Invoke();
    }
    public InventoryItem ItemUse(int i)//hold choosen item
    {
        if (i-1 < inventory.Count)
        {
            if (inventory[i - 1] != null)
                return inventory[i - 1];
        }
        return null;
    }
}


