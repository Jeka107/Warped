using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockPointTriggerOn : MonoBehaviour
{
    [SerializeField] private GravityController gravityController;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player")
        {
            gravityController.cockPitRoom = true;
        }
    }
}
