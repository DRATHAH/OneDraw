using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;
    public GameObject prefab;
    public int id = 0;
    [TextArea]
    public string desc;
    [Tooltip("If a consumable, the value that it gives upon consumption")]
    public float value = 5;
    [Tooltip("How much item sells for.")]
    public int price = 0;
    public bool isDefaultItem = false;

    public virtual void Use()
    {
        Debug.Log("Using " + name);

        // Find which item to actually use
        string noSpaceName = name.Replace(" ", "");
        UnityAction useAction = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), this, noSpaceName);
        useAction.Invoke();
    }

    #region Item Use Functions

    /*
     *  ====IMPORTANT====
     *  
     *  When making these functions, be sure that the name of the function IS THE SAME NAME as the name VARIABLE of the item
     *  you are making the function for! Otherwise, you will not be able to use the item!
     *  
     */

    void Coin()
    {
        PlayerStats.Instance.UpdateCoins(price);
    }

    void HealthPotion()
    {
        GameManager.instance.player.Heal((int)value);
    }

    #endregion
}
