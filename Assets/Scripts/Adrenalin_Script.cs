using UnityEngine;

public class Adrenalin_Script : MonoBehaviour
{
    [SerializeField] private PlayerStats player;
    [SerializeField] public int manaBoost;

    private ItemObject itemObject;

    private void Start()
    {
        itemObject = GetComponent<ItemObject>();
    }
    private void OnTriggerEnter(Collider other) //pick up item when collide.
    {
        if (other.CompareTag("Player"))
        {
            itemObject.OnPickUp();
        }
    }
}
