using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public bool canMove = true;
    public int maxHealth = 10;
    public int health = 10;
    public bool isPlayer = false;
    [Tooltip("Multiplies any damage taken by this amount.")]
    public float damageMultiplier = 1;
    public List<GameObject> loot = new List<GameObject>();
    [Tooltip("Add a new entry for EACH loot drop. X/100 chance for item to drop.")]
    public List<float> lootChance = new List<float>();
    public Dictionary<List<GameObject>, float> lootMap = new Dictionary<List<GameObject>, float>();
    [Tooltip("Force that dropped items 'pop' out from object")]
    public float lootDropForce = 1;
    [Tooltip("Randomization of where loot spawns on object")]
    public float lootSpawnOffset = .1f;
    [Tooltip("Sound(s) entity makes upon being destroyed.")]
    public List<AudioSource> lootSounds;
    [Tooltip("Sound particle in prefabs folder.")]
    public GameObject soundParticle;

    // Base stats that are updated once game starts
    float baseMultiplier = 1;

    bool modifyingDebuffs = false;

    [HideInInspector] public Rigidbody rb;
    bool targetable = true;
    public Dictionary<Hazard, int> debuffs = new Dictionary<Hazard, int>(); // Types of debuffs applied to object
    public Dictionary<Hazard, float> timePair = new Dictionary<Hazard, float>(); // How long those debuffs will last for

    public UnityEvent OnDestroyEvents;

    // Start is called before the first frame update
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = maxHealth;
        baseMultiplier = damageMultiplier;

        List<GameObject> sameItemList = new List<GameObject>(); // List of repeat items the character drops upon death
        for (int i = 0; i < loot.Count; i++)
        {
            if (i > 0 && loot[i] != loot[i - 1]) // If moved to a new item
            {
                lootMap.Add(sameItemList, lootChance[i-1]); // Add the list of same items and their drop chance to lootMap
                sameItemList = new List<GameObject>(); // Clear sameItemList
            }
            sameItemList.Add(loot[i]);
        }
        if (loot.Count > 0) // If character only drops one item, make sure to include that first item
        {
            lootMap.Add(sameItemList, lootChance[lootChance.Count - 1]);
        }
    }

    public virtual void OnHit(int damage, Vector3 knockback, GameObject hit)
    {
        // We can add damage modifiers based off of the 'hit' GameObject using tags

        Health -= (int)(damage * damageMultiplier + 0.5f);
        if (rb)
        {
            rb.AddForce(knockback, ForceMode.Impulse);
        }
    }

    public virtual void ApplyDebuff(Hazard type)
    {
        if (debuffs.Count > 0) // If it's not the first debuff this character gets
        {
            Dictionary<Hazard, int> tempStacks = new Dictionary<Hazard, int>(); // Dictionary of new debuffs to add and their stack #
            List<Hazard> toRemove = new List<Hazard>(); // List of debuffs to remove

            foreach (Hazard hazard in debuffs.Keys)
            {
                if (hazard.type == type.type && debuffs[hazard] >= type.stacks) // If debuff is same type and stronger or the same, only refresh the timer
                {
                    timePair[hazard] = hazard.subjectDuration;
                    Debug.Log("Refreshed " + type.type);
                }
                else if (debuffs[hazard] < type.stacks && hazard.type == type.type) // If new debuff is the same but stronger than previous debuff
                {
                    // Remove weaker debuff
                    toRemove.Add(hazard);

                    // Apply more powerful debuff
                    tempStacks.Add(type, type.stacks);
                    Debug.Log("Superceded " + type.type);
                }
                else // Add a brand new debuff
                {
                    tempStacks.Add(type, type.stacks);
                    Debug.Log("Added new  " + type.type);
                }
            }

            foreach (Hazard removal in toRemove)
            {
                timePair.Remove(removal);
                debuffs.Remove(removal);
            }

            foreach (Hazard toAdd in tempStacks.Keys)
            {
                debuffs.Add(toAdd, tempStacks[toAdd]);
                timePair.Add(toAdd, toAdd.subjectDuration);
            }
        }
        else // Apply first debuff to character
        {
            timePair.Add(type, type.subjectDuration);
            debuffs.Add(type, type.stacks);
            Debug.Log("Added first " + type.type);
        }
    }

    public void DebuffHit(Hazard type) // Function to be called when a debuff tick is applied
    {
        OnHit(type.damage, Vector3.zero, gameObject); // Applies damage if any
        damageMultiplier = type.damageVulnerability; // Makes character more vulnerable if programmed
        if (type.stuns) // Stuns character if programmed
        {
            StartCoroutine(StunRecover(type.stunDuration));
        }

        Debug.Log(gameObject.name + " debuffed over time by " + type.type + ", " + Mathf.Round(timePair[type] * 100) / 100);
    }

    public virtual void RemoveDebuff(Hazard type)
    {
        List<Hazard> toRemove = new List<Hazard>(); // List of debuffs to be removed

        if (debuffs.ContainsKey(type)) // If debuff is currently affecting character
        {
            Debug.Log(type.type + " removed");

            toRemove.Add(type);

            damageMultiplier = baseMultiplier; // Reset affected stats back to normal
        }

        // Remove debuff from object
        foreach (Hazard removal in toRemove)
        {
            timePair.Remove(removal);
            debuffs.Remove(removal);
        }
    }

    public virtual void Update()
    {
        foreach (Hazard negative in debuffs.Keys)
        {
            if (timePair[negative] >= 0)
            {
                timePair[negative] -= Time.deltaTime; // Duration of debuff goes down based on time
                
            }
        }
    }

    public virtual void Heal(int health)
    {
        Health += health;
    }

    public virtual void RemoveCharacter()
    {
        OnDestroyEvents.Invoke(); // Invoke any special events when destroyed

        foreach (List<GameObject> lootList in lootMap.Keys) // For each unique item in loot drops
        {
            foreach(GameObject loot in lootList) // Repeat drops for any repeated items
            {
                float randChance = Random.Range(0f, 100f);
                if (lootMap[lootList] >= randChance && loot)
                {
                    // Makes loot pop away at different angles
                    float RandX = Random.Range(-lootSpawnOffset, lootSpawnOffset);
                    float RandY = Random.Range(-lootSpawnOffset, lootSpawnOffset);
                    float RandZ = Random.Range(-lootSpawnOffset, lootSpawnOffset);
                    Vector3 spawn = new Vector3(transform.position.x + RandX, transform.position.y + RandY, transform.position.z + RandZ);
                    GameObject lootObj = Instantiate(loot, spawn, Quaternion.identity); // Spawns loot

                    lootObj.GetComponent<Rigidbody>().AddExplosionForce(lootDropForce, transform.position - transform.up, 5); // Applies 'pop' force
                }
            }
        }

        foreach(AudioSource sound in lootSounds) // Plays destruction sound when destroyed
        {
            GameObject _sound = Instantiate(soundParticle, transform.position, Quaternion.identity);
            SoundObject soundObj = _sound.GetComponent<SoundObject>();
            soundObj.Initialize(sound);
        }

        Destroy(gameObject);
    }

    IEnumerator StunRecover(float time)
    {
        canMove = false;
        if (rb)
        {
            rb.velocity = Vector3.zero;
        }
        yield return new WaitForSeconds(time);
        canMove = true;
    }
}
