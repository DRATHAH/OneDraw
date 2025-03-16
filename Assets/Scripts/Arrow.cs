using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int dmg = 1;
    public float knockback = 0;
    public Vector3 initialForce;
    public Rigidbody rb;
    public Transform lead;

    BoxCollider arrowCol;
    bool canHit = true;

    // Start is called before the first frame update
    void Start()
    {
        arrowCol = GetComponent<BoxCollider>();
        rb.AddForce(initialForce);
    }

    private void Update()
    {
        if (canHit)
        {
            Vector3 direction = ((transform.position + rb.velocity) - lead.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.transform.root.GetComponent<PlayerShoot>().hasArrow = true;
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
            canHit = false;
            GameObject hit = collision.gameObject;
            damageable.OnHit(dmg, rb.velocity * knockback, hit);
            Debug.Log("Arrow did " + dmg + " damage to " + hit.name);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            canHit = false;
            arrowCol.isTrigger = true;
            rb.freezeRotation = true;
            rb.isKinematic = true;
        }
    }

    public void Initialize(Vector3 origin, float shootForce, int damage, float knockbackForce)
    {
        initialForce = origin * shootForce;
        dmg = damage;
        knockback = knockbackForce;
    }
}
