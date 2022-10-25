using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingPlayer : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private AudioSource alienBeingHitSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyAI>()?.TakeDamage(damage);
            alienBeingHitSound.Play();
        }
    }
}
