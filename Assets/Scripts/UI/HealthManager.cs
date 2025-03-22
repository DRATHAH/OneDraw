using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public TMP_Text coinCounter;
    public int healthAmount = 10;
    public int maxHealth = 10;

    public Animator deathAnimation;

    PlayerStats statsManager;

    // Start is called before the first frame update
    void Start()
    {
        statsManager = PlayerStats.Instance;

        coinCounter.text = statsManager.coins.ToString();
        healthAmount = statsManager.health;
        maxHealth = statsManager.maxHealth;
    }

    // Using this method updates the Health Bar Full Image proportional to current hp / max hp
    public void UpdateHealthBar(int newHealth)
    {
        healthBar.fillAmount = (float)newHealth / (float)maxHealth;
    }

    public void UpdateCoins(int newCoins)
    {
        coinCounter.text = newCoins.ToString();
    }

    public void PlayDeathAnimation()
    {
        deathAnimation.SetTrigger("Start");
    }
}
