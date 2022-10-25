using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float enemyHealth = 50;

    public void ReduceHealth(int damage)
    {
        enemyHealth -= damage;
        if (enemyHealth <= 0)
        {
            Debug.Log(2);
            Destroy(gameObject);  
        }
    }
}
