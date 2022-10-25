using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private GameObject slotPrefab;

    private ItemSlot slot;
    private InventoryItem item;

    void Start()
    {
        if (inventorySystem.inventory.Count != 0)
        {
            DrawInventory();
        }

        InventorySystem.OnChange += OnUpdateInventory;
    }
    private void OnDestroy()
    {
        InventorySystem.OnChange -= OnUpdateInventory;
    }

    private void OnUpdateInventory()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        DrawInventory();
    }

    public void DrawInventory()
    {
        int slotNum = 1;

        foreach (InventoryItem item in inventorySystem.inventory)
        {
            if (slotNum <= inventorySystem.inventory.Count)
                item.SlotNum(slotNum);

            AddInventorySlot(item);

            if (this.item == item)
            {
                slot.GetComponent<ItemSlot>().ActivateSelected();
            }
            slotNum++;
        }
    }
    public void AddInventorySlot(InventoryItem item)
    {
        GameObject obj = Instantiate(slotPrefab);
        obj.transform.SetParent(transform, false);

        slot = obj.GetComponent<ItemSlot>();
        slot.Set(item);
    }
    public void SelectedItem(InventoryItem item)
    {
        foreach (Transform slot in GetComponentInChildren<Transform>())
        {
            slot.GetComponent<ItemSlot>().DeActivateSelected();
        }
        foreach (Transform slot in GetComponentInChildren<Transform>())
        {
            if (slot.GetComponent<ItemSlot>()?.item == item)
            {
                this.item = item;
                slot.GetComponent<ItemSlot>().ActivateSelected();
                return;
            }
        }
    }
}
