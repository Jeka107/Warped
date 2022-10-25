using UnityEngine;
using System;

public class BlackHole : MonoBehaviour
{
    private enum IGNORETAG { Player,PlayerRadius, BlackHole,Ground,PlayerWeapon, AttackCollectable } //for ignoring tag

    [SerializeField] private float GRAVITY_PULL = .78f;
    [SerializeField] private float m_GravityRadius = 1f;
    [SerializeField] private float throwPower;
    [SerializeField] private float suckTime;
    [SerializeField] private float stopTime;
    private Rigidbody rb;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        FindObjectOfType<PlayerActions>()?.SetKeyCode(KeyCode.R);
        transform.position = Camera.main.transform.localPosition + Camera.main.transform.forward * 3 + new Vector3(0, 1f, 0);
        m_GravityRadius = GetComponent<SphereCollider>().radius;

        rb.velocity = Camera.main.transform.forward * throwPower * Time.deltaTime ;

        GetComponent<AudioSource>().Play();

        Invoke("StopBlckHole", stopTime);//stop black hole in middle air
    }

    /// <summary>
    /// Attract objects towards an area when they come within the bounds of a collider.
    /// This function is on the physics timer so it won't necessarily run every frame.
    /// </summary>
    /// <param name="other">Any object within reach of gravity's collider</param>
    void OnTriggerStay(Collider other)
    {
        if (rb.velocity == Vector3.zero)
        {
            if (other.attachedRigidbody && !Enum.IsDefined(typeof(IGNORETAG), other.tag))//black holes dont suck each other
            {
                other.GetComponent<Rigidbody>().isKinematic = false;
                float gravityIntensity = Vector3.Distance(transform.position, other.transform.position) / m_GravityRadius;
                other.attachedRigidbody?.AddForce((transform.position - other.transform.position) * gravityIntensity * other.attachedRigidbody.mass * GRAVITY_PULL * Time.smoothDeltaTime);
                Debug.DrawRay(other.transform.position, transform.position - other.transform.position);
            }
        }
    }

    private void OnCollisionEnter(Collision collision) //stop black hole on hit object
    {
        rb.constraints= RigidbodyConstraints.FreezePosition;
        Destroy(gameObject, suckTime);
    }

    private void StopBlckHole() //stop black hole in middle air
    {
        rb.constraints = RigidbodyConstraints.FreezePosition;
        Destroy(gameObject, suckTime);
    }
}
