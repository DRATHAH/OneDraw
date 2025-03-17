using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerShoot : MonoBehaviour
{
    public bool hasArrow = true;
    public GameObject arrowPrefab;
    public Transform arrowSpawn;
    public float spawnOffset;
    public float shootStr = 100f; // How fast the arrow moves when pulled all the way back
    [Tooltip("Percentile number that determines shoot strength")]
    public float shootMod = 100;
    [Tooltip("How fast the player draws the arrow back")]
    public float drawSpeed = 25;
    [Tooltip("How much damage the arrow does at max draw strength")]
    public float arrowDamage = 10;
    [Tooltip("How much knockback the arrow inflicts on targets")]
    public float knockbackForce = 0;
    public TMP_Text chargeIndicator;
    public GameObject UIText;
    public GameObject arrowIcon;
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
        if (Input.GetKey(KeyCode.Mouse0) && hasArrow)
        {
            UIText.SetActive(true);
            if (!drawAudioPlaying)
            {
                bowDrawSFX.Play(0);
                drawAudioPlaying = true;
            }
            if (shootProgress < shootMod)
            {
                shootProgress += drawSpeed * Time.deltaTime;
            }
            else if (shootProgress > shootMod)
            {
                shootProgress = shootMod;
            }
            chargeIndicator.text = (int)shootProgress + "%";
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && hasArrow)
        {
            UIText.SetActive(false);
            bowDrawSFX.Stop();
            drawAudioPlaying = false;
            bowShootSFX.Play(0);
            hasArrow = false;
            float speed = shootStr * (shootProgress / shootMod);
            float dmgCalc = arrowDamage * (shootProgress / shootMod) + 0.5f;
            int dmg = (int)dmgCalc;
            float knockback = knockbackForce * (shootProgress / shootMod);

            GameObject arrow = Instantiate(arrowPrefab, arrowSpawn.position + arrowSpawn.forward * spawnOffset, arrowSpawn.rotation);
            arrow.GetComponent<Arrow>().Initialize(arrowSpawn.forward, speed, dmg, knockback);
            shootProgress = 0;
        }
        arrowIcon.SetActive(hasArrow);
    }

    public void PlayPickupSound()
    {
        ArrowPickupSFX.Play(0);
    }
}
