using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemy : DamageableCharacter
{
    public int dmg = 1;
    public float moveSpeed = 10f;
    [Tooltip("Transform of the parent object containing waypoints")]
    public Transform patrolRoute;

    private NavMeshAgent agent;
    private Transform[] locations;
    private int currentLocation = 0;
    private bool chasingPlayer;
    private Transform player;
    private Rigidbody body;

    public override void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        player = GameObject.FindWithTag("Player").transform;
        body = GetComponent<Rigidbody>();

        InitializePatrolRoute();
        MoveToNextPatrolLocation();
    }

    public override void Update()
    {
        // Patrol
        if (!chasingPlayer && !agent.pathPending && agent.remainingDistance < 0.2f)
        {
            MoveToNextPatrolLocation();
        }

        // Chase
        if (chasingPlayer)
        {
            agent.SetDestination(player.position);
        }
    }

    void MoveToNextPatrolLocation()
    {
        if (locations.Length == 0) return;

        agent.SetDestination(locations[currentLocation].position);
        currentLocation = (currentLocation + 1) % locations.Length;
    }

    void InitializePatrolRoute()
    {
        locations = new Transform[patrolRoute.childCount];
        for (int i = 0; i < patrolRoute.childCount; i++)
        {
            locations[i] = patrolRoute.GetChild(i);
        }
    }

    // Detect when player enters enemy's range, then initiate chase
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player detected - start chasing!");
            chasingPlayer = true;
            agent.SetDestination(player.position);
        }
    }

    // Detect when player leaves enemy's range; return to patrol
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player out of range - resume patrol.");
            chasingPlayer = false;
            MoveToNextPatrolLocation();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        DamageableCharacter damageable = collision.transform.root.GetComponent<DamageableCharacter>();
        GameObject hit = collision.gameObject;

        if (damageable)
        {
            if (hit.CompareTag("Player"))
            {
                damageable.OnHit(dmg, body.velocity, hit, false);
                //Debug.Log("Hit Player");
            }
        }
    }
}
