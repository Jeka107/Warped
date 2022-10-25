using UnityEngine;

public class AttackingEnemy : MonoBehaviour
{
    [SerializeField] private int damage = 30;
    [SerializeField] private AudioSource HittingPlayerSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<PlayerStats>()?.ReduceHp(damage);
            Debug.Log("hit");
            HittingPlayerSound?.Play();
        }
    }
}
