using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private GameObject slotPrefab;

    private ItemSlot slot;
    private InventoryItem item;

    void Start()
    {
        if (inventorySystem.inventory.Count != 0) //draw current inventory UI.
        {
            DrawInventory();
        }

        InventorySystem.OnChange += OnUpdateInventory; //when inventoy changes then update UI.
    }
    private void OnDestroy()
    {
        InventorySystem.OnChange -= OnUpdateInventory;
    }

    private void OnUpdateInventory()
    {
        foreach (Transform t in transform)//destoy current inventory UI.
        {
            Destroy(t.gameObject);
        }
        DrawInventory();//draw new one.
    }

    public void DrawInventory()
    {
        int slotNum = 1;

        foreach (InventoryItem item in inventorySystem.inventory) //getting each item in the inventory system list.
        {
            if (slotNum <= inventorySystem.inventory.Count)
                item.SlotNum(slotNum);//updating slot number.

            AddInventorySlot(item); //addin new item slot.

            if (this.item == item)//when item selected.
            {
                slot.GetComponent<ItemSlot>().ActivateSelected();
            }
            slotNum++;
        }
    }
    public void AddInventorySlot(InventoryItem item) //addint new item slot as children to inventory.
    {
        GameObject obj = Instantiate(slotPrefab);
        obj.transform.SetParent(transform, false);

        slot = obj.GetComponent<ItemSlot>();
        slot.Set(item);
    }
    public void SelectedItem(InventoryItem item)
    {
        foreach (Transform slot in GetComponentInChildren<Transform>()) //unselect all items.
        {
            slot.GetComponent<ItemSlot>().DeActivateSelected();
        }
        foreach (Transform slot in GetComponentInChildren<Transform>())//activate selected item.
        {
            if (slot.GetComponent<ItemSlot>()?.item == item)//when current item equels to what player selected.
            {
                this.item = item;
                slot.GetComponent<ItemSlot>().ActivateSelected();//if does then activate selected.
                return;
            }
        }
    }
}
