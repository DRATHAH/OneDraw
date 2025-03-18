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
    float baseMultiplier;

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

    public virtual void OnHit(int damage, Vector3 knockback, GameObject hit)
    {
        // We can add damage modifiers based off of the 'hit' GameObject using tags

        Health -= (int)(damage * damageMultiplier + 0.5f);
        if (rb)
        {
            rb.AddForce(knockback, ForceMode.Impulse);
        }
    }

    public virtual void OnHitDot(Hazard type)
    {
        Debug.Log(debuffs.ContainsKey(type) + ", " + type.type);
        if (debuffs.ContainsKey(type) && debuffs[type] >= type.stacks)
        {
            timePair[type] = type.subjectDuration; // If debuff is stronger or the same, only refresh the timer
        }
        else if (debuffs.ContainsKey(type) && debuffs[type] < type.stacks) // New debuff is stronger than previous debuff
        {
            // Remove weaker debuff
            debuffs.Remove(type);
            timePair.Remove(type);

            // Apply more powerful debuff
            timePair.Add(type, type.lifeTime);
            debuffs.Add(type, type.stacks);
        }
        else // Add a new debuff
        {
            if (type.subjectDuration == 0) // Just proc initial effect if it's an instant effect
            {
                OnHit(type.damage, Vector3.zero, gameObject);
                Debug.Log("debuffed once by " + type.type);
            }
            else // Add timer for over-time effects
            {
                timePair.Add(type, type.lifeTime);
                debuffs.Add(type, type.stacks);
            }
        }
    }

    public virtual void Update()
    {
        foreach (Hazard negative in debuffs.Keys)
        {
            if (timePair[negative] > 0)
            {
                timePair[negative] -= Time.deltaTime; // Duration of debuff goes down based on time
                if (negative.timeSinceTick <= 0 && negative.overTime) // Check to see if tick has gone off according to the Hazard gameObject
                {
                    OnHit(negative.damage, Vector3.zero, gameObject);
                    damageMultiplier = negative.damageVulnerability;

                    Debug.Log("Debuffed by " + negative.type);
                }
            }
            else
            {
                // Remove debuff from object
                debuffs.Remove(negative);
                timePair.Remove(negative);

                damageMultiplier = baseMultiplier; // Reset affected stats back to normal
            }
        }
    }

    

    public virtual void Heal(int health)
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

        foreach(AudioSource sound in lootSounds)
        {
            GameObject _sound = Instantiate(soundParticle, transform.position, Quaternion.identity);
            SoundObject soundObj = _sound.GetComponent<SoundObject>();
            soundObj.Initialize(sound);
        }

        Destroy(gameObject);

    }
}
