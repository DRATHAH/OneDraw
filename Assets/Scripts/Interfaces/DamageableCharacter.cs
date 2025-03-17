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

            if (health > maxHealth)
            {
                health = maxHealth;
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
    [Tooltip("Add a new entry for EACH loot drop. X/100 chance for item to drop.")]
    public List<float> lootChance = new List<float>();
    public Dictionary<List<GameObject>, float> lootMap = new Dictionary<List<GameObject>, float>();
    [Tooltip("Force that dropped items 'pop' out from object")]
    public float lootDropForce = 1;
    [Tooltip("Randomization of where loot spawns on object")]
    public float lootSpawnOffset = .1f;

    [HideInInspector] public Rigidbody rb;
    bool targetable = true;

    public UnityEvent OnDestroyEvents;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = maxHealth;

        List<GameObject> sameItemList = new List<GameObject>();
        for (int i = 0; i < loot.Count; i++)
        {
            if (i > 0 && loot[i] != loot[i - 1])
            {
                lootMap.Add(sameItemList, lootChance[i-1]);
                sameItemList = new List<GameObject>();
            }
            sameItemList.Add(loot[i]);
        }
        lootMap.Add(sameItemList, lootChance[lootChance.Count-1]);
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
        foreach (List<GameObject> lootList in lootMap.Keys)
        {
            foreach(GameObject loot in lootList)
            {
                float randChance = Random.Range(0f, 100f);
                if (lootMap[lootList] >= randChance && loot)
                {
                    float RandX = Random.Range(-lootSpawnOffset, lootSpawnOffset);
                    float RandY = Random.Range(-lootSpawnOffset, lootSpawnOffset);
                    float RandZ = Random.Range(-lootSpawnOffset, lootSpawnOffset);
                    Vector3 spawn = new Vector3(transform.position.x + RandX, transform.position.y + RandY, transform.position.z + RandZ);
                    GameObject lootObj = Instantiate(loot, spawn, Quaternion.identity);

                    lootObj.GetComponent<Rigidbody>().AddExplosionForce(lootDropForce, transform.position - transform.up, 5);
                }
            }
        }


        Destroy(gameObject);

    }
}
