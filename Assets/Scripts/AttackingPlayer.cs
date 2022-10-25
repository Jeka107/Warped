using UnityEngine;

public class AttackingPlayer : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private AudioSource alienBeingHitSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))//on collide with enemy,hit enemy and reduce enemy's HP.
        {
            other.GetComponent<EnemyAI>()?.TakeDamage(damage);
            alienBeingHitSound.Play();
        }
    }
}
