using UnityEngine;

[CreateAssetMenu(fileName = "New Hazard Stat", menuName = "Environmental/Hazard Stat")]
public class HazardStats : ScriptableObject
{

    public enum HazardType
    {
        frost,
        fire,
        lightning,
    }
    public HazardType type;

    [Tooltip("Multipliers for effects.")]
    public int stacks = 1;
    [Tooltip("Whether it disables movement for a short while.")]
    public bool stuns = false;
    [Tooltip("Duration of the stun in seconds.")]
    public float stunDuration = 0;
    [Tooltip("How much a stack scales stun duration (additive).")]
    public float stunScale = 0;
    [Tooltip("Damage of initial entrance and DoT effects.")]
    public int damage = 0;
    [Tooltip("How much a stack scales damage (additive).")]
    public float damageScale = 0;
    [Tooltip("How much more damage an object will take. Ex: a value of 2 = x2 damage.")]
    public float damageVulnerability = 0;
    [Tooltip("How much a stack scales damage vulnerability (additive).")]
    public float vulnerabilityScale = 0;
    [Tooltip("How long the effect lasts on the ground.")]
    public float lifeTime = 0;
    [Tooltip("How much a stack scales lifetime (additive).")]
    public float lifeScale = 0;
    [Tooltip("Size modifier. For arrow upgrades, scales with stacks.")]
    public float size = 0;
    [Tooltip("How much a stack scales size (additive).")]
    public float sizeScale = 0;
    [Tooltip("How fast effects like DoT proc.")]
    public float tickRate = 1;
    [Tooltip("How much a stack scales tick rate (subtractive).")]
    public float tickScale = 0;
    [Tooltip("How long the effect lasts on a target (can carry over after a target leaves the area).")]
    public float subjectDuration = 0;
    [Tooltip("How much a stack scales subject duration (additive).")]
    public float subjectScale = 0;
    [Tooltip("Idle sound for the hazard.")]
    public AudioClip idleSound;
}
