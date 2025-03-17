using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.FilePathAttribute;

public class WaitingEnemy : DamageableCharacter
{
    public int dmg = 1;
    public float moveSpeed = 10f;

    private NavMeshAgent agent;
    private bool chasingPlayer;
    private Transform player;
    private Vector3 startLocation;
    private Rigidbody body;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        player = GameObject.FindWithTag("Player").transform;
        body = GetComponent<Rigidbody>();
        startLocation = transform.position;
    }

    private void Update()
    {
        // Chase
        if (chasingPlayer)
        {
            agent.SetDestination(player.position);
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
            agent.SetDestination(startLocation);
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
                damageable.OnHit(dmg, body.velocity, hit);
                //Debug.Log("Hit Player");
            }
        }
    }
}
