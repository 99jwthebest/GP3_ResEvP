using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("Zombie Attributes")]
    public float health = 200;
    [SerializeField]
    private int scoreFromZombie;
    public float speed = 1.5f;

   // private GameObject player;

    [Header("Zombie AI Settings")]
    public GameObject[] waypoints;
    [SerializeField] private Transform targetPos;

    private NavMeshAgent enemy_NavMeshAgent;
    private Animator enemy_Animator;
    private int waypointIndex;
    private Vector3 prevPosition;
    public float curSpeed;
    public float followRange = 10f;

    public float attackRange = 3f;
    public Transform attackPoint_Transform;
    public float attackPoint_Range = 2f;
    public bool canAttack;

    private void Awake()
    {
        //player = GameObject.Find("Player");
    }

    void Start()
    {
        GameObject playerHolder = GameObject.FindGameObjectWithTag("Player");
        if (playerHolder != null)
        {
            targetPos = playerHolder.transform;
        }
        else
        {
            Debug.Log("Player not found!");
        }
        prevPosition = transform.position;

        waypointIndex = 0;

        enemy_NavMeshAgent = GetComponent<NavMeshAgent>();
        enemy_Animator = GetComponent<Animator>();
        canAttack = true;
    }

    void Update()
    {
        //transform.LookAt(player.gameObject.transform);
        //transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Setting Current Speed for ANIMs
        Vector3 curMove = transform.position - prevPosition;
        curSpeed = curMove.magnitude / Time.deltaTime;
        prevPosition = transform.position;
        enemy_Animator.SetFloat("Speed", curSpeed);

        // Player Detection
        float targetDistance = Vector3.Distance(transform.position, targetPos.position);
        if(targetDistance <= followRange)
        {
            enemy_NavMeshAgent.destination = targetPos.position;

            if(targetDistance <= attackRange && canAttack == true)
            {
                // Attack player
                canAttack = false;
                //enemy_Animator.SetInteger("AttackIndex", Random.Range(0, 7));
                enemy_Animator.SetTrigger("Attack");
            }
        }
        else
        {
            // Waypoints
            float waypointDistance = Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position);
            enemy_NavMeshAgent.destination = waypoints[waypointIndex].transform.position;
            if (waypointDistance < enemy_NavMeshAgent.stoppingDistance + 0.5f)
            {
                waypointIndex++;
                if (waypointIndex >= waypoints.Length)
                {
                    waypointIndex = 0;
                }
            }
        }

        //bool shouldChasePlayer = false;

        //float targetDistance = Vector3.Distance(transform.position, targetPos.position);
        //if (shouldChasePlayer)
        //{
        //    enemy_NavMeshAgent.destination = targetPos.position;

        //    if (targetDistance <= attackRange && canAttack == true)
        //    {
        //        // Attack player
        //        canAttack = false;
        //        //enemy_Animator.SetInteger("AttackIndex", Random.Range(0, 7));
        //        enemy_Animator.SetTrigger("Attack");
        //    }
        //}
        //else
        //{
        //    if (targetDistance <= followRange)
        //    {
        //        enemy_NavMeshAgent.destination = targetPos.position;

        //        if (targetDistance <= attackRange && canAttack == true)
        //        {
        //            // Attack player
        //            canAttack = false;
        //            //enemy_Animator.SetInteger("AttackIndex", Random.Range(0, 7));
        //            enemy_Animator.SetTrigger("Attack");
        //        }
        //    }
        //    else
        //    {
        //        // Waypoints
        //        float waypointDistance = Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position);
        //        enemy_NavMeshAgent.destination = waypoints[waypointIndex].transform.position;
        //        if (waypointDistance < enemy_NavMeshAgent.stoppingDistance + 0.5f)
        //        {
        //            waypointIndex++;
        //            if (waypointIndex >= waypoints.Length)
        //            {
        //                waypointIndex = 0;
        //            }
        //        }
        //    }
        //}
    }



    public void EnemyDie(GameObject enemyObject)
    {
        ScoreSystem.instance.AddScore(scoreFromZombie);

        
        GameManager.KillZombie(enemyObject);
    }

    public void enemyTakeDamage(float value)
    {
        health -= value;
        if (health <= 0)
        {
            EnemyDie(this.gameObject);
        }
    }

    public void ZombieAttack()
    {
        Debug.Log("Event Fired, i guess");
        Collider[] hitTargets = Physics.OverlapSphere(attackPoint_Transform.position, attackPoint_Range);
        foreach (var target in hitTargets)
        {
            if (target.CompareTag("Player"))
            {
                //Add Damage
                Debug.Log("The punch actualllyy worked!!!");
            }
        }
    }

    public void BaseSlash()
    {
        Debug.Log("Event Fired b");
        Collider[] hitTargets = Physics.OverlapSphere(attackPoint_Transform.transform.position, attackPoint_Range);
        foreach (var target in hitTargets)
        {
            if (target.CompareTag("Player"))
            {
                //DamPlayer();
                Debug.Log("Based!!!");
            }
        }
    }

    public void AttackReset()
    {
        canAttack = true;
        enemy_Animator.ResetTrigger("Attack");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackPoint_Transform.position, attackPoint_Range);
    }
}
