using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Arrow : MonoBehaviour
{
    public int dmg = 1;
    public float knockback = 0;
    public int frostStacks = 0;
    public int fireStacks = 0;
    public int lightningStacks = 0;
    public Vector3 initialForce;
    public Rigidbody rb;
    public Transform lead;
    public UnityEvent hitEvent;

    [Header("Particles")]
    [SerializeField] GameObject frostParticles;
    [SerializeField] GameObject fireParticles;
    [SerializeField] GameObject lightningParticles;

    BoxCollider arrowCol;
    bool canHit = true;
    int maxDmg;
    Vector3 maxForceVel;
    float speedRef = 0; // used to calculate adaptive damage
    float mag;

    // Start is called before the first frame update
    void Start()
    {
        frostParticles.SetActive(false);
        fireParticles.SetActive(false);
        lightningParticles.SetActive(false);

        arrowCol = GetComponent<BoxCollider>();
        rb.AddForce(initialForce);

        if (frostStacks > 0)
        {
            frostParticles.SetActive(true);
        }
        if (fireStacks > 0)
        {
            fireParticles.SetActive(true);
        }
        if (lightningStacks > 0)
        {
            lightningParticles.SetActive(true);
        }
    }

    private void Update()
    {
        if (speedRef == 0 && rb.velocity.magnitude != 0)
        {
            speedRef = rb.velocity.magnitude;
        }

        if (canHit)
        {
            // Makes arrow face direction it is moving
            Vector3 direction = ((transform.position + transform.forward * 0.5f + rb.velocity) - lead.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5);

            // Calculates damage based on speed of arrow
            if (rb.velocity.magnitude > 0)
            {
                mag = rb.velocity.magnitude;
                float dmgCalc = (rb.velocity.magnitude / (maxForceVel.magnitude / (rb.mass) * Time.fixedDeltaTime)) * maxDmg + 0.5f;
                dmg = (int)dmgCalc;
                if (dmg > maxDmg)
                {
                    dmg = maxDmg;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && other.transform.GetComponent<PlayerMovement>())
        {
            other.transform.root.GetComponent<PlayerShoot>().hasArrow = true;
            other.transform.root.GetComponent<PlayerShoot>().PlayPickupSound();
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
        {
            canHit = true;
            arrowCol.isTrigger = false;
            rb.isKinematic = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        DamageableCharacter damageable = collision.transform.root.GetComponent<DamageableCharacter>();
        if (damageable && canHit && collision.transform.root.gameObject.layer != LayerMask.NameToLayer("Player") && damageable.Targetable)
        {
            // Freeze arrow once it hits a destructible
            canHit = false;
            rb.velocity = Vector3.zero;
            arrowCol.isTrigger = true;
            rb.isKinematic = true;

            GameObject hit = collision.gameObject;
            damageable.OnHit(dmg, rb.velocity * knockback, hit);
            Debug.Log("Arrow did " + dmg + " damage to " + hit.name);

            if (!damageable || damageable.health <= 0)
            {
                canHit = true;
                arrowCol.isTrigger = false;
                rb.isKinematic = false;
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            // Freeze arrow once it hits a wall
            canHit = false;
            rb.velocity = Vector3.zero;
            arrowCol.isTrigger = true;
            rb.isKinematic = true;
        }

        if (frostStacks > 0)
        {

        }
    }

    public void Initialize(Vector3 origin, float shootForce, float maxForce, int maxDamage, float knockbackForce, int frost, int fire, int lightning)
    {
        initialForce = origin * shootForce;
        maxForceVel = origin * maxForce;
        maxDmg = maxDamage;
        knockback = knockbackForce;
        frostStacks = frost;
        fireStacks = fire;
        lightningStacks = lightning;
    }
}
