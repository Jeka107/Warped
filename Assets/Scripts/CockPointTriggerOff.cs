using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockPointTriggerOff : MonoBehaviour
{
    [SerializeField] private GravityController gravityController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            gravityController.cockPitRoom = false;
        }
    }
}
