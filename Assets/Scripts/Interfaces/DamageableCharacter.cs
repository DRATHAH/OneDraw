using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Progress;

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
    public List<GameObject> loot;
    public List<float> lootChance;
    public Dictionary<GameObject, float> lootMap;

    Rigidbody rb;
    bool targetable = true;

    public UnityEvent OnDestroyEvents;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = maxHealth;

        for (int i = 0; i < loot.Count; i++)
        {
            lootMap.Add(loot[i], lootChance[i]);
        }
    }

    public void OnHit(int damage, Vector3 knockback, GameObject hit)
    {
        // We can add damage modifiers based off of the 'hit' GameObject using tags

        Health -= damage;
        if (rb)
        {
            rb.AddForce(knockback, ForceMode.Impulse);
        }
    }

    public void RemoveCharacter()
    {
        OnDestroyEvents.Invoke();
        if (lootMap != null)
        {
            foreach (GameObject loot in lootMap.Keys)
            {
                float randChance = Random.Range(0f, 100f);
                if (lootMap[loot] >= randChance && loot)
                {
                    Instantiate(loot);
                }
            }
        }

        Destroy(gameObject);

    }
}
