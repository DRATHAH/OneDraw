using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    public HazardStats stats;
    public TMP_Text text;
    public Image buttonBackground;
    public Image icon;
    public List<Sprite> cardIcons = new List<Sprite>();
    public List<Color> cardColors = new List<Color>();
    public int damage = 0;
    public float fireStrength = 0;
    public float drawSpeed = 0;
    public float knockbackForce = 0;

    public void Initialize(HazardStats baseStats)
    {
        stats = (HazardStats)ScriptableObject.CreateInstance(typeof(HazardStats));
        stats.type = baseStats.type;

        stats.stacks = baseStats.stacks;
        stats.stuns = baseStats.stuns;
        stats.stunDuration = baseStats.stunDuration;
        stats.damage = baseStats.damage + (int)((stats.stacks - 1) * baseStats.damageScale + .5f);
        stats.damageVulnerability = baseStats.damageVulnerability + ((stats.stacks - 1) * baseStats.vulnerabilityScale);
        stats.lifeTime = baseStats.lifeTime + ((stats.stacks - 1) * baseStats.lifeScale);
        stats.size = baseStats.size + ((stats.stacks - 1) * baseStats.sizeScale);
        stats.tickRate = baseStats.tickRate - ((stats.stacks - 1) * baseStats.tickScale);
        stats.subjectDuration = baseStats.subjectDuration + ((stats.stacks - 1) * baseStats.subjectScale);

        text.text = stats.type.ToString().ToUpper();
        foreach (Sprite sprite in cardIcons)
        {
            if (sprite.name.ToLower().Contains(stats.type.ToString()))
            {
                icon.sprite = sprite;
                int i = cardIcons.IndexOf(sprite);
                buttonBackground.color = cardColors[i];
            }
        }
    }

    public void SendStats()
    {
        PlayerStats.Instance.ModifyBowStats(damage, fireStrength, drawSpeed, knockbackForce, stats.type);
    }
}
