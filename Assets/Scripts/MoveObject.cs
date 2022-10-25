using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [SerializeField] private GameObject stopMovingObject;
    [SerializeField] private float pullPower = 10f;
    [SerializeField] private float throwPower=20f;
    [SerializeField] private float power = 1f;
    [SerializeField] private float smooth = 5f;

    private PlayerMovement playerMovement;
    private PlayerActions playerActions;
    private Rigidbody rb;
    private Collider myCollider;
    private GameObject hitObject;

    [HideInInspector] public bool moveToPlayer = false;
    private bool doAction = true;
    private Vector3 velocity = Vector3.zero;

    [HideInInspector] public bool holdingObject = false;
    
    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerActions = FindObjectOfType<PlayerActions>();
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        stopMovingObject.SetActive(false);
    }
    private void Update()
    {
        if (moveToPlayer && !holdingObject)//pull draggable object to player(following the player).
        {
            transform.position = Vector3.MoveTowards(transform.position, Camera.main.transform.position, pullPower*Time.deltaTime);
        }
        else if (holdingObject && this.tag == "Shoot")//set draggable object infront.
        {
            var followPosition = Camera.main.transform.localPosition + Camera.main.transform.forward * 5 + Vector3.up;
            //transform.position =new Vector3(followPosition.x, followPosition.y, followPosition.z);

            transform.position = Vector3.SmoothDamp(transform.position, followPosition,ref velocity, smooth * Time.deltaTime);
            rb.velocity = Vector3.zero;

            stopMovingObject.SetActive(false);
        }
        
    }
    
    public void MoveToPlayer(GameObject hitObject)
    {
        moveToPlayer = true;
        this.hitObject = hitObject;
        transform.gameObject.tag = "Shoot";
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.angularVelocity = new Vector3(1, 0, 0);
        stopMovingObject.SetActive(true);
        LayerChangeItemHold();
    }
    public void ThrowObject()
    {
        if (doAction)
        {
            LayerChangeIDraggable();

            rb.velocity = Camera.main.transform.forward * throwPower*Time.fixedDeltaTime;
            transform.gameObject.tag = Enums.HIT_OBJECT_TAGS.Draggable.ToString();
            myCollider.isTrigger = false;

            if (!playerActions.activateSkill)
            {
                rb.useGravity = true;
            }

            stopMovingObject.SetActive(false);
            holdingObject = false;
            moveToPlayer = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "StopMoving"&&this.gameObject== hitObject)
        {
            rb.velocity = Vector3.zero;
            holdingObject = true;
            playerActions.SetKeyCode(KeyCode.E);
        }
        if(other.tag==Enums.HIT_OBJECT_TAGS.Ground.ToString())
        {
            doAction = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == Enums.HIT_OBJECT_TAGS.Ground.ToString())
        {
            doAction = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == Enums.HIT_OBJECT_TAGS.Draggable.ToString())||
            (collision.gameObject.tag == "Player") && rb.velocity != Vector3.zero)
        {
            var collisionRb = collision.gameObject.GetComponent<Rigidbody>();
            var direction = (collision.gameObject.transform.position - this.transform.position).normalized;

            if (collisionRb.isKinematic) { collisionRb.isKinematic = false; }
            
            collisionRb.AddForce(direction * power*Time.fixedDeltaTime, ForceMode.Force);
        } 
    }

    private void LayerChangeItemHold()
    {
        int LayerIgnoreRaycast = LayerMask.NameToLayer(Enums.HIT_OBJECT_Layers.ItemHold.ToString());
        gameObject.layer = LayerIgnoreRaycast;

        foreach (Transform item in gameObject.GetComponentInChildren<Transform>())
        {
            item.gameObject.layer = LayerIgnoreRaycast;
        }
        playerMovement.SetHolding(true);
    }
    private void LayerChangeIDraggable()
    {
        int LayerIgnoreRaycast = LayerMask.NameToLayer(Enums.HIT_OBJECT_Layers.Draggable.ToString());
        gameObject.layer = LayerIgnoreRaycast;

        foreach (Transform item in gameObject.GetComponentInChildren<Transform>())
        {
            item.gameObject.layer = LayerIgnoreRaycast;
        }
        playerMovement.SetHolding(false);
    }
}
