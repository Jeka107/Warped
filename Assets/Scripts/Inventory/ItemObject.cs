using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public InventoryItemData referenceItem;

    public void OnPickUp()
    {
        FindObjectOfType<InventorySystem>().Add(referenceItem); //adding item to inventory system.
        Destroy(gameObject);
    }
}
