using UnityEngine;

[CreateAssetMenu(fileName = "New Bow", menuName = "Inventory/Bow")]
public class Bow : ScriptableObject
{
    new public string name = "New Bow";
    public GameObject prefab;
    public int dmg = 1;
    public float fireStrength = 3500;
    public float drawSpeed = 50;
    public float knockbackForce = 0;
    public int frostStacks = 0;
    public int fireStacks = 0;
    public int lightningStacks = 0;
}
