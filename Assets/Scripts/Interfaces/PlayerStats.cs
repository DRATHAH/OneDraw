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

    public PlayerMovement player;
    public Bow bow;
    [SerializeField] Bow defaultBow;
    public int coins = 0;
    public int score = 0;
    public int health = 10;
    public int maxHealth = 10;

    // Start is called before the first frame update
    void Start()
    {
        if (!bow)
        {
            bow = (Bow)ScriptableObject.CreateInstance(typeof(Bow));
            bow.dmg = defaultBow.dmg;
            bow.fireStrength = defaultBow.fireStrength;
            bow.drawSpeed = defaultBow.drawSpeed;
            bow.knockbackForce = defaultBow.knockbackForce;
            bow.frostStacks = defaultBow.frostStacks;
            bow.fireStacks = defaultBow.fireStacks;
            bow.lightningStacks = defaultBow.lightningStacks;
        }
    }

    public void ModifyBowStats(int dmgMod, float fireStrengthMod, float drawSpeedMod, float knockbackForceMod, int frostStackMod, int fireStackMod, int lightningStackMod)
    {
        if (bow)
        {
            bow.dmg += dmgMod;
            bow.fireStrength += fireStrengthMod;
            bow.drawSpeed += drawSpeedMod;
            bow.knockbackForce += knockbackForceMod;
            bow.frostStacks += frostStackMod;
            bow.fireStacks += fireStackMod;
            bow.lightningStacks += lightningStackMod;

            UpdateBowStats();
        }
    }

    public void UpdateBowStats()
    {
        PlayerShoot shootStats = player.GetComponent<PlayerShoot>();
        shootStats.arrowDamage = bow.dmg;
        shootStats.shootStr = bow.fireStrength;
        shootStats.drawSpeed = bow.drawSpeed;
        shootStats.frostStacks = bow.frostStacks;
        shootStats.fireStacks = bow.fireStacks;
        shootStats.lightningStacks = bow.lightningStacks;
    }

    
}
