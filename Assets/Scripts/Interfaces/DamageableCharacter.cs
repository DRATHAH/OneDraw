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
    public List<GameObject> loot = new List<GameObject>();
    public List<float> lootChance = new List<float>();
    public Dictionary<GameObject, float> lootMap = new Dictionary<GameObject, float>();
    [Tooltip("Force that dropped items 'pop' out from object")]
    public float lootDropForce = 1;
    [Tooltip("Randomization of where loot spawns on object")]
    public float lootSpawnOffset = .1f;

    private Rigidbody rb;
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

    public void Heal(int health)
    {
        Health += health;
    }

    public void RemoveCharacter()
    {
        OnDestroyEvents.Invoke();
        foreach (GameObject loot in lootMap.Keys)
        {
            float randChance = Random.Range(0f, 100f);
            if (lootMap[loot] >= randChance && loot)
            {
                float RandX = Random.Range(-lootSpawnOffset, lootSpawnOffset);
                float RandY = Random.Range(-lootSpawnOffset, lootSpawnOffset);
                float RandZ = Random.Range(-lootSpawnOffset, lootSpawnOffset);
                Vector3 spawn = new Vector3(transform.position.x + RandX, transform.position.y + RandY, transform.position.z + RandZ);
                GameObject lootObj = Instantiate(loot, spawn, Quaternion.identity);

                lootObj.GetComponent<Rigidbody>().AddExplosionForce(lootDropForce, transform.position - transform.up, 5);
            }
        }


        Destroy(gameObject);

    }
}
