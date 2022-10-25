using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalRadius : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private GravityController gravityController;

    private void Awake()
    {
        GetComponent<SphereCollider>().radius = radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Draggable")
        {
            gravityController.AddToList(other.gameObject);//adding to list draggable objects.
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Draggable")
        {
            gravityController.RemoveFromList(other.gameObject);//removing from list draggable objects.
        }
    }
}
