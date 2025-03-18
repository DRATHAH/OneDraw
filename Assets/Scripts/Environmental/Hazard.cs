using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public enum EffectType
    {
        frost,
        fire,
        lightning,
    }
    public EffectType type = EffectType.frost;
    public ParticleSystem graphic;

    [Header("Things that effect a DamageableCharacter")]
    [Tooltip("Multipliers for effects.")]
    public int stacks = 1;
    [Tooltip("Damage of initial entrance and DoT effects.")]
    public int damage = 0;
    [Tooltip("How much a stack scales damage (additive).")]
    [SerializeField] float damageScale = 1;
    [Tooltip("How much more damage an object will take. Ex: a value of 2 = x2 damage.")]
    public float damageVulnerability = 1;
    [Tooltip("How much a stack scales damage vulnerability (additive).")]
    [SerializeField] float vulnerabilityScale = 1;

    [Header("Hazard Stats")]
    [Tooltip("How long the effect lasts on the ground.")]
    public int lifeTime = 0;
    [Tooltip("How much a stack scales lifetime (additive).")]
    [SerializeField] float lifeScale = 1;
    [Tooltip("Size modifier. For arrow upgrades, scales with stacks.")]
    public float size = 1;
    [Tooltip("How much a stack scales size (additive).")]
    [SerializeField] float sizeScale = 1;
    [Tooltip("Whether to have over-time effects or not.")]
    public bool overTime = false;
    [Tooltip("How fast effects like DoT proc.")]
    public float tickRate = 1;
    [Tooltip("How much a stack scales tick rate (subtractive).")]
    [SerializeField] float tickScale = 1;
    [Tooltip("How long the effect lasts on a target (can carry over after a target leaves the area).")]
    public float subjectDuration = 0;
    [Tooltip("How much a stack scales subject duration (additive).")]
    [SerializeField] float subjectScale = 1;

    [HideInInspector] public float timeSinceTick = 0;

    float timeAlive = 0;
    bool deactivated = false;
    SphereCollider col;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<SphereCollider>();

        RaycastHit[] hit = Physics.SphereCastAll(transform.position, col.radius, Vector3.up, 0);
        foreach(RaycastHit raycastHit in hit)
        {
            DamageableCharacter character = raycastHit.transform.root.GetComponent<DamageableCharacter>();
            if (character && character.Targetable)
            {
                character.OnHitDot(this);
            }
        }
    }

    public void Initialize(int stk, int dmg, float dmgVul, int life, float sizeMod, float tick, float subDur)
    {
        stacks = stk;
        damage = dmg + (int)((stacks-1) * damageScale + .5f);
        damageVulnerability = dmgVul + ((stacks - 1) * vulnerabilityScale);
        lifeTime = life + (int)((stacks - 1) * lifeScale + .5f);
        size = sizeMod + ((stacks - 1) * sizeScale);
        tickRate = tick - ((stacks - 1) * tickScale);
        if (tickRate <= 0)
        {
            tickRate = 0.05f;
        }
        subjectDuration = subDur + ((stacks - 1) * subjectScale);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeAlive < lifeTime)
        {
            timeAlive += Time.deltaTime;
        }
        else if (!deactivated)
        {
            deactivated = true;
            graphic.Stop();
            col.enabled = false;
        }

        if (!overTime)
        {
            timeSinceTick = 100;
        }
        else
        {
            timeSinceTick += Time.deltaTime;
        }

        if (timeSinceTick >= tickRate)
        {
            timeSinceTick = 0;
        }
    }

    IEnumerator DeleteAfterTime()
    {
        yield return new WaitForSeconds(subjectDuration);
        Destroy(gameObject);
    }
}
