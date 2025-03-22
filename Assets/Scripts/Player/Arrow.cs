using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Arrow : MonoBehaviour
{
    public int dmg = 1;
    public float knockback = 0;
    public int frostStacks = 0;
    public int fireStacks = 0;
    public int lightningStacks = 0;
    public float turnTime = 10f;
    public Vector3 initialForce;
    public Rigidbody rb;
    public Transform lead;
    [Tooltip("Layers that the arrow can collide with.")]
    public LayerMask collisionLayers;
    public UnityEvent hitEvent;

    [Header("Particles")]
    [SerializeField] List<GameObject> particles = new List<GameObject>();
    [SerializeField] GameObject hazardPrefab;
    public List<HazardStats> hazards = new List<HazardStats>();

    BoxCollider arrowCol;
    bool canHit = true;
    int maxDmg;
    Vector3 maxForceVel;
    float speedRef = 0; // used to calculate adaptive damage
    float mag;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject p in particles)
        {
            p.SetActive(false);
        }

        arrowCol = GetComponent<BoxCollider>();
        rb.AddForce(initialForce);

        foreach(GameObject graphic in particles)
        {
            foreach(HazardStats haz in hazards)
            {
                if (haz.stacks > 0 && graphic.name.ToLower().Contains(haz.type.ToString()))
                {
                    graphic.SetActive(true);
                }
            }
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
            if (direction.magnitude > 0)
            {
                Quaternion lookRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnTime);
            }

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

        RaycastHit[] hit = Physics.BoxCastAll(transform.position, arrowCol.size/2, transform.forward, transform.rotation, 1, collisionLayers, QueryTriggerInteraction.Ignore);
        if (hit.Length == 0 && !canHit)
        {
            canHit = true;
            arrowCol.isTrigger = false;
            rb.isKinematic = false;
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

    private void OnCollisionEnter(Collision collision)
    {
        DamageableCharacter damageable = collision.transform.GetComponentInParent<DamageableCharacter>();
        if (damageable && canHit && collision.transform.root.gameObject.layer != LayerMask.NameToLayer("Player") && damageable.targetable)
        {
            // Freeze arrow once it hits a destructible
            canHit = false;
            rb.velocity = Vector3.zero;
            arrowCol.isTrigger = true;
            rb.isKinematic = true;

            GameObject hit = collision.gameObject;
            damageable.OnHit(dmg, rb.velocity * knockback, hit);
            Debug.Log("Arrow did " + dmg + " damage to " + hit.name);

            foreach (HazardStats stat in hazards)
            {
                if (stat.stacks > 0)
                {
                    GameObject zone = Instantiate(hazardPrefab, transform.position, Quaternion.identity);
                    zone.GetComponent<Hazard>().Initialize(stat);
                }
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            // Freeze arrow once it hits a wall
            canHit = false;
            rb.velocity = Vector3.zero;
            arrowCol.isTrigger = true;
            rb.isKinematic = true;

            foreach (HazardStats stat in hazards)
            {
                if (stat.stacks > 0)
                {
                    GameObject zone = Instantiate(hazardPrefab, transform.position, Quaternion.identity);
                    zone.GetComponent<Hazard>().Initialize(stat);
                }
            }
        }
    }

    public void Initialize(Vector3 origin, float shootForce, float maxForce, int maxDamage, float knockbackForce, List<HazardStats> upgrades)
    {
        initialForce = origin * shootForce;
        maxForceVel = origin * maxForce;
        maxDmg = maxDamage;
        knockback = knockbackForce;

        foreach (HazardStats haz in upgrades)
        {
            hazards.Add(haz);
        }
    }
}
