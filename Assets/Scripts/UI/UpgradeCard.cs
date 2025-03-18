using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeCard : MonoBehaviour
{
    public string cardName = "Frost";
    public TMP_Text text;
    public int damage = 0;
    public float fireStrength = 0;
    public float drawSpeed = 0;
    public float knockbackForce = 0;
    public int frostStacks = 0;
    public int fireStacks = 0;
    public int lightningStacks = 0;

    private void Start()
    {
        text.text = cardName;
    }

    public void SendStats()
    {
        PlayerStats.Instance.ModifyBowStats(damage, fireStrength, drawSpeed, knockbackForce, frostStacks, fireStacks, lightningStacks);
    }
}
