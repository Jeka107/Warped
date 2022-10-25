using UnityEngine;

public class CockPointTriggerOn : MonoBehaviour
{
    [SerializeField] private GravityController gravityController;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player")//on collide with player,player is in cockpit.
        {
            gravityController.cockPitRoom = true;
        }
    }
}
