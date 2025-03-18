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
    public int frostStacks = PlayerStats.Instance.bow.frostStacks;
    public int fireStacks = PlayerStats.Instance.bow.fireStacks;
    public int lightningStacks = PlayerStats.Instance.bow.lightningStacks;

    [Header("UI")]
    public TMP_Text chargeIndicator;
    public GameObject UIText;
    public GameObject arrowIcon;
    public GameObject frostIcon;
    public GameObject fireIcon;
    public GameObject lightningIcon;

    [Header("SFX")]
    public AudioSource bowDrawSFX;
    public AudioSource bowShootSFX;
    public AudioSource ArrowPickupSFX;

    float shootProgress = 0; // How long the player has held the mouse down for
    bool drawAudioPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Cursor.lockState == CursorLockMode.Locked)
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
                bowDrawSFX.Play(0);
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

        if (Input.GetKeyUp(KeyCode.Mouse0) && hasArrow && Cursor.lockState == CursorLockMode.Locked && canShoot)
        {
            UIText.SetActive(false);
            bowDrawSFX.Stop();
            drawAudioPlaying = false;
            bowShootSFX.Play(0);
            hasArrow = false;
            float speed = shootStr * (shootProgress / 100);
            float knockback = knockbackForce * (shootProgress / 100);

            GameObject arrow = Instantiate(arrowPrefab, arrowSpawn.position + arrowSpawn.forward * spawnOffset, arrowSpawn.rotation);
            arrow.GetComponent<Arrow>().Initialize(arrowSpawn.forward, speed, shootStr, arrowDamage, knockback, frostStacks, fireStacks, lightningStacks);
            shootProgress = 0;
        }
        arrowIcon.SetActive(hasArrow);
        frostIcon.SetActive(frostStacks > 0);
        fireIcon.SetActive(fireStacks > 0);
        lightningIcon.SetActive(lightningStacks > 0);
    }

    public void PlayPickupSound()
    {
        ArrowPickupSFX.Play(0);
    }
}
