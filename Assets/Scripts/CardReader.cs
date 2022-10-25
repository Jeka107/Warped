using UnityEngine;

public class CardReader : MonoBehaviour
{
    [SerializeField] private GameObject slidingDoor;
    [SerializeField] private GameObject openningDoorSound;

    public void UseObject(InventoryItemData item)
    {
        if (item?.id == "Item_Key_card") //if player holding key card.
        {
            //Open Door.
            Debug.Log("Door Opened");

            slidingDoor.GetComponent<MeshCollider>().enabled = false;

            foreach (Transform door in slidingDoor?.GetComponentInChildren<Transform>())//open door
            {
                door.gameObject.SetActive(false);
                openningDoorSound.GetComponent<AudioSource>().Play();
            }
        }
    }
}