using UnityEngine;

public class CockPointTriggerOff : MonoBehaviour
{
    [SerializeField] private GravityController gravityController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")//on collide with player ,player is not in cockpit.
        {
            gravityController.cockPitRoom = false;
        }
    }
}
