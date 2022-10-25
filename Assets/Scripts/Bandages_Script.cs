using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandages_Script : MonoBehaviour
{
    [SerializeField] private PlayerStats player;
    [SerializeField] public int healthBoost;

    private ItemObject itemObject;

    private void Start()
    {
        itemObject = GetComponent<ItemObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            itemObject.OnPickUp();
        }
    }
}
