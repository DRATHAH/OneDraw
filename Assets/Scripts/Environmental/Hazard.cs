using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Hazard : MonoBehaviour
{
    [Header("Hazard References")]
    public HazardStats.HazardType type;
    public ParticleSystem graphic;
    public List<ParticleSystem> hazardParticle = new List<ParticleSystem>();

    [Header("Things that effect a DamageableCharacter")]
    [Tooltip("Multipliers for effects.")]
    public int stacks = 1;
    [Tooltip("Whether it disables movement for a short while.")]
    public bool stuns = false;
    [Tooltip("Duration of the stun in seconds.")]
    public float stunDuration = 0;
    [Tooltip("Damage of initial entrance and DoT effects.")]
    public int damage = 0;
    [Tooltip("How much more damage an object will take. Ex: a value of 2 = x2 damage.")]
    public float damageVulnerability = 0;

    [Header("Hazard Stats")]
    [Tooltip("How long the effect lasts on the ground.")]
    public float lifeTime = 0;
    [Tooltip("Size modifier. For arrow upgrades, scales with stacks.")]
    public float size = 0;

    [Tooltip("Whether to have over-time effects or not.")]
    public bool overTime = false;
    [Tooltip("How fast effects like DoT proc.")]
    public float tickRate = 1;
    [Tooltip("How long the effect lasts on a target (can carry over after a target leaves the area).")]
    public float subjectDuration = 0;

    float timeSinceTick = 0;
    float timeSinceTickInside = 0;

    float timeAlive = 0;
    bool deactivated = false;
    [SerializeField] CapsuleCollider col;

    List<DamageableCharacter> characters = new List<DamageableCharacter>(); // List of targets currently affected by hazard's debuff

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale *= size;

        // Checks what particles to set active
        foreach(ParticleSystem particles in hazardParticle)
        {
            if (!particles.gameObject.name.ToLower().Contains(type.ToString()))
            {
                particles.gameObject.SetActive(false);
            }
            else
            {
                graphic = particles;
            }
        }

        col = GetComponent<CapsuleCollider>();

        // Checks to see if it spawned with targets already inside it
        RaycastHit[] hit = Physics.SphereCastAll(transform.TransformPoint(col.center), col.radius * size, Vector3.up, 0);
        foreach(RaycastHit raycastHit in hit)
        {
            DamageableCharacter character = raycastHit.transform.root.GetComponent<DamageableCharacter>();
            if (character && character.Targetable && !characters.Contains(character))
            {
                characters.Add(character);
                character.ApplyDebuff(this); // Apply the debuff
                character.DebuffHit(this);
            }
        }
    }

    // Debuff character walking inside hazard
    private void OnTriggerEnter(Collider other)
    {
        DamageableCharacter character = other.transform.root.GetComponent<DamageableCharacter>();
        if (character && character.Targetable && !characters.Contains(character))
        {
            character.ApplyDebuff(this);
            character.DebuffHit(this);
            characters.Add(character);
        }
        else if (characters.Contains(character) && character.timePair.ContainsKey(this) && character.timePair[this] > 0)
        {
            character.ApplyDebuff(this);
        }
    }

    // If hazard doesn't do over-time effects, remove the debuff from the leaving target
    private void OnTriggerExit(Collider other)
    {
        DamageableCharacter character = other.transform.root.GetComponent<DamageableCharacter>();
        if (character && character.Targetable && !overTime)
        {
            character.RemoveDebuff(this);
            characters.Remove(character);
        }
    }

    public void Initialize(HazardStats baseStats) // Sets stats for the hazard
    {
        type = baseStats.type;

        stacks = baseStats.stacks;
        stuns = baseStats.stuns;
        stunDuration = baseStats.stunDuration;
        damage = baseStats.damage + (int)((stacks - 1) * baseStats.damageScale + .5f);
        damageVulnerability = baseStats.damageVulnerability + ((stacks - 1) * baseStats.vulnerabilityScale);
        lifeTime = baseStats.lifeTime + ((stacks - 1) * baseStats.lifeScale);
        size = baseStats.size + ((stacks - 1) * baseStats.sizeScale);
        tickRate = baseStats.tickRate - ((stacks - 1) * baseStats.tickScale);
        subjectDuration = baseStats.subjectDuration + ((stacks - 1) * baseStats.subjectScale);

        overTime = subjectDuration > 0;


        if (!overTime) // For reapplying debuffs while standing in the hazard
        {
            tickRate = 1;
        }

        if (tickRate <= 0) // Makes sure tick rate doesn't go below 0 and cause weird glitches
        {
            tickRate = 0.05f;
        }

        if (subjectDuration == 0) // Make sure debuff is only applied upon entering the hazard
        {
            subjectDuration = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timeAlive < lifeTime) // Checks to see if the hazard has lasted through its duration on the ground
        {
            timeAlive += Time.deltaTime;
        }
        else if (!deactivated) // Disable its ability to apply debuffs
        {
            deactivated = true;
            graphic.Stop();
            col.enabled = false;
            StartCoroutine(DeleteAfterTime()); // Keep it around in case anything has leftover debuffs, then destroy the hazard
        }
        
        // Tick debuffs
        timeSinceTick += Time.deltaTime;
        timeSinceTickInside += Time.deltaTime;

        if (timeSinceTick >= tickRate && overTime) // If hazard has over-time effects and its tick was procked
        {
            timeSinceTick = 0;
            List<DamageableCharacter> toRemove = new List<DamageableCharacter>(); // List of all targets that should no longer be effected

            foreach (DamageableCharacter dChar in characters) // Check targets already debuffed
            {
                if (dChar && dChar.timePair.ContainsKey(this) && dChar.timePair[this] > 0) // If target has this hazard's debuff still affecting it and there's time left on it
                {
                    dChar.DebuffHit(this); // Hit it with another tick

                    // Reapply debuff (refresh duration) if the character is standing in the hazard
                    RaycastHit[] hit = Physics.SphereCastAll(transform.TransformPoint(col.center), col.radius * size, Vector3.up, 0);
                    foreach (RaycastHit raycastHit in hit)
                    {
                        DamageableCharacter character = raycastHit.transform.root.GetComponent<DamageableCharacter>();
                        if (character && character.Targetable && character.Equals(dChar) && col.enabled)
                        {
                            dChar.ApplyDebuff(this);
                        }
                    }
                }
                else // If the effect has worn off
                {
                    if (dChar)
                    {
                        dChar.RemoveDebuff(this); // Remove debuff from character
                        toRemove.Add(dChar);
                    }
                }
            }

            // Remove targets that had the debuff run out
            foreach(DamageableCharacter dChar in toRemove)
            {
                characters.Remove(dChar);
            }
        }
        else if (timeSinceTickInside >= 1 && !overTime) // Reapply debuff if the character is standing in the hazard, even if it doesn't have any over-time effects
        {
            timeSinceTickInside = 0;

            foreach (DamageableCharacter dChar in characters)
            {
                if (dChar)
                {
                    dChar.RemoveDebuff(this); // Remove debuff from character
                }
            }

            characters.Clear();

            RaycastHit[] hit = Physics.SphereCastAll(transform.TransformPoint(col.center), col.radius * size, Vector3.up, 0);
            foreach (RaycastHit raycastHit in hit)
            {
                DamageableCharacter character = raycastHit.transform.root.GetComponent<DamageableCharacter>();
                if (character && character.Targetable && !characters.Contains(character) && col.enabled)
                {
                    characters.Add(character);
                    character.ApplyDebuff(this);
                    character.DebuffHit(this);
                }
            }
        }
    }

    IEnumerator DeleteAfterTime()
    {
        yield return new WaitForSeconds(subjectDuration + 3);
        characters.Clear();
        Destroy(gameObject);
    }
}
