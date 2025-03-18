using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    #region Singleton
    public static HealthManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of HealthManager found!");
            return;
        }
        instance = this;
    }

    #endregion

    public Image healthBar;
    public int healthAmount = 10;
    public int maxHealth = 10;

    PlayerStats statsManager;

    // Start is called before the first frame update
    void Start()
    {
        statsManager = PlayerStats.Instance;

        healthAmount = statsManager.health;
        maxHealth = statsManager.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            TakeDamage(1);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Heal(1);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            GainMaxHealth(1);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            LoseMaxHealth(1);
        }
    }

    public void TakeDamage(int damage)
    {
        healthAmount -= damage;
        UpdateHealthBar();
    }

    public void Heal(int healAmount)
    {
        healthAmount += healAmount;
        ClampHealth();
        UpdateHealthBar();
    }

    public void GainMaxHealth(int gainAmount)
    {
        maxHealth += gainAmount;
        UpdateHealthBar();
    }

    public void LoseMaxHealth(int loseAmount)
    {
        maxHealth -= loseAmount;
        ClampHealth();
        UpdateHealthBar();
    }

    public void setMaxHealth(int amount)
    {
        maxHealth = amount;
        ClampHealth();
        UpdateHealthBar();
    }

    // Using this method updates the Health Bar Full Image proportional to current hp / max hp
    void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)healthAmount / (float)maxHealth;
    }

    // Using this method prevents hp from exceeding Max hp
    void ClampHealth()
    {
        healthAmount = (int)Mathf.Clamp(healthAmount, 0, maxHealth);
    }
}
