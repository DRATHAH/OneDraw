using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image healthBar;
    public int healthAmount = 10;
    public int maxHealth = 10;
    // Start is called before the first frame update
    void Start()
    {
        
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
        healthAmount = (int)Mathf.Clamp(healthAmount, 0, maxHealth);
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
        healthAmount = (int)Mathf.Clamp(healthAmount, 0, maxHealth);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)healthAmount / (float)maxHealth;
    }
}
