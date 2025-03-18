using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton

    public static Inventory instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }
        instance = this;
    }

    #endregion

    public delegate void OnItemChanged(); // When invoked, calls every method subscribed to this
    public OnItemChanged onItemChangedCallback; // Call after modifying inventory to update it

    [Tooltip("How many times an item can stack.")]
    public int itemSpace = 100;
    [Tooltip("How many unique items the player can hold.")]
    public int space = 10;

    public List<Item> uniqueItems = new List<Item>();
    public Dictionary<Item, int> items = new Dictionary<Item, int>();

    public bool Add(Item newItem)
    {
        if (!newItem.isDefaultItem)
        {
            if (items.ContainsKey(newItem) && items[newItem] >= itemSpace) // Modify item name so that 'items' doesn't have duplicate keys
            {
                Item tempItem = (Item)ScriptableObject.CreateInstance(typeof(Item));
                tempItem.name = newItem.name;
                tempItem.icon = newItem.icon;
                tempItem.prefab = newItem.prefab;
                tempItem.id = newItem.id;
                tempItem.desc = newItem.desc;
                tempItem.isDefaultItem = newItem.isDefaultItem;
                newItem = tempItem;
            }

            if (items.ContainsKey(newItem) && items[newItem] < itemSpace)
            {
                items[newItem]++;
            }
            else if (items.Count < space)
            {
                items.Add(newItem, 1);
                uniqueItems.Add(newItem);
            }
            else if (items.Count > space)
            {
                return false;
            }

            if (onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke();
            }
        }

        return true;
    }

    public void Remove(Item newItem)
    {
        if (items[newItem] == 1)
        {
            items.Remove(newItem);
            uniqueItems.Remove(newItem);
        }
        else
        {
            items[newItem]--;
        }

        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }

    public void ClearInventory()
    {
        items.Clear();
        uniqueItems.Clear();
        Debug.LogWarning("Cleared Inventory");

        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }
}
