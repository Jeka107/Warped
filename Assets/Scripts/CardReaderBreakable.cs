using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardReaderBreakable : MonoBehaviour
{
    [SerializeField] private GameObject backDoor;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="PlayerWeapon")
        {
            backDoor.SetActive(false);
        }
    }
}
