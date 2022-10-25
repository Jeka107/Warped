using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public delegate void OnKill(Quest quest);
    public static event OnKill onKill;

    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private Transform player;

    //Health
    [Header("Health")]
    [SerializeField] private float health = 50;

    //Check for Ground/Obstacles
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;

    //Patroling
    [Header("Patroling")]
    [SerializeField] private Vector3 walkPoint;
    [SerializeField] private bool walkPointSet;
    [SerializeField] private Transform walkPointsTransform;

    //Attack Player
    [Header("Attack Player")]
    [SerializeField] private float timeBetweenAttacks;
    private bool alreadyAttacked;

    //States
    [Header("States")]
    [SerializeField] private bool isDead;
    [SerializeField] private float sightRange, attackRange;
    [SerializeField] private bool playerInSightRange, playerInAttackRange;

    [Header("Time")]
    [SerializeField] private float minStopTime;
    [SerializeField] private float maxStopTime;

    private List<Transform> walkPoints = new List<Transform>();
    private float timer = 0;
    private Animator animator;
    [SerializeField] private AudioSource idleAlienSound;
    [SerializeField] private AudioSource attackingAlienSound;
    

    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        if (walkPointsTransform != null)
        {
            foreach (Transform t in walkPointsTransform)
            {
                walkPoints.Add(t);
            }
        }
    }
    private void Update()
    {
        if (!isDead)
        {
            //Check if Player in sightrange
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

            //Check if Player in attackrange
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
            

            if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }
    }

    private void Patroling()
    {
        float stopTime = Random.Range(minStopTime, maxStopTime);
        timer += Time.deltaTime;

        if (timer > stopTime)
        {
            if (isDead) return;

            if (!walkPointSet) SearchWalkPoint();

            //Calculate direction and walk to Point
            if (walkPointSet)
            {
                animator.SetBool("isWalking", true);
                agent.SetDestination(walkPoint);

                Vector3 direction = walkPoint - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.15f);
            }

            //Calculates DistanceToWalkPoint
            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            //Walkpoint reached
            if (distanceToWalkPoint.magnitude < 1f)
            {
                timer = 0;
                walkPointSet = false;
            }
        }
        else if(!walkPointSet)
            animator.SetBool("isWalking", false);
    }
    private void SearchWalkPoint()
    {
        if (walkPoints.Count >0)
        {
            int randomNum = Random.Range(0, walkPoints.Count);
            walkPoint = walkPoints[randomNum].position;

            //if (Physics.Raycast(walkPoint, -transform.up, 2, whatIsGround))
                walkPointSet = true;
        }    
    }
    private void ChasePlayer()
    {
        if (isDead) return;

        animator.SetBool("isWalking", true);
        agent.SetDestination(player.position);
    }
    private void AttackPlayer()
    {
        if (isDead) return;

        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            //Attack
            animator.SetBool("isWalking", false);
            animator.SetBool("jawaAttack", true);

            alreadyAttacked = true;
            Invoke("ResetAttack", timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        if (isDead) return;

        alreadyAttacked = false;
    }
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health < 0 && !isDead)
        {
            var questObject = GetComponent<QuestObject>();

            if (questObject != null)
            {
                onKill?.Invoke(questObject.quest);
            }

            isDead = true;
            Destroy(gameObject);
            //Invoke("Destroyy", 2.8f);
        }
    }
    private void Destroyy()
    {
        Destroy(gameObject);
    }
}
