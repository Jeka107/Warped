using UnityEngine;

public class CeilingSignal : MonoBehaviour
{
    [SerializeField] private float drag;
    [SerializeField] private float waitTime;

    private Rigidbody rb;
    private MoveObject moveObject;
    private void Start()
    {
        rb=GetComponent<Rigidbody>();
        moveObject = GetComponent<MoveObject>();
    }
    private void Update()
    {
        if(moveObject?.moveToPlayer== true) //if player activate teleknisis on this gameobject.
        {
            rb.useGravity = true;
            rb.drag = 1;
        }
    }
    public void Signal() //used on timeline
    {
        rb.isKinematic = false;
        rb.drag = drag;
        GetComponent<Animator>().enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player")) //on collide with playre ,kill player.
        {
            collision.collider.GetComponent<PlayerStats>()?.ReduceHp(100);
        }
    }
}
