using UnityEngine;

public class CardReaderBreakable : MonoBehaviour
{
    [SerializeField] private GameObject backDoor;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="PlayerWeapon")//when weapon collide with breakable cardreader.
        {
            backDoor.SetActive(false);//open door.
        }
    }
}
