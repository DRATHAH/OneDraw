using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerShoot : MonoBehaviour
{
    [Header("Stats")]
    public bool canShoot = true;
    public bool hasArrow = true;
    public GameObject arrowPrefab;
    public Transform arrowSpawn;
    public GameObject soundParticle;
    public float spawnOffset;
    public float shootStr = 100f; // How fast the arrow moves when pulled all the way back
    [Tooltip("How fast the player draws the arrow back out of 100. Ex: a value of 25 takes the player 4 seconds to draw all the way.")]
    public float drawSpeed = 25;
    [Tooltip("How much damage the arrow does at max draw strength")]
    public int arrowDamage = 10;
    [Tooltip("How much knockback the arrow inflicts on targets")]
    public float knockbackForce = 0;

    public enum UpgradeType
    {
        frost,
        fire,
        lightning,
    }

    [Header("Upgrade Stats")]
    public UpgradeType upgradeType = UpgradeType.frost;
    public List<HazardStats> arrowUpgrades = new List<HazardStats>();

    [Header("UI")]
    public TMP_Text chargeIndicator;
    public GameObject UIText;
    public GameObject arrowIcon;
    public List<GameObject> elementIcons = new List<GameObject>();

    [Header("SFX")]
    public AudioSource bowDrawSFX;
    public AudioSource bowShootSFX;
    public AudioSource ArrowPickupSFX;

    float shootProgress = 0; // How long the player has held the mouse down for
    bool drawAudioPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject icon in elementIcons)
        {
            icon.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Cursor.lockState == CursorLockMode.Locked) // Make sure player isn't interacting with UI or anything else
        {
            canShoot = true;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            canShoot = false;
        }

        if (Input.GetKey(KeyCode.Mouse0) && hasArrow && Cursor.lockState == CursorLockMode.Locked && canShoot) // Checks to make sure player can draw back bow first
        {
            UIText.SetActive(true);
            if (!drawAudioPlaying)
            {
                bowDrawSFX.Play();
                drawAudioPlaying = true;
            }
            if (shootProgress < 100)
            {
                shootProgress += drawSpeed * Time.deltaTime;
            }
            else if (shootProgress > 100)
            {
                shootProgress = 100;
            }
            chargeIndicator.text = (int)shootProgress + "%";
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && hasArrow && Cursor.lockState == CursorLockMode.Locked && canShoot) // Shoot bow when mouse button released
        {
            UIText.SetActive(false);
            bowDrawSFX.Stop();
            drawAudioPlaying = false;

            GameObject sound = Instantiate(soundParticle, transform.position, Quaternion.identity);
            sound.GetComponent<SoundObject>().Initialize(bowShootSFX);

            hasArrow = false;
            float speed = shootStr * (shootProgress / 100);
            float knockback = knockbackForce * (shootProgress / 100);

            GameObject arrow = Instantiate(arrowPrefab, arrowSpawn.position + arrowSpawn.forward * spawnOffset, arrowSpawn.rotation);
            arrow.GetComponent<Arrow>().Initialize(arrowSpawn.forward, speed, shootStr, arrowDamage, knockback, arrowUpgrades);
            shootProgress = 0;
        }
        arrowIcon.SetActive(hasArrow);
    }

    public void UpdateStats(int newDmg, float newFireStr, float newDrawSpeed, Dictionary<HazardStats.HazardType, HazardStats> newStacks)
    {
        arrowDamage = newDmg;
        shootStr = newFireStr;
        drawSpeed = newDrawSpeed;

        arrowUpgrades.Clear(); // Clear previous upgrades
        foreach (HazardStats.HazardType type in newStacks.Keys)
        {
            arrowUpgrades.Add(newStacks[type]);
        }

        foreach (HazardStats hazard in arrowUpgrades)
        {
            foreach (GameObject icon in elementIcons)
            {
                if (icon.name.ToLower().Contains(hazard.type.ToString()) && hazard.stacks > 0)
                {
                    icon.SetActive(true);
                    TMP_Text stackNum = icon.GetComponentInChildren<TMP_Text>();
                    if (stackNum)
                    {
                        stackNum.text = hazard.stacks.ToString();
                    }
                }
            }
        }
    }

    public void PlayPickupSound()
    {
        ArrowPickupSFX.Play(0);
    }
}
