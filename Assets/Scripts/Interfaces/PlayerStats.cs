using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Allows script to be directly referenced without variables and saves it across scenes
    #region Singleton

    private static PlayerStats instance;
    public static PlayerStats Instance
    {
        get => instance;
        private set
        {
            if (instance == null)
            {
                instance = value;
                DontDestroyOnLoad(value);
            }
            else if (instance != value)
            {
                Debug.Log($"{nameof(PlayerStats)} intance already exists, destroying duplicate");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public Bow bow;
    [SerializeField] Bow defaultBow;
    public int coins = 0;
    public int score = 0;
    public int health = 10;
    public int maxHealth = 10;

    public List<HazardStats> hazardTypes = new List<HazardStats>();

    Dictionary<HazardStats.HazardType, HazardStats> stacks = new Dictionary<HazardStats.HazardType, HazardStats>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(HazardStats type in hazardTypes) // Make new bow upgrades based on the default bow upgrade stats in Prefabs > Particles
        {
            HazardStats newStats = (HazardStats)ScriptableObject.CreateInstance(typeof(HazardStats));
            newStats.type = type.type;
            newStats.stacks = type.stacks;
            newStats.stuns = type.stuns;
            newStats.stunDuration = type.stunDuration;
            newStats.stunScale = type.stunScale;
            newStats.damage = type.damage;
            newStats.damageScale = type.damageScale;
            newStats.damageVulnerability = type.damageVulnerability;
            newStats.vulnerabilityScale = type.vulnerabilityScale;
            newStats.lifeTime = type.lifeTime;
            newStats.lifeScale = type.lifeScale;
            newStats.size = type.size;
            newStats.sizeScale = type.sizeScale;
            newStats.tickRate = type.tickRate;
            newStats.tickScale = type.tickScale;
            newStats.subjectDuration = type.subjectDuration;
            newStats.subjectScale = type.subjectScale;
            newStats.idleSound = type.idleSound;

            stacks.Add(type.type, newStats);
        }

        if (!bow) // Make new bow stats for the player based off of the defaultBow object in Prefabs > Items
        {
            bow = (Bow)ScriptableObject.CreateInstance(typeof(Bow));
            bow.dmg = defaultBow.dmg;
            bow.fireStrength = defaultBow.fireStrength;
            bow.drawSpeed = defaultBow.drawSpeed;
            bow.knockbackForce = defaultBow.knockbackForce;
        }

        UpdateBowStats();
    }

    public void ModifyBowStats(int dmgMod, float fireStrengthMod, float drawSpeedMod, float knockbackForceMod, HazardStats.HazardType typeMod)
    {
        if (bow)
        {
            bow.dmg += dmgMod;
            bow.fireStrength += fireStrengthMod;
            bow.drawSpeed += drawSpeedMod;
            bow.knockbackForce += knockbackForceMod;

            foreach(HazardStats.HazardType type in stacks.Keys)
            {
                if (typeMod.Equals(stacks[type].type))
                {
                    stacks[type].stacks++;
                }
            }

            UpdateBowStats();
        }
    }

    public void UpdateBowStats()
    {
        PlayerShoot shootStats = GameManager.instance.player.GetComponent<PlayerShoot>();
        shootStats.UpdateStats(bow.dmg, bow.fireStrength, bow.drawSpeed, stacks);
    }

    public void UpdateCoins(int newCoins)
    {
        coins += newCoins;
        HealthManager.instance.UpdateCoins(coins);
    }
}
