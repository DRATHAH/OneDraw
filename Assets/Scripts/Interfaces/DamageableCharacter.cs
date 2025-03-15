using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableCharacter : MonoBehaviour, IDamageable
{
    public int Health
    {
        set
        {
            health = value;

            if (value > 0)
            {
                // Hit animation
            }

            if (health <= 0 && Targetable)
            {
                Targetable = false;
                health = 0;
                if (!isPlayer)
                {
                    RemoveCharacter();
                }
            }
        }
        get
        {
            return health;
        }
    }
    public bool Targetable {
        get { return targetable; }
        set
        {
            targetable = value;
        }
    }


    public int maxHealth = 10;
    public int health = 10;
    public bool isPlayer = false;

    Rigidbody rb;
    bool targetable = true;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = maxHealth;
    }

    public void OnHit(int damage, Vector3 knockback, GameObject hit)
    {
        // We can add damage modifiers based off of the 'hit' GameObject using tags

        Health -= damage;
        rb.AddForce(knockback, ForceMode.Impulse);
    }

    public void RemoveCharacter()
    {
        Destroy(gameObject);
    }
}
