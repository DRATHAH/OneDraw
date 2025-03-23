using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemy : DamageableCharacter
{
    [Tooltip("Transform of the parent object containing waypoints")]
    public Transform patrolRoute;

    private Transform[] locations;
    private int currentLocation = 0;
    private bool chasingPlayer;

    [Tooltip("Layer of the player and anything else enemies should attack.")]
    public LayerMask targetLayer;
    [Tooltip("Layer enemies are on.")]
    public LayerMask enemyLayer;

    [Header("Stats")]
    public int dmg = 1;
    [Tooltip("How long, in seconds, the enemy waits before attacking again.")]
    public float attackRate;
    public float moveSpeed = 10f;
    public float turnSpeed = 10f;
    [Tooltip("Range at which the enemy will start chasing targets.")]
    public float sightRange = 5f;
    [Tooltip("Whether the enemy needs to face the target to see it.")]
    public bool requireFacing = false;
    [Tooltip("Range at which the enemy will stop chasing and attack targets.")]
    public float attackRange = 2f;
    [Tooltip("AudioSource component that most sound will be played through.")]
    public AudioSource enemyAudio;
    public AudioClip attackSound;
    public List<AudioClip> idleSounds = new List<AudioClip>();
    [Tooltip("How much time will pass before an idle sound plays.")]
    public float idleTime = 10f;

    Animator animator;
    Transform target;
    GameObject attackParticles;
    float timeSinceIdled = 0;
    bool targetInAttack, targetInSight = false;
    bool canAttack = true;
    private NavMeshAgent agent;
    private Vector3 startLocation;

    public override void Start()
    {
        base.Start();

        attackParticles = GetComponentInChildren<ParticleSystem>().gameObject;
        attackParticles.SetActive(false);
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.acceleration = moveSpeed * 2;
        startLocation = transform.position;
        animator = GetComponent<Animator>();

        InitializePatrolRoute();
        MoveToNextPatrolLocation();
    }

    public override void Update()
    {
        base.Update();

        if (canMove)
        {
            // Get a list of all things the enemy should be hostile towards within range

            RaycastHit[] hit = Physics.SphereCastAll(transform.position, sightRange, Vector3.up, 0, targetLayer); // Get the list of objects
            foreach (RaycastHit ray in hit)
            {
                DamageableCharacter dChar = ray.transform.root.GetComponent<DamageableCharacter>();
                if (dChar && dChar.targetable) // Make sure it can attack them
                {
                    float tempDistance = Vector3.Distance(dChar.transform.position, transform.position);
                    if ((target && Vector3.Distance(target.position, transform.position) > tempDistance) || !target) // If this target is closer than the previous target
                    {
                        target = dChar.transform;
                    }
                }
            }

            if (target)
            {
                float distance = Vector3.Distance(target.position, transform.position);
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Vector3 direction = (target.position - transform.position).normalized;
                Ray directRay = new Ray(transform.position, direction);
                RaycastHit obstacle;
                if (Physics.Raycast(directRay, out obstacle, sightRange, ~enemyLayer)) // Make sure enemy has direct line of sight with target, ignoring other enemies
                {
                    DamageableCharacter dChar = obstacle.collider.transform.root.GetComponent<DamageableCharacter>();
                    if (dChar == target.GetComponent<DamageableCharacter>() && (Vector3.Dot(forward, direction) > 0.5f || !requireFacing)) // Make sure enemy is facing target
                    {
                        if (distance <= attackRange)
                        {
                            targetInAttack = true;
                        }
                        else
                        {
                            targetInAttack = false;
                        }

                        if (distance <= sightRange)
                        {
                            targetInSight = true;
                        }
                        else
                        {
                            targetInSight = false;
                        }
                    }
                }

                if (targetInSight && targetInAttack && target.GetComponent<DamageableCharacter>().targetable)
                {
                    Attack();
                }
                else if (targetInSight && !targetInAttack && target.GetComponent<DamageableCharacter>().targetable)
                {
                    ChaseTarget();
                    chasingPlayer = true;
                }
                else
                {
                    chasingPlayer = false;
                    agent.SetDestination(startLocation);
                }
            }

            if (!chasingPlayer && !agent.pathPending && agent.remainingDistance < 0.2f)
            {
                MoveToNextPatrolLocation();
            }

            // Idle sound code
            if (!targetInAttack && !targetInSight)
            {
                timeSinceIdled += Time.deltaTime;
            }
            else
            {
                timeSinceIdled = 0;
            }

            if (timeSinceIdled >= idleTime)
            {
                timeSinceIdled = 0;
                int random = Random.Range(0, idleSounds.Count);
                enemyAudio.clip = idleSounds[random];
                GameObject sound = Instantiate(soundParticle, transform.position, Quaternion.identity);
                sound.GetComponent<SoundObject>().Initialize(enemyAudio);
            }
        }
    }

    void MoveToNextPatrolLocation()
    {
        if (locations.Length == 0)
        {
            return;
        }
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

    void ChaseTarget()
    {
        FaceTarget(target.position);
        agent.SetDestination(target.position);
    }

    void Attack()
    {
        FaceTarget(target.position);
        agent.SetDestination(transform.position);

        if (canAttack)
        {
            // Animation for attacking
            attackParticles.SetActive(true);
            animator.SetTrigger("Attack");
            AnimatorClipInfo[] attackAnimInfo = animator.GetCurrentAnimatorClipInfo(0);

            enemyAudio.clip = attackSound;
            GameObject sound = Instantiate(soundParticle, transform.position, Quaternion.identity);
            sound.GetComponent<SoundObject>().Initialize(enemyAudio);
            StartCoroutine(ResetAttack(attackAnimInfo.Length));
        }
    }

    public void ToggleAttackCollider() // Called through animation
    {
        if (target)
        {
            float distance = Vector3.Distance(target.position, transform.position);
            if (distance < attackRange)
            {
                target.GetComponent<DamageableCharacter>().OnHit(dmg, Vector3.zero, null);
            }
        }
    }

    IEnumerator ResetAttack(float animationTime)
    {
        // Wait for attackRate delay as well as for the attack animation to finish
        canAttack = false;
        yield return new WaitForSeconds(animationTime);
        attackParticles.SetActive(false);
        yield return new WaitForSeconds(attackRate);
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }
}
